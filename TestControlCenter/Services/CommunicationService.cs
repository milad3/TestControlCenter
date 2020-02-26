using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestControlCenterDomain;

namespace TestControlCenter.Services
{
    public class CommunicationService
    {
        public static async Task<List<MftStudent>> GetStudents(TestItem testItem)
        {
            // TODO implement


            var result = new List<MftStudent>();

            result.Add(new MftStudent
            {
                Mobile = "09108710767",
                Name = "میلاد حسین پناهی",
                NationalCode = "37200502147",
                IdInServer = "jhkjasd54565",
                Token = "56as4d6a5saslkdjhak"
            });

            result.Add(new MftStudent
            {
                Mobile = "09123582789",
                Name = "رضا طبرزدی",
                NationalCode = "37522556644",
                IdInServer = "xdkaasd545sa5",
                Token = "d6a5saslkdjhakasd"
            });

            result.Add(new MftStudent
            {
                Mobile = "09121113355",
                Name = "محمد محمدی",
                NationalCode = "37522556644",
                IdInServer = "xdkaasd545sa5"
            });

            result.Add(new MftStudent
            {
                Mobile = "0911552244",
                Name = "کریم کریمی",
                NationalCode = "37522556644",
                IdInServer = "xdkaasd545sa5"
            });

            result.Add(new MftStudent
            {
                Mobile = "0911552244",
                Name = "رسول رسولی",
                NationalCode = "37522556644",
                IdInServer = "xdkaasd545sa5"
            });

            return result;
        }

        public static async Task<List<TestItem>> GetNewTestItems()
        {
            var result = new List<TestItem>();

            using (var db = new DataService())
            {
                var latest = db.GetLatestTestItem();

                var newOnes = await GetNewTestItemsFromServer(latest);

                result.AddRange(newOnes);

                await db.SaveTestItems(newOnes);
            }

            return result;
        }

