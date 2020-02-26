using System.IO;
using System.Reflection;

namespace TestControlCenter.Infrastructure
{
    public static class StaticValues
    {
        public static string RootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string ExamTempDir = $"{RootPath}\\temp\\exam";
    }
}
