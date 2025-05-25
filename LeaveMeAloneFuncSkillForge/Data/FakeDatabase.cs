using Bogus;
using LeaveMeAloneFuncSkillForge.Domain;

namespace LeaveMeAloneFuncSkillForge.Data
{
    public static class FakeDatabase
    {
        public static Faker<Film> FilmFaker = new Faker<Film>()
            .RuleFor(f => f.Title, f => f.Lorem.Sentence(3))
            .RuleFor(f => f.Genre, f => f.PickRandom("Action", "Comedy", "Drama"));

        public static Faker<TaskData> TaskFaker = new Faker<TaskData>()
            .RuleFor(t => t.EstimatedHours, f => f.Random.Int(1, 80))
            .RuleFor(t => t.ComplexityLevel, f => f.Random.Int(1, 10))
            .RuleFor(t => t.IsUrgent, f => f.Random.Bool(0.3f))
            .RuleFor(t => t.AssignedDeveloper, f => f.Name.FullName())
            .RuleFor(t => t.BackupDeveloper, (f, t) => f.Name.FullName())
            .RuleFor(t => t.CreatedDate, f => f.Date.Recent(30))
            .RuleFor(t => t.DueDate, (f, t) => f.Date.Between(t.CreatedDate.AddDays(1), t.CreatedDate.AddDays(30)));
    }
}
