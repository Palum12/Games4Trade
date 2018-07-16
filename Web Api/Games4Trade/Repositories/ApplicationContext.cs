using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace Games4Trade.Persistence
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<Dummy> Dummies { get; set; }
        public DbContextOptions<ApplicationContext> Options { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Options = options;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dummy>(entity => { entity.HasKey(d => d.Id); });

            base.OnModelCreating(modelBuilder);
        }
    }
}
