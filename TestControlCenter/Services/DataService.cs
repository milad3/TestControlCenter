using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestControlCenter.Data;
using TestControlCenterDomain;
using TestControlCenter.Models;
using System.IO;
using TestControlCenter.Tools;
using TestControlCenter.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.ObjectModel;

namespace TestControlCenter.Services
{
    public class DataService : IDisposable
    {
        public DatabaseContext Database { get; set; }

        public static bool CreateDatabaseIfNeeded()
        {
            using (var db = new DatabaseContext())
            {
                return db.Database.EnsureCreated();
            }
        }

        public DataService()
        {
            Database = new DatabaseContext();
        }

        public async Task<List<TestItem>> GetTestsAsync(string filter, TestsSortOrder sortOrder)
        {
            var query = GetTestsQuery(filter, sortOrder);
            return await query.ToListAsync();
        }

        public async Task<TestMarkingViewModel> GetTestMarkAsync(TestMark testMark)
        {
            var wanted = await Database.TestMarks.Include(x => x.Student).Include(x => x.TestItem).Include(x => x.TestItem.Questions).Include("TestMarkAnswers.TestItemQuestion.Clues").Include("TestMarkAnswers.Records").FirstAsync(x => x.Id == testMark.Id);
            var model = new TestMarkingViewModel(wanted, new MessageLogger())
            {
                Answers = new ObservableCollection<TestMarkAnswer>()
            };
            foreach (var item in model.TestMark.TestMarkAnswers.OrderBy(x => x.TestItemQuestion.Order))
            {
                model.Answers.Add(item);
            }

            return model;
        }

        public TestItem GetTestItem(string key)
        {
            return Database.TestItems.Include(x => x.Questions).FirstOrDefault(x => x.Key == key);
        }

        public async Task<List<TestMark>> SearchTestMarks(bool showFinilized, bool showUnmarked, bool showMarked, string filter, TestsListSortOrder sortOrder)
        {
            var max = 20;

            IOrderedQueryable<TestMark> query;

            if (string.IsNullOrEmpty(filter))
            {
                query = Database.TestMarks.AsQueryable().Take(max)
                                          .OrderByDescending(x => x.Id);
            }
            else
            {
                var filterInLower = filter.ToLower();
                query = Database.TestMarks.AsQueryable().Where(
                x =>
                x.Student.Name.ToLower().Contains(filterInLower)
                || x.TestItem.Title.Contains(filter)
                || x.TestItem.Software.ToLower().Contains(filterInLower)
                || x.TestItem.TagsCommaSeperated.ToLower().Contains(filterInLower)).Take(max).OrderByDescending(x => x.Id);
            }

            if (showFinilized && !showMarked && !showUnmarked)
            {
                query = query.Where(x => x.IsFinal).Take(max).OrderByDescending(x => x.Id);
            }
            else if (showFinilized && !showMarked && showUnmarked)
            {
                query = query.Where(x => x.IsFinal || !x.IsMarked).Take(max).OrderByDescending(x => x.Id);
            }
            else if (showFinilized && showMarked && !showUnmarked)
            {
                query = query.Where(x => x.IsFinal || x.IsMarked).Take(max).OrderByDescending(x => x.Id);
            }
            else if (showFinilized && showMarked && showUnmarked)
            {
                query = query.Where(x => x.IsFinal || x.IsMarked || !x.IsMarked).Take(max).OrderByDescending(x => x.Id);
            }
            else if (showUnmarked && !showFinilized && !showMarked)
            {
                query = query.Where(x => !x.IsMarked).Take(max).OrderByDescending(x => x.Id);
            }
            else if (showUnmarked && !showFinilized && showMarked)
            {
                query = query.Where(x => !x.IsMarked || x.IsMarked).Take(max).OrderByDescending(x => x.Id);
            }
            else if (showMarked && !showFinilized && !showUnmarked)
            {
                query = query.Where(x => x.IsMarked).Take(max).OrderByDescending(x => x.Id);
            }

            switch (sortOrder)
            {
                case TestsListSortOrder.Oldest:
                    query = query.OrderBy(x => x.Id);
                    break;
                default:
                    query = query.OrderByDescending(x => x.Id);
                    break;
            }

            return await query.Include(x => x.Student).Include(x => x.TestItem).ToListAsync();
        }

