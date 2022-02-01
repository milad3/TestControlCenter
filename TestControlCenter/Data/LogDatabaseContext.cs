using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;
using TestControlCenter.Properties;
using TestControlCenterDomain;

namespace TestControlCenter.Data
{
    public class LogDatabaseContext : DbContext
    {
        public DbSet<LogMessage> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var conn = new SQLiteConnection(Settings.Default.LogDatabaseConnection);
            conn.Open();

            optionsBuilder.UseSqlite(conn);
        }

        public override void Dispose()
        {
            var conn = Database.GetDbConnection();
            conn.Close();

            base.Dispose();
        }
    }
}
