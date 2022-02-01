using System.IO;
using System.Reflection;
using TestControlCenter.Properties;

namespace TestControlCenter.Infrastructure
{
    public enum UserType
    {
        None,
        Operator,
        Student
    }

    public static class StaticValues
    {
        public static string RootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string ExamTempDir = $"{RootPath}\\temp\\exam";

        public static string InfoUrl = $"{Settings.Default.ServerBaseUrl}/getinfo.php";

        public static string LoginUrl = $"{Settings.Default.ServerBaseUrl}/login.php";

        public static string StudentLoginUrl = $"{Settings.Default.ServerBaseUrl}/studentlogin.php";

        public static string GetExamsUrl = $"{Settings.Default.ServerBaseUrl}/getexams.php";

        public static string GetExamUrl = $"{Settings.Default.ServerBaseUrl}/getexam.php";

        public static string GetStudentsUrl = $"{Settings.Default.ServerBaseUrl}/getstudents.php";

        public static string SetTestStartedUrl = $"{Settings.Default.ServerBaseUrl}/examstart.php";

        public static string SyncTestUrl = $"{Settings.Default.ServerBaseUrl}/syncdata.php";

        public static UserType LoggedInUserType = UserType.None;
    }
}
