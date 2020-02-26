using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TestControlCenterDomain;
using TestControlCenter.Infrastructure;
using TestControlCenter.Models;
using System.IO;

namespace TestControlCenter.Services
{
    public class ViewModelsHelper
    {
        public static async Task<MainWindowViewModel> GetMainWindowViewModel()
        {
            var model = new MainWindowViewModel
            {
                Tests = new ObservableCollection<TestItemViewModel>()
            };

            await model.LoadTestItems();

            return model;
        }

        internal static async Task<TestMarkingViewModel> GetExamDetailsViewModel(TestMark testMark)
        {
            using (var db = new DataService())
            {
                return await db.GetTestMarkAsync(testMark);
            }
        }

        public static AnswerRecordsViewerViewModel GetAnswerRecordsViewerViewModel(IEnumerable<TestItemQuestionClueRecord> records)
        {
            var model = new AnswerRecordsViewerViewModel
            {
                Images = new List<ImageData>()
            };

            foreach (var record in records)
            {
                if(!File.Exists(record.ImageAddress))
                {
                    continue;
                }

                model.Images.Add(new ImageData { Address = record.ImageAddress });
            }

            return model;
        }

        public static TestStartViewModel GetTestStartViewModel(List<MftStudent> students, TestItem testItem)
        {
            var mapper = new Mapper(Mappings.TestItemMapperConfiguration);
            var model = new TestStartViewModel
            {
                TestItem = mapper.Map<TestItem, TestItemViewModel>(testItem),
                Students = students
            };

            return model;
        }

        public static async Task<TestsListViewModel> GetTestsListViewModel(bool showFinilized, bool showMarked, bool showUnmarked, TestsListSortOrder sortOrder)
        {
            var model = new TestsListViewModel
            {
                ShowFinilized = showFinilized,
                ShowMarked = showMarked,
                ShowUnmarked = showUnmarked,
                SortOrder = sortOrder
            };

            using (var db = new DataService())
            {
                model.Tests = await db.SearchTestMarks(model.ShowFinilized, model.ShowUnmarked, model.ShowMarked, model.Filter, model.SortOrder);
            }

            return model;
        }

        public static ExamViewModel GetExamViewModel(TestItem testItem, MftStudent student, System.Windows.Rect workArea)
        {
            var ExamViewModel = new ExamViewModel
            {
                Student = student,
                TestItem = testItem,
                Width = 420,
                Height = 650,
                MinWidth = 370,
                MinHeight = 420,
                WorkArea = workArea
            };
            ExamViewModel.SetLocationToBottomRight();

            return ExamViewModel;
        }
    }
}
