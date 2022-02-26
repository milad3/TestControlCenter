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
        const string DIR = @"C:\Users\hosse\OneDrive\Desktop\Exams";

        static void Main(string[] args)
        {
            CleanUp();

            var coverImage = @"D:\Dev\MFT\TestControlCenter\ExcelTest\cover.jpg";
            var dll = @"D:\Dev\MFT\TestControlCenter\ExcelTest\bin\Debug\ExcelTest.dll";
            var images = @"D:\Dev\MFT\TestControlCenter\ExcelTest\Images";

            var test = new TestItem
            {
                CourseId = "5c2232dbc0f947040d00010d",
                CourseName = "Microsoft Office Excel",
                Processes = "excel",
                TerminateAfterExam = true,
                DateTime = new DateTime(2019, 12, 15),
                DepartmentId = "5c2232dbc0f947040d00010d",
                DepartmentName = "ICDL",
                GroupId = "5c2232dbc0f947040d00010d",
                GroupName = "حسابداری",
                HasNegativeScore = false,
                Key = "600d52d1e0ccbc220b9e7e1a",
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
                Version = "1.501",
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
                                Order = 2,
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
                                ImageAddress = "4c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "4c2"
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
                                ImageAddress = "5c1",
                                IsForFinalAnswer = true
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "5c2",
                                IsForFinalAnswer = true
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "5c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "5c2"
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
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Autofill و در Sheet1 سری اعداد زوج را از محدوده A1:A2 تا عدد 100 در جهت عمودی بدون فرمت سلولها Autofill کنید.از سلول D1 از عدد 1 در جهت افقی با فاصله عددی 52.5  تا 52.5 تا تا سلول M1 Autofill کنید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 8,
                        Score = 2,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "8c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 2,
                                ImageAddress = "8c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "درsheet1 از  فایل Freeze ستونهای A,B را ثابت کنید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 9,
                        Score = 1,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "9c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 2,
                                ImageAddress = "9c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book3 و Sheet1 سلول B8 را به سلول A5 به صورت مطلق وابسته کنید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 10,
                        Score = 2,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "10c"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book3 و Sheet2  آدرس سلولهای موجود در  در سلول D1 را به صورت مطلق نمایش دهید. و جمع H1:H5 را درH6 بدست آورده و جواب و فرمول H6 را در K10 کپی کنید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 11,
                        Score = 3,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "11c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 2,
                                ImageAddress = "11c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در Sheet1 از فایل Book4   در سلول E1 حاصل جمع  A1 وB1 را بر حاصل ضرب C1در D1 تقسیم کنید و تا سلول ,E7 Autofill کنید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 12,
                        Score = 2,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "12c"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در Book5 و Sheet1 در محدوده D8:D13 افزایش حقوق پایه با ضریب را بدست اورید و سپس در E8:E13 جمع کل حقوق پایه  با افزایش حقوق پایه با ضریب را بدست آورید.",
                        Level = TestQuestionLevel.Level4,
                        Order = 13,
                        Score = 4,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "13c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 2,
                                ImageAddress = "13c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Error و در Sheet1 در  سلولهای C6  و G3 بدون تغییر در  محتویات این سلولها و تغییر فرمول خطاهای سلولهای را تصحیح کنید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 14,
                        Score = 2,
                         Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "14c"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book6 و در Sheet1 جمع کل فاکتور را با توجه به تعداد و مبلغ هر کالا در سلول F18 بدست آورید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 15,
                        Score = 2,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "15f",
                                IsForFinalAnswer = true
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 2,
                                ImageAddress = "15c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "15c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "فایل If و Sheet1 در سلول j5 تابع If تایپ کنید اگر معدل E5:I5 بزرگتر مساوی 15 بود خوب و در غیر این صورت بد ظاهر شود و فرمول را تا سلول J10 با Autofill کپی کنید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 16,
                        Score = 2.5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "16c"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل If و sheet1جمع کل فاکتور را با توجه به تعداد و مبلغ هر کالا در سلول E11 بدست آورید و در سلول E12 تابع IF تایپ کنید اگر E11 بزرگتر مساوی 300000 بود 20 درصد تخفیف و در غیر این صورت 10 درصد تخفیف محاسبه و مبلغ کل را بدست اورید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 17,
                        Score = 3.5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "17f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Print و sheet1 محدوده A1:L50 را برای چاپ همراه با نام ستونها و خطوط سلولها برای جاپ آماده کنید ردیف 1 را در بالا و ستونهای B:C را درسمت چپ تمام صفحات چاپ کنید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 18,
                        Score = 3,

                    },
                    new TestItemQuestion
                    {
                        Question = "در فایلBook7  و Sheet1 در سلول D2 تابع IF تایپ کنید اگر سلول C2 بزرگتر مساوی 15 بود C2 با F2 جمع شود و در غیر این صورت C2 با F3 جمع شود و فرمول را تا D9 Autofill کنید.",
                        Level = TestQuestionLevel.Level4,
                        Order = 19,
                        Score = 4,
                         Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "19c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "19c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Functions و Sheet1 در سلول E12  با استفاده از تابع بزرگترین عدد محدوده E5:I10 در سلول E13  با استفاده از تابع کوچکترین عدد محدوده E5:I10 و در سلول E14  با استفاده از تابع تعداد سلولهای محدوده C4:I10 که محتویات آنها عدد است را بشمارید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 20,
                        Score = 3.5,

                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book9 و Sheet1 محدوده B1:B10 را به فرمت Currency و Symbol ریال Persian(Iran) با دو رقم بعد از ممیز تغییر دهید. سلول F1 و F2 را با فرمت تاریخ 14-Mar نمایش دهید. و محدوده D1:D10 را به فرمتی تغییر دهید که اعداد در قالب متن (Text) در آن دیده شوند.",
                        Level = TestQuestionLevel.Level2,
                        Order = 21,
                        Score = 1.5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "21c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "21c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book10 و در Sheet1 رنگ زمینه A2:C2 را سبز Bold و سایز فونت را 14 کنید و A3:A6را رنگ فونت آبی ایتالیک و Underline و سایز 15 کنید.فرمت A3 را به E7،E1 وF4 کپی کنید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 22,
                        Score = 1,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "22f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book11 و Sheet1 محدوده B2:G5 را به یک سلول تبدیل کنید سلولها را در هم ادغام کنید. عبارت ICDL exam را از نظر افقی و عمودی در وسط سلول تایپ کنید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 23,
                        Score = 1.5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "23f",
                                IsForFinalAnswer = true
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Book12 و Sheet1  برای محدوده C2: H10 کادر (Border) قرار دهید Inside دو خط موازی قرمز و Outline دو خط موازی آبی باشد. و محتویات سلول K14 را در داخل سلول در سطرهای زیر هم نمایش دهید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 24,
                        Score = 1,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "24c1"
                            },
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "24c2"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Chart و Sheet1 چارت موجود در Sheet1 را به 3-D clustered column تغییر دهید Chart Area را سبز و Plot Area  را ابی کنید و مقادیر محور Y را بر روی سریها نمایش دهید.",
                        Level = TestQuestionLevel.Level2,
                        Order = 25,
                        Score = 1.5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "25c"
                            }
                        }
                    },
                    new TestItemQuestion
                    {
                        Question = "در فایل Climate و Sheet1 نام نمودار را درجه حرارت نام محور X را ماه و نام محور Y را دما قرار دهید. و ماکزیمم مقیاس محور Y را به 40 و فاصله بین نقاط اصلی را 10 قرار دهید. Legend را در پایین Chart Area قرار دهید.",
                        Level = TestQuestionLevel.Level3,
                        Order = 26,
                        Score = 2.5,
                        Clues = new List<TestItemQuestionClue>
                        {
                            new TestItemQuestionClue
                            {
                                Forced = true,
                                Order = 1,
                                ImageAddress = "26c"
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
            if (Directory.Exists(DIR))
            {
                Directory.Delete(DIR, true);
            }

            Directory.CreateDirectory(DIR);
        }
    }
}
