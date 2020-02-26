﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using TestControlCenter.Infrastructure;
using TestControlCenterDomain;

namespace TestControlCenter.Tools
{
    public class GlobalTools
    {
        public static string GetNewFileName(string dir, string ext)
        {
            var result = $"{dir}\\{Guid.NewGuid()}{ext}";
            do
            {
                if(!File.Exists(result))
                {
                    break;
                }

                result = $"{dir}\\{new Guid()}{ext}";
            } while (true);

            return result;
        }

        public static string GetPersianDate(DateTime date, bool onlyDate = true)
        {
            var cal = new PersianCalendar();

            if (onlyDate)
            {
                return $"{cal.GetYear(date)}/{cal.GetMonth(date)}/{cal.GetDayOfMonth(date)}";
            }

            return $"{cal.GetYear(date)}/{cal.GetMonth(date)}/{cal.GetDayOfMonth(date)} {cal.GetHour(date).ToString("00")}:{cal.GetMinute(date).ToString("00")}";
        }

        public static DateTime GetDate(string persianDate)
        {
            if(string.IsNullOrEmpty(persianDate))
            {
                return default;
            }

            var allParts = persianDate.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var parts = allParts[0].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var year = Convert.ToInt32(parts[0]);
            var month = Convert.ToInt32(parts[1]);
            var day = Convert.ToInt32(parts[2]);

            var hour = 0;
            var min = 0;

            if(allParts.Length > 1)
            {
                var timeParts = allParts[1].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                hour = Convert.ToInt32(timeParts[0]);
                min = Convert.ToInt32(timeParts[1]);
            }

            return new DateTime(year, month, day, hour, min, 0);
        }

        public static string GetImagesDir(TestItem testItem)
        {
            return $"{StaticValues.RootPath}\\Files\\Tests\\{testItem.Key}\\";
        }

        public static ITestMarker LoadTestMarkerProcessor(TestItem testItem, string imagesDir)
        {
            var dll = testItem.ProcessorAddress;

            if (string.IsNullOrEmpty(dll))
            {
                return null;
            }

            if (!File.Exists(dll))
            {
                return null;
            }

            var loaded = Assembly.LoadFrom(dll);

            var types = loaded.GetTypes();
            var type = types.First(x => x.GetInterfaces().Any(i => i.Name == nameof(ITestMarker)));

            var testMarker = Activator.CreateInstance(type) as ITestMarker;
            var processor = new ProcessorTools();
            var advancedProcessor = new ProcessorToolsAdvanced();
            testMarker.Configure(processor, advancedProcessor, imagesDir);

            return testMarker;
        }
    }
}