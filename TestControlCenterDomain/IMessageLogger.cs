using System.Threading.Tasks;

namespace TestControlCenterDomain
{
    public interface IMessageLogger
    {
        LogMessage LogMessage(LogMessage message);

        Task<LogMessage> LogMessageAsync(LogMessage message);
    }
}