        public async Task UpdateExam(TestMark testMark)
        {
            var wanted = Database.TestMarks.FirstOrDefault(x => x.Id == testMark.Id);

            wanted.Score = testMark.Score;

            wanted.IsFinal = testMark.IsFinal;

            wanted.IsMarked = testMark.IsMarked;

            wanted.TestMarkAnswers = testMark.TestMarkAnswers;

            if (testMark.IsSynced)
            {
                wanted.IsSynced = testMark.IsSynced;
                wanted.SyncDateTime = DateTime.Now;
            }

            await Database.SaveChangesAsync();
        }

        public async Task<TestItem> GetTestAsync(int id)
        {
            return await Database.TestItems.Include("Questions.Clues").FirstOrDefaultAsync(x => x.Id == id);
        }

        private IQueryable<TestItem> GetTestsQuery(string filter, TestsSortOrder sortOrder)
        {
            IOrderedQueryable<TestItem> query;

            if (string.IsNullOrEmpty(filter))
            {
                query = Database.TestItems.Include(x => x.Questions).Take(20).OrderByDescending(x => x.IsPinned);
            }
            else
            {
                var filterInLower = filter.ToLower();
                query = Database.TestItems.Include(x => x.Questions).Where(
                x =>
                x.Title.ToLower().Contains(filterInLower)
                || x.Key.Contains(filter)
                || x.Software.ToLower().Contains(filterInLower)
                || x.CourseName.ToLower().Contains(filterInLower)
                || x.DepartmentName.ToLower().Contains(filterInLower)
                || x.GroupName.ToLower().Contains(filterInLower)
                || x.TagsCommaSeperated.ToLower().Contains(filterInLower)).Take(20).OrderByDescending(x => x.IsPinned);
            }

            switch (sortOrder)
            {
                case TestsSortOrder.OldestAdded:
                    query = query.ThenByDescending(x => x.AddDateTime);
                    break;
                case TestsSortOrder.NewsetAdded:
                    query = query.ThenBy(x => x.AddDateTime);
                    break;
                case TestsSortOrder.Oldest:
                    query = query.ThenBy(x => x.DateTime);
                    break;
                case TestsSortOrder.Newest:
                    query = query.ThenByDescending(x => x.DateTime);
                    break;
                case TestsSortOrder.Level:
                    query = query.ThenByDescending(x => x.TestLevel);
                    break;
                case TestsSortOrder.TestsDoneCounter:
                    query = query.ThenByDescending(x => x.TestsDoneCounter);
                    break;
                default:
                    query = query.ThenByDescending(x => x.AddDateTime);
                    break;
            }

            return query;
        }

        public async Task<bool> IsThereUnmarkedTests()
        {
            return await Database.TestMarks.AsQueryable().AnyAsync(x => !x.IsMarked);
        }

        public async Task<List<TestMark>> GetUnSyncedTestMarksAsync()
        {
            return await Database.TestMarks.Include(x => x.TestItem).Include(x => x.Student).AsQueryable().Where(x => !x.IsSynced && x.IsFinal).ToListAsync();
        }

        public async Task<TestMark> SaveExam(List<TestItemQuestionClueRecord> records, DateTime startDateTime)
        {
            var testMark = new TestMark
            {
                Student = GlobalValues.Student,
                TestItemId = GlobalValues.Test.Id,
                TestMarkAnswers = new List<TestMarkAnswer>(),
                FinishDateTime = DateTime.Now,
                StartDateTime = startDateTime
            };

            var dir = $"{StaticValues.RootPath}\\data\\{GlobalValues.Student.Token}\\{Guid.NewGuid()}\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var forCleanUp = new List<string>();
            foreach (var items in records.GroupBy(x => x.TestItemQuestionId))
            {
                var answer = new TestMarkAnswer
                {
                    PrivateScore = 0,
                    PublicScore = 0,
                    Records = new List<TestItemQuestionClueRecord>()
                };
                foreach (var item in items)
                {
                    var newAddress = GlobalTools.GetNewFileName(dir, ".retadata");
                    File.Copy(item.ImageAddress, newAddress);
                    forCleanUp.Add(item.ImageAddress);
                    item.ImageAddress = newAddress;
                    item.TestItemQuestion = null;
                    answer.Records.Add(item);
                }
                if (items.Count() > 0)
                {
                    answer.TestItemQuestionId = items.First().TestItemQuestionId;
                }

                testMark.TestMarkAnswers.Add(answer);
            }

            foreach (var item in forCleanUp)
            {
                File.Delete(item);
            }

            Database.TestMarks.Add(testMark);

            await Database.SaveChangesAsync();

            return testMark;
        }

