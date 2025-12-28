
using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Functional;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class MemoizationTests
    {
        [Fact]
        public void Memorize_CallsUnderlyingFunction_OnlyOnceForSameInput()
        {
            var callCount = 0;

            Func<string, List<Film>> getFilmsByActor = actor =>
            {
                callCount++;
                return new List<Film>
            {
                    new Film { Id = 1, Title = "Film A" },
                    new Film { Id = 2, Title = "Film B" }
            };
            };

            var memoized = getFilmsByActor.Memorize();

            var r1 = memoized("Kevin Bacon");
            var r2 = memoized("Kevin Bacon");
            var r3 = memoized("Kevin Bacon");

            Assert.Equal(1, callCount);
            Assert.Equal(2, r1.Count);
            Assert.Same(r1, r2);
            Assert.Same(r2, r3);
        }

        [Fact]
        public void Memorize_ComputesOncePerUniqueKey()
        {
            var callCount = 0;

            Func<string, int> getActorScore = actor =>
            {
                callCount++;
                return actor.Length;
            };

            var memoized = getActorScore.Memorize();

            memoized("Tom Hanks");
            memoized("Tom Hanks");
            memoized("Meg Ryan");

            Assert.Equal(2, callCount);
        }

        [Fact]
        public void Memorize_TwoParameters_AvoidsRepeatedCalls()
        {
            var callCount = 0;

            Func<string, int, string> getFilmKey = (actor, year) =>
            {
                callCount++;
                return $"{actor}-{year}";
            };

            var memoized = getFilmKey.Memorize();

            memoized("Kevin Bacon", 1995);
            memoized("Kevin Bacon", 1995);
            memoized("Kevin Bacon", 1995);

            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Memorize_ObjectParameter_WithoutKeyGenerator_CallsMultipleTimes()
        {
            var callCount = 0;

            Func<Film, string> getFilmTitle = film =>
            {
                callCount++;
                return film.Title;
            };

            var memoized = getFilmTitle.Memorize();

            memoized(new Film { Id = 1, Title = "Apollo 13" });
            memoized(new Film { Id = 1, Title = "Apollo 13" });

            // Two different object references => two cache entries
            Assert.Equal(2, callCount);
        }


        [Fact]
        public void Memorize_ObjectParameter_WithKeyGenerator_CachesCorrectly()
        {
            var callCount = 0;

            Func<Film, string> getFilmTitle = film =>
            {
                callCount++;
                return film.Title;
            };

            var memoized = getFilmTitle.Memorize(film => film.Id.ToString());

            memoized(new Film { Id = 1, Title = "Apollo 13" });
            memoized(new Film { Id = 1, Title = "Apollo 13" });
            memoized(new Film { Id = 1, Title = "Apollo 13" });

            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Memorize_DoesNotPersist_BetweenInstances()
        {
            var callCount = 0;

            Func<int, int> square = x =>
            {
                callCount++;
                return x * x;
            };

            var memo1 = square.Memorize();
            memo1(10);
            memo1(10);

            var memo2 = square.Memorize();
            memo2(10);

            // new memoization scope => new cache
            Assert.Equal(2, callCount);
        }

        private static List<Film> CreateFilms() =>
        new()
        {
                new Film { Id = 1, Title = "Apollo 13", Genre = "Drama", BoxOfficeRevenue = 355 },
                new Film { Id = 2, Title = "In the Cut", Genre = "Drama", BoxOfficeRevenue = 152 },
                new Film { Id = 3, Title = "Top Gun", Genre = "Action", BoxOfficeRevenue = 777 },
                new Film { Id = 4, Title = "Mission Impossible", Genre = "Action", BoxOfficeRevenue = 791 },
                new Film { Id = 5, Title = "Random Indie", Genre = "Drama", BoxOfficeRevenue = 5 }
        };

        // (*) Expensive pure function without memoization
        [Fact]
        public void WithoutMemoization_ExpensiveLookup_IsRepeatedManyTimes()
        {
            var films = CreateFilms();

            var lookupCalls = 0;

            Func<string, Film?> slowLookup = title =>
            {
                lookupCalls++;
                Thread.Sleep(200); // simulate DB / API
                return films.SingleOrDefault(f => f.Title == title);
            };

            var format = FilmFuncs.GetFormattedFilmInfos(slowLookup);

            var titles = new[]
            {
                    "Apollo 13",
                    "Apollo 13",
                    "Apollo 13",
                    "Top Gun",
                    "Top Gun"
            };

            var result = format(titles).ToList();

            Assert.Equal(5, lookupCalls); // every call recomputed
            Assert.Equal(5, result.Count);
        }

        // (*) Same pipeline, single change: memoization
        [Fact]
        public void WithMemoization_ExpensiveLookup_IsComputedOncePerKey()
        {
            var films = CreateFilms();

            var lookupCalls = 0;

            Func<string, Film?> slowLookup = title =>
            {
                lookupCalls++;
                Thread.Sleep(200);
                return films.SingleOrDefault(f => f.Title == title);
            };

            var memoizedLookup = slowLookup.Memorize();

            var format = FilmFuncs.GetFormattedFilmInfos(memoizedLookup);

            var titles = new[]
            {
                    "Apollo 13",
                    "Apollo 13",
                    "Apollo 13",
                    "Top Gun",
                    "Top Gun"
                };

            var result = format(titles).ToList();

            Assert.Equal(2, lookupCalls); // massive reduction
            Assert.Equal(5, result.Count);
        }

        // Memoization composes naturally with FP pipelines
        [Fact]
        public void Memoization_ComposesWithGroupingAndAggregation()
        {
            var films = CreateFilms();

            var revenueCalls = 0;

            Func<Film, double> expensiveRevenueProjection = film =>
            {
                revenueCalls++;
                Thread.Sleep(100);
                return film.BoxOfficeRevenue;
            };

            var memoizedRevenue = expensiveRevenueProjection.Memorize(f => f.Id!.Value.ToString());

            var result = films.GroupBy(f => f.Genre).Select(g =>
                    {
                        var average = g.Average(memoizedRevenue);
                        return new
                        {
                            Genre = g.Key,
                            TopFilms = g.Where(f => memoizedRevenue(f) > average)
                        };
                    }).ToList();

            // each films revenue is computed only once
            Assert.Equal(films.Count, revenueCalls);
        }

        //  Memoization + laziness = FP superpower
        [Fact]
        public void Memoization_Preserves_LazyEvaluationButRemovesRepetition()
        {
            var films = CreateFilms();

            var expensiveCalls = 0;

            Func<Film, string> expensiveDescriptor = film =>
            {
                expensiveCalls++;
                Thread.Sleep(300);
                return $"Summary ready for {film.Title}";
            };

            var memoizedDescriptor =
                expensiveDescriptor.Memorize(f => f.Id!.Value.ToString());

            var analyzer = films
                .Select(f => memoizedDescriptor(f)) // lazy
                .Take(3)                            // partial evaluation
                .ToList();

            // only 3 evaluations, not all films
            Assert.Equal(3, expensiveCalls);

            // reusing memoized function
            var again = films.Select(memoizedDescriptor).ToList();

            // no extra work
            Assert.Equal(films.Count, expensiveCalls);
        }

        // Memoization enables recursive / multi-step algorithms
        [Fact]
        public void Memoization_Prevents_ExplosionInMultiStepAnalysis()
        {
            var films = CreateFilms();

            var scoreCalls = 0;

            Func<Film, double> score = film =>
            {
                scoreCalls++;
                Thread.Sleep(100);
                return film.BoxOfficeRevenue * 1.1;
            };

            var memoizedScore = score.Memorize(f => f.Id!.Value.ToString());

            // multiple passes over same data
            var total = films.Sum(memoizedScore)
                + films.Where(f => f.Genre == "Drama").Sum(memoizedScore)
                + memoizedScore(films.OrderByDescending(memoizedScore).First());

            // without memoization - dozens of calls
            // with memoization - once per film
            Assert.Equal(films.Count, scoreCalls);
        }
    }
}