using Bogus;
using LeaveMeAloneFuncSkillForge.Domain;

namespace LeaveMeAloneFuncSkillForge.Data
{
    public static class FakeDatabase
    {
        public static Faker<Film> FilmFaker = new Faker<Film>()
        .RuleFor(f => f.Title, f => f.Lorem.Sentence(3))
        .RuleFor(f => f.Genre, f => f.PickRandom("Action", "Comedy", "Drama"));
    }
}
