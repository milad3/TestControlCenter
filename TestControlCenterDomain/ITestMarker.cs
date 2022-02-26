using System;

namespace TestControlCenterDomain
{
    public interface ITestMarker
    {
        void Configure(IProcessorTools processor, IProcessorTools advancedProcessor, TestBasicInformation testBasicInformation);

        MarkQuestionResult Mark(TestMarkAnswer answer, double tolerance);

        bool IsImportant(TestItemQuestion question, string keyName, bool isMouse);

        string GetInformation(TestItemQuestion question);
    }
}