        public bool TestIsThere(string key)
        {
            return Database.TestItems.Any(x => x.Key == key);
        }

        public TestItem GetLatestTestItem()
        {
            return Database.TestItems.AsQueryable().OrderByDescending(x => x.DateTime).FirstOrDefault();
        }

        public async Task SaveTestItems(params TestItem[] testItems)
        {
            await AddOrUpdateTestItems(testItems);
        }

        public async Task SaveTestItems(IEnumerable<TestItem> testItems)
        {
            await AddOrUpdateTestItems(testItems);
        }

        private async Task AddOrUpdateTestItems(IEnumerable<TestItem> testItems)
        {
            foreach (var item in testItems)
            {
                if (Database.TestItems.Any(x => x.Key == item.Key))
                {
                    var wanted = Database.TestItems.Include("Questions.Clues").First(x => x.Key == item.Key);

                    if (File.Exists(wanted.CoverImageAddress))
                    {
                        if (string.Compare(wanted.CoverImageAddress, item.CoverImageAddress, true) != 0)
                        {
                            AppCleaner.GarbageFiles.Add(wanted.CoverImageAddress);
                        }
                    }

                    if (File.Exists(wanted.ProcessorAddress))
                    {
                        if (string.Compare(wanted.ProcessorAddress, item.ProcessorAddress, true) != 0)
                        {
                            AppCleaner.GarbageFiles.Add(wanted.ProcessorAddress);
                        }
                    }

                    foreach (var q in wanted.Questions)
                    {
                        q.Clues.Clear();
                    }
                    Database.SaveChanges();

                    wanted.CourseId = item.CourseId;
                    wanted.CourseName = item.CourseName;
                    wanted.CoverImageAddress = item.CoverImageAddress;
                    wanted.DateTime = item.DateTime;
                    wanted.DepartmentId = item.DepartmentId;
                    wanted.DepartmentName = item.DepartmentName;
                    wanted.GroupId = item.GroupId;
                    wanted.GroupName = item.GroupName;
                    wanted.HasNegativeScore = item.HasNegativeScore;
                    wanted.IsActive = item.IsActive;
                    wanted.MaxScore = item.MaxScore;
                    wanted.MinScore = item.MinScore;
                    wanted.PassScore = item.PassScore;
                    wanted.ProcessorAddress = item.ProcessorAddress;

                    foreach (var q in item.Questions)
                    {
                        var wantedQuestion = wanted.Questions.AsQueryable().Include(x => x.Clues).FirstOrDefault(x => x.Order == q.Order);
                        if (wantedQuestion != null)
                        {
                            wantedQuestion.Hint = q.Hint;
                            wantedQuestion.Level = q.Level;
                            wantedQuestion.Question = q.Question;
                            wantedQuestion.Score = q.Score;
                            wantedQuestion.TextAnswer = q.TextAnswer;
                            wantedQuestion.Clues = q.Clues;
                        }
                        else
                        {
                            wanted.Questions.Add(q);
                        }
                    }

                    //var toBeRemoved = Database.Questions.AsQueryable().Where(x => x.TestItemId == wanted.Id);

                    //Database.Questions.RemoveRange(toBeRemoved);
                    //wanted.Questions = item.Questions; TODO : resolve to be removed

                    wanted.Requirement = item.Requirement;
                    wanted.ShortDescription = item.ShortDescription;
                    wanted.Software = item.Software;
                    wanted.SoftwareVersion = item.SoftwareVersion;
                    wanted.SyllabusRef = item.SyllabusRef;
                    wanted.TagsCommaSeperated = item.TagsCommaSeperated;
                    wanted.Temp = item.Temp;
                    wanted.TestLevel = item.TestLevel;
                    wanted.TimeAllowedInMin = item.TimeAllowedInMin;
                    wanted.Title = item.Title;
                    wanted.TotalQuestionsCount = item.TotalQuestionsCount;
                    wanted.Version = item.Version;
                }
                else
                {
                    Database.TestItems.Add(item);
                }
            }

            await Database.SaveChangesAsync();
        }

        public async Task TogglePinForTestItemAndSave(TestItem item)
        {
            var wanted = await Database.TestItems.AsQueryable().FirstAsync(x => x.Id == item.Id);
            wanted.IsPinned = !wanted.IsPinned;

            await Database.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Database.Dispose();
            }
        }
    }
}
