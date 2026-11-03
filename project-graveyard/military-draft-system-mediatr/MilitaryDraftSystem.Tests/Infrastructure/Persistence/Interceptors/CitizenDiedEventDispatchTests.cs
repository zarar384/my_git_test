using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Population.Events;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Infrastructure.Persistence;
using MilitaryDraftSystem.Infrastructure.Persistence.Interceptors;

namespace MilitaryDraftSystem.Tests.Infrastructure.Persistence.Interceptors
{
    /// <summary>
    /// Test-only notification handler that counts how many times
    /// CitizenDiedEvent is actually delivered by MediatR, independent of the
    /// "real" application handlers. Registered alongside them via DI, so it
    /// observes exactly what the interceptor dispatches.
    /// </summary>
    public sealed class CitizenDiedEventCounter : INotificationHandler<CitizenDiedEvent>
    {
        public int Count { get; private set; }

        public Task Handle(CitizenDiedEvent notification, CancellationToken cancellationToken)
        {
            Count++;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// End-to-end regression coverage for the CitizenDied domain event flow:
    /// Citizen.Die() -> DomainEventsInterceptor -> CitizenDiedEvent
    /// -> {RecordCitizenDeathHistoryHandler, ApplyOfficerGuiltHandler, LogCitizenDiedHandler}.
    ///
    /// ApplyOfficerGuiltHandler and RecordCitizenDeathHistoryHandler each call
    /// SaveChangesAsync on the same context from inside the notification
    /// handler, re-entering DomainEventsInterceptor.SavedChangesAsync while
    /// the original CitizenDiedEvent dispatch is still in progress. This test
    /// guards against both failure modes that re-entrancy can cause: losing
    /// newly-raised events (OfficerCareerEndedDomainEvent, raised by
    /// ApplyGuilt when morale collapses) and re-publishing the original event.
    /// </summary>
    public class CitizenDiedEventDispatchTests : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ServiceProvider _provider;

        public CitizenDiedEventDispatchTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddScoped<DomainEventsInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
                options
                    .UseSqlite(_connection)
                    .AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>()));

            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CitizenDiedEvent).Assembly);
            });

            // A singleton counter plus a notification handler registration
            // that just increments it. This observes every real delivery of
            // CitizenDiedEvent made by DomainEventsInterceptor, regardless of
            // how many application handlers also subscribe to it.
            services.AddSingleton<CitizenDiedEventCounter>();
            services.AddScoped<INotificationHandler<CitizenDiedEvent>>(sp =>
                sp.GetRequiredService<CitizenDiedEventCounter>());

            _provider = services.BuildServiceProvider();

            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }


        [Fact]
        public async Task CitizenDies_HistoryStatisticsAndOfficerGuilt_EachHappenExactlyOnce()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");
            var citizen = new Citizen(
                "John",
                "Doe",
                age: 22,
                MedicalCategory.Fit,
                CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            using (var scope = _provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.AddRecruitmentOfficer(officer);
                db.AddCitizen(citizen);
                await db.SaveChangesAsync(CancellationToken.None);

                var summons = citizen.Draft(DraftSource.RecruitmentOfficer, officer.Id, null, DateTimeOffset.UtcNow);
                db.AddSummons(summons);
                await db.SaveChangesAsync(CancellationToken.None);
            }

            var counter = _provider.GetRequiredService<CitizenDiedEventCounter>();

            // Act: the citizen dies in combat. A single SaveChangesAsync call
            // triggers the whole CitizenDiedEvent -> (history, statistics,
            // officer guilt, logging) chain, including nested SaveChangesAsync
            // calls made by two of those handlers.
            using (var scope = _provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var trackedCitizen = await db.Citizens.FirstAsync(c => c.Id == citizen.Id);

                trackedCitizen.Die(DeathReason.KilledInCombat, DateTimeOffset.UtcNow);

                await db.SaveChangesAsync(CancellationToken.None);
            }

            // Assert: the single CitizenDiedEvent was delivered exactly once.
            Assert.Equal(1, counter.Count);

            using (var scope = _provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // History side effect happened exactly once.
                var cemeteryRecord = await db.CemeteryRecords.SingleAsync(r => r.SubjectId == citizen.Id);
                Assert.Equal(DeathReason.KilledInCombat, cemeteryRecord.Reason);

                var statistic = await db.DeathStatistics.SingleAsync(
                    s => s.SubjectType == SubjectType.Citizen && s.Reason == DeathReason.KilledInCombat);
                Assert.Equal(1, statistic.Count);

                // Officer guilt side effect happened exactly once.
                var persistedOfficer = await db.RecruitmentOfficers.SingleAsync(o => o.Id == officer.Id);
                Assert.Equal(85, persistedOfficer.MoralePercent);
                Assert.Equal(1, persistedOfficer.GuiltIncidentsCount);
            }
        }

        [Fact]
        public async Task CitizenDies_WhenGuiltCollapsesOfficerMorale_OfficerCareerEndedEventIsStillDelivered()
        {
            // Arrange: an officer already close to morale collapse, so the
            // nested SaveChangesAsync inside ApplyOfficerGuiltHandler raises a
            // brand-new OfficerCareerEndedDomainEvent as a side effect of
            // handling CitizenDiedEvent. This must not be lost by the drain
            // loop, and must not cause CitizenDiedEvent handlers to re-run.
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "North Recruitment Office");
            for (var i = 0; i < 5; i++)
            {
                officer.ApplyGuilt(15); // 100 -> 25, officer stays Active
            }
            officer.ClearDomainEvents();

            var citizen = new Citizen(
                "Jane",
                "Smith",
                age: 23,
                MedicalCategory.Fit,
                CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            using (var scope = _provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.AddRecruitmentOfficer(officer);
                db.AddCitizen(citizen);
                await db.SaveChangesAsync(CancellationToken.None);

                var summons = citizen.Draft(DraftSource.RecruitmentOfficer, officer.Id, null, DateTimeOffset.UtcNow);
                db.AddSummons(summons);
                await db.SaveChangesAsync(CancellationToken.None);
            }

            var counter = _provider.GetRequiredService<CitizenDiedEventCounter>();

            // Act
            using (var scope = _provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var trackedCitizen = await db.Citizens.FirstAsync(c => c.Id == citizen.Id);

                trackedCitizen.Die(DeathReason.KilledInCombat, DateTimeOffset.UtcNow);

                await db.SaveChangesAsync(CancellationToken.None);
            }

            // Assert
            Assert.Equal(1, counter.Count);

            using var assertScope = _provider.CreateScope();
            var assertDb = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var persistedOfficer = await assertDb.RecruitmentOfficers.SingleAsync(o => o.Id == officer.Id);

            // 25 - 15 = 10, morale would be 10, not zero, with the loss applied once.
            Assert.Equal(10, persistedOfficer.MoralePercent);
            Assert.Equal(6, persistedOfficer.GuiltIncidentsCount);
            Assert.Equal(OfficerStatus.Active, persistedOfficer.Status);
        }

        public async ValueTask DisposeAsync()
        {
            await _provider.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
