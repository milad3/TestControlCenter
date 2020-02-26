using Microsoft.EntityFrameworkCore;
using TestControlCenter.Properties;
using System.Data.SQLite;
using TestControlCenter.Services;
using TestControlCenterDomain;

namespace TestControlCenter.Data
{
    public class DatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var conn = new SQLiteConnection(Settings.Default.DatabaseConnection);
            // conn.SetPassword(SecurityTools.GetDatabasePassword()); TODO: set password again!
            conn.Open();

            optionsBuilder.UseSqlite(conn);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestMark>().HasOne(x => x.TestItem);
        }

        public override void Dispose()
        {
            var conn = Database.GetDbConnection();
            conn.Close();

            base.Dispose();
        }

        public DbSet<TestItem> TestItems { get; set; }

        public DbSet<TestItemQuestion> Questions { get; set; }

        public DbSet<TestMark> TestMarks { get; set; }
    }
}
