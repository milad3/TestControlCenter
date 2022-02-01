using TestControlCenter.Data;
using TestControlCenterDomain;

namespace TestControlCenter.Services
{
    public class MessageLogger : IMessageLogger
    {
        public static bool CreateDatabaseIfNeeded()
        {
            using (var db = new LogDatabaseContext())
            {
                return db.Database.EnsureCreated();
            }
        }

        public LogMessage LogMessage(LogMessage message)
        {
            using (var db = new LogDatabaseContext())
            {
                db.Logs.Add(message);

                db.SaveChanges();
            }

            return message;
        }
    }
}
