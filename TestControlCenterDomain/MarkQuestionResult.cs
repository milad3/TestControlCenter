using System;

namespace TestControlCenterDomain
{
    public enum MarkQuestionResultType
    {
        Correct,
        Wrong,
        MaybeCorrect,
        Unknown
    }

    public class MarkQuestionResult
    {
        public DateTime DateTime { get; set; }

        public MarkQuestionResultType ResultType { get; set; }

        public TestMarkAnswer TestMarkAnswer { get; set; }
    }
}
