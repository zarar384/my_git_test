using LeaveMeAloneCSharp.Domain;
using Microsoft.EntityFrameworkCore;

namespace LeaveMeAloneCSharp.Test.Db
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        public DbSet<Film> Films => Set<Film>();
    }
}
