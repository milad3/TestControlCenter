using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TestControlCenterDomain;

namespace ExcelTest
{
    public class TestMarker : ITestMarker
    {
        private IProcessorTools processor;
        private IProcessorTools processorAdvanced;
        private string imagesDir;
        private TestBasicInformation testBasicInformation;

        public void Configure(IProcessorTools processorTools,
            IProcessorTools advancedProcessorTools,
            TestBasicInformation testBasicInformation)
        {
            processor = processorTools;
            processorAdvanced = advancedProcessorTools;

            imagesDir = testBasicInformation.ImagesDirectory;
            this.testBasicInformation = testBasicInformation;
        }

        private (int result, List<ImageSearchResultItem> locations) CheckImages(string clueImage, string answerImage, double tolerance)
        {
            var results = processorAdvanced.SearchImage($"{imagesDir}{clueImage}.retadata", answerImage, tolerance);
            if (results.Any(x => x.Tolerance > tolerance))
            {
                return (1, results);
            }

            return (0, results);
        }

        private MarkQuestionResult MarkQuestion(TestMarkAnswer answer, double tolerance)
        {
            var result = new MarkQuestionResult
            {
                DateTime = DateTime.Now,
                ResultType = MarkQuestionResultType.Correct,
                TestMarkAnswer = answer
            };

            switch (answer.TestItemQuestion.Order)
            {
                case 1:
                    var results = AnswerMarking(answer, tolerance, result);

                    if (results?.Count > 0)
                    {
                        var up = results.OrderBy(x => x.Y).First();
                        if (!results.Any(x => x.Y > up.Y + 350))
                        {
                            answer.PublicScore = 0;
                        }
                    }
                    if (answer.Records?.Any(x => x.Data == "true") == false)
                    {
                        CluesMarking(answer, tolerance * .75, result);
                        if (answer.PrivateScore != 100)
                        {
                            answer.PublicScore = 0;
                        }
                    }
                    break;
                case 2:
                    CluesMarking(answer, tolerance, result);

                    if (answer.Records?.Any(x => x.Data == "true") == true)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }

                    if (answer.PrivateScore != 100)
                    {
                        answer.PublicScore = 0;
                    }
                    break;
                case 3:
                    AnswerMarking(answer, tolerance, result);
                    CluesMarking(answer, tolerance, result);
                    break;
                case 4:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 5:
                    AnswerMarking(answer, tolerance, result);

                    if (answer.PublicScore == 0)
                    {
                        CluesMarking(answer, tolerance, result);
                        if (answer.PrivateScore == 100)
                        {
                            answer.PublicScore = answer.TestItemQuestion.Score;
                        }
                    }
                    break;
                case 6:
                    AnswerMarking(answer, tolerance, result);
                    CluesMarking(answer, tolerance, result);

                    if (answer.PrivateScore != 100)
                    {
                        answer.PublicScore = 0;
                    }
                    break;
                case 7:
                    AnswerMarking(answer, tolerance, result);
                    CluesMarking(answer, tolerance, result);

                    break;
                case 8:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 9:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 10:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 11:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 12:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 13:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 14:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 15:
                    AnswerMarking(answer, tolerance, result);
                    CluesMarking(answer, tolerance, result);
                    break;
                case 16:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 17:
                    AnswerMarking(answer, tolerance, result);
                    break;
                case 19:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 20:
                    CluesMarking(answer, tolerance * .9, result);
                    if (answer.PrivateScore > 66)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 21:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 22:
                    AnswerMarking(answer, tolerance, result);
                    break;
                case 23:
                    AnswerMarking(answer, tolerance, result);
                    break;
                case 24:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 25:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 26:
                    CluesMarking(answer, tolerance, result);
                    if (answer.PrivateScore == 100)
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
            }

            return result;
        }