        private static async Task<List<TestItem>> GetNewTestItemsFromServer(TestItem latest)
        {
            // TODO remove these things!!



            var result = new List<TestItem>();

            result.Add(new TestItem
            {
                Title = "آزمون عملی Excel 2019",
                DepartmentName = "ICT",
                GroupName = "ICDL/MOS",
                CourseName = "ICDL",
                TestLevel = TestLevel.Level3,
                HasNegativeScore = false,
                TotalQuestionsCount = 25,
                SyllabusRef = "4.1.1.2",
                TimeAllowedInMin = 60,
                IsActive = true,
                Software = "Microsoft Office 2019",
                AddDateTime = DateTime.Now,
                DateTime = new DateTime(2020, 1, 5),
                MaxScore = 20,
                MinScore = 0,
                PassScore = 12,
                ShortDescription = "آزمون عملی نرم افزار Excel برای دریافت پایان نامه مجموعه Office",
                SoftwareVersion = "2019",
                Key = "asd4565465asdas",
                CoverImageAddress = "\\Files\\Images\\t1.jpg",
                TestsDoneCounter = 25,
                Questions = new List<TestItemQuestion>
                {
                    new TestItemQuestion
                    {
                        Order = 1,
                        Question = "دو فایل Excel را همزمان باز کنید و به شکل افقی در کنار یکدیگر نمایش دهید.",
                        Hint = "از هر روشی که مایل باشید می توانید استفاده کنید.",
                        Score = .5,
                        Level = TestQuestionLevel.Level1,
                        TextAnswer = "بر روی آیکن نرم افزار دو بار دابل کلیک کرده و دو فایل را باز کنید سپس از زبانه View گروه Window گزینه Arrange All را انتخاب کنید و از پنجره باز شده Horizontal را انتخاب کرده Ok را کلیک کنید."
                    },
                    new TestItemQuestion
                    {
                        Order = 2,
                        Question = "در فایل Book1 و Sheet1 دو محدوده همجوار C1:H11 و K12:P19 را به طور همزمان انتخاب کرده و فقط فرمت این محدوده ها را پاک کنید.",
                        Hint = "از هر روشی که مایل باشید می توانید استفاده کنید.",
                        Score = 1,
                        Level = TestQuestionLevel.Level2,
                        TextAnswer = "در Name Box آدرس محدوده­ها را به شکل K12:P19 C1:H11, تایپ کرده کلید Enter را فشار دهید سپس از زبانه Home گروه Editing گزینه Clear و سپس Clear Format را انتخاب کنید."
                    },
                    new TestItemQuestion
                    {
                        Order = 3,
                        Question = "در فایل Book8 و Sheet1 با استفاده ار تابع در سلول B2 سلول A2 را تا دو رقم اعشار گرد کنید و فرمول را تا سلول B8 تعمیم دهید.",
                        Score = 1,
                        Level = TestQuestionLevel.Level3,
                        TextAnswer = "در سلول B2 مساوی را تایپ کنید و تابع Round(A2,2) را تایپ کنید و سپس سلول را تا B8 با Autofill تعمیم دهید."
                    },
                    new TestItemQuestion
                    {
                        Order = 4,
                        Question = "در فایل Book8 و Sheet1 با استفاده ار تابع در سلول B2 سلول A2 را تا دو رقم اعشار گرد کنید و فرمول را تا سلول B8 تعمیم دهید.",
                        Score = 1,
                        Level = TestQuestionLevel.Level3,
                        TextAnswer = "در سلول B2 مساوی را تایپ کنید و تابع Round(A2,2) را تایپ کنید و سپس سلول را تا B8 با Autofill تعمیم دهید."
                    },
                    new TestItemQuestion
                    {
                        Order = 5,
                        Question = "در فایل Book8 و Sheet1 با استفاده ار تابع در سلول B2 سلول A2 را تا دو رقم اعشار گرد کنید و فرمول را تا سلول B8 تعمیم دهید.",
                        Score = 1,
                        Level = TestQuestionLevel.Level3,
                        TextAnswer = "در سلول B2 مساوی را تایپ کنید و تابع Round(A2,2) را تایپ کنید و سپس سلول را تا B8 با Autofill تعمیم دهید."
                    },
                    new TestItemQuestion
                    {
                        Order = 6,
                        Question = "در فایل Book8 و Sheet1 با استفاده ار تابع در سلول B2 سلول A2 را تا دو رقم اعشار گرد کنید و فرمول را تا سلول B8 تعمیم دهید.",
                        Score = 1,
                        Level = TestQuestionLevel.Level3,
                        TextAnswer = "در سلول B2 مساوی را تایپ کنید و تابع Round(A2,2) را تایپ کنید و سپس سلول را تا B8 با Autofill تعمیم دهید."
                    },
                    new TestItemQuestion
                    {
                        Order = 7,
                        Question = "در فایل Book8 و Sheet1 با استفاده ار تابع در سلول B2 سلول A2 را تا دو رقم اعشار گرد کنید و فرمول را تا سلول B8 تعمیم دهید.",
                        Score = 1,
                        Level = TestQuestionLevel.Level3,
                        TextAnswer = "در سلول B2 مساوی را تایپ کنید و تابع Round(A2,2) را تایپ کنید و سپس سلول را تا B8 با Autofill تعمیم دهید."
                    }
                }
            });

            var refTest = result.First();
            var d = new TestItem
            {
                AddDateTime = refTest.AddDateTime,
                CourseId = refTest.CourseId,
                GroupName = refTest.GroupName,
                DateTime = refTest.DateTime,
                DepartmentId = refTest.DepartmentId,
                GroupId = refTest.GroupId,
                DepartmentName = refTest.GroupName,
                HasNegativeScore = refTest.HasNegativeScore,
                IsActive = refTest.IsActive,
                IsPinned = refTest.IsPinned,
                MaxScore = 100,
                MinScore = 0,
                PassScore = 60,
                Requirement = refTest.Requirement,
                SyllabusRef = refTest.SyllabusRef,
                Version = refTest.Version,
                Title = "اصول فتوشاپ",
                CoverImageAddress = refTest.CoverImageAddress.Replace("t1", "t2"),
                Software = "Photoshop",
                SoftwareVersion = "2019",
                ShortDescription = "آزمون مقدماتی و اصول فتوشاپ",
                TestLevel = TestLevel.Level1,
                TimeAllowedInMin = 30,
                TotalQuestionsCount = 10,
                CourseName = "گرافیک",
                Key = "dvsdd4565465asdas",
                TestsDoneCounter = 15
            };
            result.Add(d);

            var d2 = new TestItem
            {
                AddDateTime = refTest.AddDateTime,
                CourseId = refTest.CourseId,
                DepartmentName = refTest.DepartmentName,
                DateTime = refTest.DateTime,
                DepartmentId = refTest.DepartmentId,
                GroupId = refTest.GroupId,
                GroupName = refTest.GroupName,
                HasNegativeScore = refTest.HasNegativeScore,
                IsActive = refTest.IsActive,
                IsPinned = refTest.IsPinned,
                MaxScore = 100,
                MinScore = 0,
                PassScore = 60,
                Requirement = refTest.Requirement,
                SyllabusRef = refTest.SyllabusRef,
                Version = refTest.Version,
                Title = "Word",
                CoverImageAddress = refTest.CoverImageAddress.Replace("t1", "t3"),
                Software = "Microsoft Office",
                SoftwareVersion = "2019",
                ShortDescription = "آزمون مرحله نهایی MS Word با انجام این آزمون شما موفق به دریافت گواهینامه انجام امور اقتصادی خواهید شد پس حتما تلاش خود را بکنید!",
                TestLevel = TestLevel.Level4,
                TimeAllowedInMin = 90,
                TotalQuestionsCount = 30,
                CourseName = "ICDL",
                TestsDoneCounter = 4,
                Key = "dvsdd45bbbasdas"
            };
            result.Add(d2);

            var d3 = new TestItem
            {
                AddDateTime = refTest.AddDateTime,
                CourseId = refTest.CourseId,
                DepartmentName = refTest.DepartmentName,
                DateTime = refTest.DateTime,
                DepartmentId = refTest.DepartmentId,
                GroupId = refTest.GroupId,
                GroupName = refTest.GroupName,
                HasNegativeScore = refTest.HasNegativeScore,
                IsActive = refTest.IsActive,
                IsPinned = refTest.IsPinned,
                MaxScore = 100,
                MinScore = 0,
                PassScore = 60,
                Requirement = refTest.Requirement,
                SyllabusRef = refTest.SyllabusRef,
                Version = refTest.Version,
                Title = " آزمون تستی با نام بسیار بسیار بسیار بسیار طولانی",
                CoverImageAddress = refTest.CoverImageAddress.Replace("t1", "t5"),
                Software = "Microsoft Office",
                SoftwareVersion = "2019",
                ShortDescription = "آزمون مرحله نهایی MS Word با انجام این آزمون شما موفق به دریافت گواهینامه انجام امور اقتصادی خواهید شد پس حتما تلاش خود را بکنید!",
                TestLevel = TestLevel.Level4,
                TimeAllowedInMin = 90,
                TotalQuestionsCount = 30,
                CourseName = "ICDL",
                Key = "xxsdd4565465asdas"
            };
            result.Add(d3);

            return result;
        }
    }
}
