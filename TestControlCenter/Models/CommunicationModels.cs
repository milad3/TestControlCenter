using TestControlCenterDomain;

namespace TestControlCenter.Models
{
    public enum LoginResultType
    {
        Failed,
        Success,
        Locked,
        NotStarted,
        CommunicationProblem
    }

    public class CommunicationResult
    {
        public LoginResultType Type { get; set; }

        public string Message { get; set; }

        public string Response { get; set; }
    }

    public class AuthenticationItem
    {
        public string Username { get; set; }

        public string Token { get; set; }

        public string Key { get; set; }

        public Student Student { get; set; }
    }

    public class ExamItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string File { get; set; }
    }
}
