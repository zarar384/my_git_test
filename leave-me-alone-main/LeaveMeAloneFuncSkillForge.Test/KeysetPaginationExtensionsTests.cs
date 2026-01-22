using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Test.Db;
using Microsoft.EntityFrameworkCore;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class KeysetPaginationExtensionsTests
    {
 
        [Fact]
        public async Task FirstPageWithoutCursor_ReturnsFirstItems()
        {
            // Arrange
            var films = CreateFilms(10);
            await using var context = CreateContext(films);

            // Act
            var page = await context.Films
                .ToKeysetPageAsync(
                    keySelector: f => f.BoxOfficeRevenue,
                    lastKey: null,
                    pageSize: 3
                );

            // Assert
            Assert.Equal(3, page.Items.Count);
            Assert.True(page.HasNextPage);
            Assert.Equal(30, page.NextCursor);
        }

        [Fact]
        public async Task SecondPageWithCursor_ReturnsNextItems()
        {
            // Arrange
            var films = CreateFilms(10);
            await using var context = CreateContext(films);

            // First page
            var firstPage = await context.Films
                .ToKeysetPageAsync(
                    f => f.BoxOfficeRevenue,
                    lastKey: null,
                    pageSize: 4
                );

            // Act - second page
            var secondPage = await context.Films
                .ToKeysetPageAsync(
                    f => f.BoxOfficeRevenue,
                    lastKey: firstPage.NextCursor,
                    pageSize: 4
                );

            // Assert
            Assert.Equal(4, secondPage.Items.Count);
            Assert.True(secondPage.Items.First().BoxOfficeRevenue > firstPage.Items.Last().BoxOfficeRevenue);
        }


        [Fact]
        public async Task DescendingOrder_ReturnsHighestRevenueFirst()
        {
            // Arrange
            var films = CreateFilms(5);
            await using var context = CreateContext(films);

            // Act
            var page = await context.Films
                .ToKeysetPageAsync(
                    f => f.BoxOfficeRevenue,
                    lastKey: null,
                    pageSize: 2,
                    ascending: false
                );

            // Assert
            Assert.Equal(2, page.Items.Count);
            Assert.Equal(50, page.Items[0].BoxOfficeRevenue);
            Assert.Equal(40, page.Items[1].BoxOfficeRevenue);
        }


        [Fact]
        public async Task LastPageSets_HasNextPageFalseAndNextCursornull()
        {
            // Arrange
            var films = CreateFilms(3);
            await using var context = CreateContext(films);

            // Act
            var page = await context.Films
                .ToKeysetPageAsync(
                    f => f.BoxOfficeRevenue,
                    lastKey: 20,
                    pageSize: 5
                );

            // Assert
            Assert.False(page.HasNextPage);
            Assert.Null(page.NextCursor);
            Assert.Single(page.Items);
        }

        [Fact]
        public async Task PageSize_LessOrEqualZeroUsesDefault()
        {
            // Arrange
            var films = CreateFilms(30);
            await using var context = CreateContext(films);

            // Act
            var page = await context.Films
                .ToKeysetPageAsync(
                    f => f.BoxOfficeRevenue,
                    lastKey: null,
                    pageSize: 0
                );

            // Assert
            Assert.Equal(20, page.Items.Count);
            Assert.True(page.HasNextPage);
        }

        // Helpers
        private static TestDbContext CreateContext(List<Film> films)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new TestDbContext(options);
            context.Films.AddRange(films);
            context.SaveChanges();

            return context;
        }

        public static List<Film> CreateFilms(int count)
        {
            return Enumerable.Range(1, count)
                .Select(i => new Film
                {
                    Id = i,
                    Title = $"Film {i}",
                    Genre = i % 2 == 0 ? "Action" : "Drama",
                    BoxOfficeRevenue = i * 10
                })
                .ToList();
        }

    }

}
