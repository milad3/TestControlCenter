using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TestControlCenterDomain;

namespace Generator
{
    class Program
    {
        const string DIR = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\Generator\bin\Debug\temp";

        static void Main(string[] args)
        {
            CleanUp();

            var coverImage = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\Generator\bin\Debug\excel.jpg";
            var dll = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\Generator\bin\Debug\ExcelTest.dll";
            var images = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\Generator\bin\Debug\images";

            var test = new TestItem
            {
                CourseId = "5c2232dbc0f947040d00010d",
                CourseName = "Microsoft Office Excel",
                DateTime = new DateTime(2019, 12, 15),
                DepartmentId = "5c2232dbc0f947040d00010d",
                DepartmentName = "ICDL",
                GroupId = "5c2232dbc0f947040d00010d",
                GroupName = "حسابداری",
                HasNegativeScore = false,
                Key = "5c2232dbc0f947040d00010d",
                MaxScore = 20,
                MinScore = 0,
                PassScore = 12,
                Requirement = "برای انجام این آزمون باید آزمون مقدماتی را پشت سر گذاشته باشید",
                ShortDescription = "آزمون متوسطه ICDL نرم افزار Excel",
                Software = "Microsoft Office",
                SoftwareVersion = "2019",
                SyllabusRef = "4.4.2.2",
                TagsCommaSeperated = "اسکل,MS Excel,eksel,exsel,ekcel",
                TestLevel = TestLevel.Level2,
                TimeAllowedInMin = 60,
                Title = "آزمون عملی Excel 2019",
                Version = "1",
                TotalQuestionsCount = 25,
                IsActive = true,
                Questions = new List<TestItemQuestion>
                {
                    new TestItemQuestion
                    {
                        Question = "دو فایل Excel را همزمان باز کنید و به شکل افقی در کنار یکدیگر نمایش دهید.",
                        Hint = "این کار را از طریق نرم افزار Excel انجام دهید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 1,
                        Score = 1,
                        TextAnswer = "بر روی آیکن نرم افزار دو بار دابل کلیک کرده و دو فایل را باز کنید سپس از زبانه View گروه Window گزینه Arrange All را انتخاب کنید و از پنجره باز شده Horizontal را انتخاب کرده Ok را کلیک کنید.",
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = false,
                                Order = 1,
                                ImageAddress = "1c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 3,
                                ImageAddress = "1f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "فایل Book1 را از  Desktop باز کنید و با فرمت Pdf  و با همان نام در Desktop ذخیره کنید.",
                        Level = TestQuestionLevel.Level1,
                        Order = 2,
                        Score = .75,
                        TextAnswer = "بر روی فایل دابل کلیک کرده تا باز شود سپس از منوی File گزینهsave As را انتخاب کنید سپس Browse را کلیک کنید از منوی Save As type گزینه Pdf را انتخاب و دکمه Save را کلیک کنید.",
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "2c"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Zoom بزرگنماییsheet1 را به 185% تغییر دهید.",
                        Level = TestQuestionLevel.Level1,
                        Order = 3,
                        Score = .5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = false,
                                Order = 1,
                                ImageAddress = "3c"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 2,
                                ImageAddress = "3f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل insert  در Sheet1 دو ستون خالی بین ستونهای نام خانوادگی و فیزیک و دو ردیف خالی زیر ردیف 16 ایجاد کنید.",
                        Level = TestQuestionLevel.Level1,
                        Order = 4,
                        Score = 1,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "4f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book1 و Sheet1 دو محدوده همجوار C1:H11 و K12:P19 را به طور همزمان انتخاب کرده و فقط فرمت این محدوده ها را پاک کنید.",
                        Level = TestQuestionLevel.Level1,
                        Order = 5,
                        Score = 1,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "5f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book8 و Sheet1 با استفاده ار تابع در سلول B2 سلول A2 را تا دو رقم اعشار گرد کنید و فرمول را تا سلول B8 تعمیم دهید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 6,
                        Score = 1.5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "6c"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "6f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book2 یک sheetجدید با نام Example ایجاد کنید آنرا به ابتدای sheet ها منتقل کرده در سلولA1 عدد 100در سلول A10 عدد 500  را تایپ کرده سپس sheet با نام Example را در فایل جدید کپی کنید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 7,
                        Score = 3,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = false,
                                Order = 1,
                                ImageAddress = "7c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 2,
                                ImageAddress = "7c2"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 3,
                                ImageAddress = "7c3"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 4,
                                ImageAddress = "7f1",
                                IsForFinalAnswer = true
                            }
                            ,
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 4,
                                ImageAddress = "7f2",
                                IsForFinalAnswer = true
                            }
                            ,
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 4,
                                ImageAddress = "7f3",
                                IsForFinalAnswer = true
                            }
                        }
                    }
                }
            };
            var json = JsonConvert.SerializeObject(test);
            File.WriteAllText($"{DIR}\\.manifest", json);

            File.Copy(coverImage, $"{DIR}\\.cover");

            File.Copy(dll, $"{DIR}\\.processor");

            var files = Directory.GetFiles(images);
            foreach (var file in files)
            {
                File.Copy(file, $"{DIR}\\{Path.GetFileName(file)}");
            }

            var parent = Directory.GetParent(DIR);

            var path = $"{parent}\\excel_test.reta";
            File.Delete(path);

            ZipFile.CreateFromDirectory(DIR, path);
        }

        private static void CleanUp()
        {
            if(Directory.Exists(DIR))
            {
                Directory.Delete(DIR, true);
            }

            Directory.CreateDirectory(DIR);
        }
    }
}