        private List<ImageSearchResultItem> AnswerMarking(TestMarkAnswer answer, double tolerance, MarkQuestionResult result)
        {
            var locations = new List<ImageSearchResultItem>();

            var finalAnswer = answer.Records.Where(x => x.RecordType == TestItemQuestionClueRecordType.Answer).OrderByDescending(x => x.Id).FirstOrDefault();
            if (finalAnswer != null)
            {
                var hitCounter = 0;
                var finalClues = answer.TestItemQuestion.Clues.Where(x => x.IsForFinalAnswer);
                var cluesToCheck = finalClues.GroupBy(x => x.Order);
                foreach (var cluesGroup in cluesToCheck)
                {
                    foreach (var clue in cluesGroup.OrderBy(x => x.Order))
                    {
                        var check = CheckImages(clue.ImageAddress, finalAnswer.ImageAddress, tolerance);
                        foreach (var item in check.locations)
                        {
                            locations.Add(item);
                        }
                        hitCounter += check.result;
                    }
                }
                if (hitCounter == finalClues.Count())
                {
                    result.TestMarkAnswer.PublicScore = result.TestMarkAnswer.TestItemQuestion.Score;
                }
            }

            return locations;
        }

        private List<ImageSearchResultItem> CluesMarking(TestMarkAnswer answer, double tolerance, MarkQuestionResult result)
        {
            var locations = new List<ImageSearchResultItem>();

            var otherClues = answer.TestItemQuestion.Clues.Where(x => !x.IsForFinalAnswer).GroupBy(x => x.Order);
            var otherAnswers = answer.Records.Where(x => x.RecordType != TestItemQuestionClueRecordType.Answer);
            var cluesCounter = 0;
            foreach (var clueGroup in otherClues)
            {
                var allCluesInOneImageCounter = 0;
                foreach (var item in otherAnswers)
                {
                    foreach (var clue in clueGroup)
                    {
                        var check = CheckImages(clue.ImageAddress, item.ImageAddress, tolerance);
                        foreach (var location in check.locations)
                        {
                            locations.Add(location);
                        }
                        allCluesInOneImageCounter += check.result;
                    }
                    if (clueGroup.Count() == allCluesInOneImageCounter)
                    {
                        allCluesInOneImageCounter++;
                    }
                }
                cluesCounter += allCluesInOneImageCounter;
            }
            var allOtherCluesCount = otherClues.Count();
            var privateScore = result.TestMarkAnswer.PublicScore > 0 ? 100 : 0;
            result.TestMarkAnswer.PrivateScore = allOtherCluesCount == 0 ? privateScore : (cluesCounter * 100) / allOtherCluesCount;
            if (result.TestMarkAnswer.PrivateScore > 100)
            {
                result.TestMarkAnswer.PrivateScore = 100;
            }

            return locations;
        }

        public MarkQuestionResult Mark(TestMarkAnswer answer, double tolerance)
        {
            return MarkQuestion(answer, tolerance);
        }

        public bool IsImportant(TestItemQuestion question, string keyName, bool isMouse)
        {
            switch (question.Order)
            {
                case 1:
                    if (isMouse)
                    {
                        var processes = Process.GetProcessesByName("excel");
                        if (processes == null || processes.Length == 0)
                        {
                            return false;
                        }

                        var wanted = processes.First();

                        if (wanted.HasExited)
                        {
                            return false;
                        }
                        var windows = EnumerateProcessWindowHandles(wanted.Id);
                        return windows.Count() == 14;
                    }

                    return false;
                default:
                    return false;
            }
        }

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }

        public string GetInformation(TestItemQuestion question)
        {
            switch (question.Order)
            {
                case 1:
                    var processes = Process.GetProcessesByName("excel");

                    if (processes == null || processes.Length == 0)
                    {
                        return "false";
                    }

                    var wanted = processes.First();

                    if (wanted.HasExited)
                    {
                        return "-1";
                    }
                    var windows = EnumerateProcessWindowHandles(wanted.Id);

                    return windows.Count() >= 14 ? "true" : "false";

                case 2:
                    var filePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Book1.pdf";
                    if (!File.Exists(filePath))
                    {
                        return "false";
                    }
                    var lastSaved = File.GetLastWriteTime(filePath);
                    return lastSaved > testBasicInformation.TestStartDateTime ? "true" : "false";
                default:
                    return null;
            }
        }
    }
}
