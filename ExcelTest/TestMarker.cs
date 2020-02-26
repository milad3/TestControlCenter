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

        public void Configure(IProcessorTools processorTools, IProcessorTools advancedProcessorTools, string imagesDir)
        {
            processor = processorTools;
            processorAdvanced = advancedProcessorTools;

            this.imagesDir = imagesDir;
        }

        private (int result, List<Rectangle> locations) CheckImages(string clueImage, string answerImage, double tolerance)
        {
            using (var smallerImage = new Bitmap($"{imagesDir}{clueImage}.mftdata"))
            {
                using (var biggerImage = new Bitmap(answerImage))
                {
                    var results = processor.SearchBitmap(smallerImage, biggerImage, tolerance);
                    if (results == null || results.Count == 0)
                    {
                        return (0, results);
                    }

                    return (1, results);
                }
            }
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

                    if(results?.Count > 0)
                    {
                        var up = results.OrderBy(x => x.Top).First();
                        var down = results.OrderByDescending(x => x.Top).First();
                        if (up.Top + 150 > down.Top)
                        {
                            answer.PublicScore = 0;
                        }
                    }
                    //CluesMarking(answer, tolerance, result);
                    //if (answer.PrivateScore != 100)
                    //{
                    //    answer.PublicScore = 0;
                    //}
                    break;
                case 2:
                    //CluesMarking(answer, tolerance, result);

                    if(File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Book1.pdf"))
                    {
                        answer.PublicScore = answer.TestItemQuestion.Score;
                    }
                    break;
                case 3:
                    AnswerMarking(answer, tolerance, result);
                    //CluesMarking(answer, tolerance, result);
                    break;
                case 4:
                    AnswerMarking(answer, tolerance, result);
                    break;
                case 5:
                    AnswerMarking(answer, tolerance, result);
                    break;
                case 6:
                    AnswerMarking(answer, tolerance, result);
                    //CluesMarking(answer, tolerance, result);

                    if(answer.PrivateScore != 100)
                    {
                        answer.PublicScore = 0;
                    }
                    break;
                case 7:
                    AnswerMarking(answer, tolerance, result);
                    //CluesMarking(answer, tolerance, result);

                    break;
            }

            return result;
        }

        private List<Rectangle> AnswerMarking(TestMarkAnswer answer, double tolerance, MarkQuestionResult result)
        {
            var locations = new List<Rectangle>();

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

        private List<Rectangle> CluesMarking(TestMarkAnswer answer, double tolerance, MarkQuestionResult result)
        {
            var locations = new List<Rectangle>();

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
            if(result.TestMarkAnswer.PrivateScore > 100)
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
                    if(isMouse)
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

                    if(processes == null || processes.Length == 0)
                    {
                        return "false";
                    }

                    var wanted = processes.First();

                    if(wanted.HasExited)
                    {
                        return "-1";
                    }
                    var windows = EnumerateProcessWindowHandles(wanted.Id);
                    return windows.Count() == 14 ? "true" : "false";

                default:
                    return null;
            }
        }
    }
}
