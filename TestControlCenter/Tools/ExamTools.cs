using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TestControlCenter.Infrastructure;
using TestControlCenter.Windows;
using TestControlCenterDomain;
using Point = System.Drawing.Point;

namespace TestControlCenter.Tools
{
    public class ExamTools
    {
        public List<TestItemQuestionClueRecord> Records { get; set; }

        public ExamTools()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (Directory.Exists(StaticValues.ExamTempDir))
            {
                Directory.Delete(StaticValues.ExamTempDir, true);
            }

            Directory.CreateDirectory(StaticValues.ExamTempDir);

            Records = new List<TestItemQuestionClueRecord>();
        }

        public static Rectangle GetScreenshopRectange()
        {
            return Screen.GetBounds(Point.Empty);
        }

        public async Task<TestItemQuestionClueRecord> TakeRecord(bool isFinal = false, string keyName = null, bool isMouse = false)
        {
            try
            {
                return await TakeRecordAction(isFinal, keyName, isMouse);
            }
            catch (Exception)
            {
                NotificationsHelper.Error("امکان ثبت پاسخ وجود ندارد.", "خطا");
                return null;
            }
        }

        private async Task<TestItemQuestionClueRecord> TakeRecordAction(bool isFinal, string keyName, bool isMouse)
        {
            if (GlobalValues.Question == null)
            {
                return null;
            }

            var record = new TestItemQuestionClueRecord
            {
                X = Cursor.Position.X,
                Y = Cursor.Position.Y,
                Order = Records.Count(x => x.TestItemQuestion.Id == GlobalValues.Question.Id) + 1,
                TestItemQuestion = GlobalValues.Question,
                TestItemQuestionId = GlobalValues.Question.Id,
                RecordType = isFinal ? TestItemQuestionClueRecordType.Answer : TestItemQuestionClueRecordType.Normal,
                DateTime = DateTime.Now,
                IsImportant = GlobalValues.TestMarker.IsImportant(GlobalValues.Question, keyName, isMouse),
                Data = isFinal ? GlobalValues.TestMarker.GetInformation(GlobalValues.Question) : null
            };

            var isAltWindow = GlobalValues.ExamWindow.Visibility == Visibility.Hidden;
            void ChangeExamWindowVisibility(Visibility visibility)
            {
                if (GlobalValues.ExamWindow != null && !isAltWindow)
                {
                    GlobalValues.ExamWindow.Dispatcher.Invoke(() =>
                    {
                        GlobalValues.ExamWindow.Visibility = visibility;
                    });
                }
            }
            void ChangeExamAltWindowVisibility(double height)
            {
                if (GlobalValues.ExamAltWindow != null && isAltWindow)
                {
                    GlobalValues.ExamAltWindow.Dispatcher.Invoke(() =>
                    {
                        GlobalValues.ExamAltWindow.Height = height;
                    });
                }
            }

            if (isFinal)
            {
                ChangeExamWindowVisibility(Visibility.Hidden);
                ChangeExamAltWindowVisibility(0);
                GlobalValues.Question.IsAnswered = true;

                await Task.Delay(1000);
            }

            using (var bitmap = Screenshot())
            {
                record.ImageAddress = GlobalTools.GetNewFileName(StaticValues.ExamTempDir, ".retadata");
                bitmap.Save(record.ImageAddress, ImageFormat.Png);
            }

            if (isFinal)
            {
                ChangeExamWindowVisibility(Visibility.Visible);
                ChangeExamAltWindowVisibility(40);
            }

            Records.Add(record);

            return record;
        }

        public static Bitmap Screenshot()
        {
            Cursor.Hide();

            if (!Directory.Exists(StaticValues.ExamTempDir))
            {
                Directory.CreateDirectory(StaticValues.ExamTempDir);
            }

            var bounds = GetScreenshopRectange();
            var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            Cursor.Show();

            return bitmap;
        }

        public static async Task TakeFinalRecord()
        {
            var rect = GetScreenshopRectange();
            var flashWindow = new FlashWindow
            {
                Width = rect.Width,
                Height = rect.Height,
                Left = rect.Left,
                Top = rect.Top
            };
            flashWindow.ShowDialog();
            await EventHookConfig.ExamTools.TakeRecord(true);
        }
    }
}
