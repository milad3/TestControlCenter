using System;
using System.Collections.Generic;

namespace TestControlCenterDomain
{
    public enum TestLevel
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4
    }

    public enum TestQuestionLevel
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4
    }

    public class TestItem
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Title { get; set; }

        public string Software { get; set; }

        public string Processes { get; set; }

        public bool TerminateAfterExam { get; set; }

        public string SoftwareVersion { get; set; }

        public string ShortDescription { get; set; }

        public string Version { get; set; }

        public string Requirement { get; set; }

        public string CoverImageAddress { get; set; }

        public TestLevel TestLevel { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime AddDateTime { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPinned { get; set; } = false;

        public string DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string GroupId { get; set; }

        public string GroupName { get; set; }

        public string CourseId { get; set; }

        public string CourseName { get; set; }

        public bool HasNegativeScore { get; set; }

        public int TotalQuestionsCount { get; set; }

        public int TimeAllowedInMin { get; set; }

        public string SyllabusRef { get; set; }

        public ICollection<TestItemQuestion> Questions { get; set; }

        public int MaxScore { get; set; }

        public int MinScore { get; set; }

        public int PassScore { get; set; }

        public int TestsDoneCounter { get; set; }

        public string TagsCommaSeperated { get; set; }

        public string Temp { get; set; }

        public string ProcessorAddress { get; set; }
    }

    public class TestItemQuestion
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public int TestItemId { get; set; }

        public TestItem TestItem { get; set; }

        public string Question { get; set; }

        public string Hint { get; set; }

        public TestQuestionLevel Level { get; set; }

        public double Score { get; set; }

        public string TextAnswer { get; set; }

        public ICollection<TestItemQuestionClue> Clues { get; set; }
    }

    public class TestItemQuestionClue
    {
        public long Id { get; set; }

        public string ImageAddress { get; set; }

        public bool Forced { get; set; }

        public double Score { get; set; }

        public int Order { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int TestItemQuestionId { get; set; }

        public TestItemQuestion TestItemQuestion { get; set; }

        public bool IsForFinalAnswer { get; set; } = false;
    }

    public class Student
    {
        public int Id { get; set; }

        public string IdInServer { get; set; }

        public string Name { get; set; }

        public string NationalCode { get; set; }

        public string Mobile { get; set; }

        public string Token { get; set; }
    }

    public enum TestItemQuestionClueRecordType
    {
        Normal,
        Answer
    }

    public class TestItemQuestionClueRecord
    {
        public int Id { get; set; }

        public int TestItemQuestionId { get; set; }

        public TestItemQuestion TestItemQuestion { get; set; }

        public int Order { get; set; }

        public TestItemQuestionClueRecordType RecordType { get; set; }

        public string ImageAddress { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsImportant { get; set; }

        public string Data { get; set; }
    }

    public class TestMarkAnswer
    {
        public long Id { get; set; }

        public double? PrivateScore { get; set; }

        public double PublicScore { get; set; }

        public ICollection<TestItemQuestionClueRecord> Records { get; set; }

        public int TestItemQuestionId { get; set; }

        public TestItemQuestion TestItemQuestion { get; set; }
    }

    public class TestMark
    {
        public int Id { get; set; }

        public Student Student { get; set; }

        public TestItem TestItem { get; set; }

        public int TestItemId { get; set; }

        public bool IsMarked { get; set; } = false;

        public bool IsSynced { get; set; } = false;

        public DateTime StartDateTime { get; set; }

        public DateTime FinishDateTime { get; set; }

        public DateTime? SyncDateTime { get; set; }

        public DateTime? FinilizeDateTime { get; set; }

        public bool IsFinal { get; set; } = false;

        public double? Score { get; set; }

        public ICollection<TestMarkAnswer> TestMarkAnswers { get; set; }
    }

    public class ImageSearchResultItem
    {
        public int X { get; set; }

        public int Y { get; set; }

        public double Tolerance { get; set; }
    }

    public class TestBasicInformation
    {
        public DateTime TestStartDateTime { get; set; }

        public string ImagesDirectory { get; set; }

        public string FilesDirectory { get; set; }

        public dynamic OtherInfo { get; set; }

        public string Temp { get; set; }
    }
}
