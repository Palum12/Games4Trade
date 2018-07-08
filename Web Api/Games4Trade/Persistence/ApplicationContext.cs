using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using MySql.Data.MySqlClient;

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

        private MySqlConnection GetConnection()
        {
            var sqlServerOptionsExtension = Options.FindExtension<SqlServerOptionsExtension>();
            return new MySqlConnection(sqlServerOptionsExtension.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
