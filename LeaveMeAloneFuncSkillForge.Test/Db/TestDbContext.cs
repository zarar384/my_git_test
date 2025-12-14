using LeaveMeAloneFuncSkillForge.Domain;
using Microsoft.EntityFrameworkCore;

namespace LeaveMeAloneFuncSkillForge.Test.Db
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        public DbSet<Film> Films => Set<Film>();
    }
}
