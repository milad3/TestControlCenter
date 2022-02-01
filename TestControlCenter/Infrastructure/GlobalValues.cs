using TestControlCenter.Models;
using TestControlCenter.Windows;
using TestControlCenterDomain;

namespace TestControlCenter.Infrastructure
{
    public class GlobalValues
    {
        public static bool ExamIsRunning = false;

        public static Student Student = null;

        public static TestItem Test = null;

        public static TestItemQuestionViewModel Question { get; set; }

        public static ExamWindow ExamWindow { get; set; }

        public static ExamAltWindow ExamAltWindow { get; set; }

        public static ITestMarker TestMarker { get; set; }
    }
}
