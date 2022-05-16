using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using TestControlCenter.Infrastructure;
using TestControlCenter.Services;
using TestControlCenterDomain;

namespace TestControlCenter.Tools
{
    public class ImportExportHelper
    {
        string importDir = $"{StaticValues.RootPath}\\temp\\testimport\\";
        readonly string exportDir = $"{StaticValues.RootPath}\\temp\\testexport\\";
        readonly string coverDir = $"{StaticValues.RootPath}\\Files\\Images\\";
        readonly string dllDir = $"{StaticValues.RootPath}\\Files\\Processors\\";
        readonly string imagesDir = $"{StaticValues.RootPath}\\Files\\Tests\\";
        readonly string filesDir = $"{StaticValues.RootPath}\\Files\\Tests\\Raw\\";

        public async Task Export(TestItem testItem, string path)
        {
            TestItem wanted;
            using (var db = new DataService())
            {
                wanted = await db.GetTestAsync(testItem.Id);
            }

            CleanUp(exportDir);

            var fileName = Path.GetFileName(path);
            var parentDir = $"{exportDir}{Path.GetFileNameWithoutExtension(fileName)}\\";
            Directory.CreateDirectory(parentDir);

            var data = JsonConvert.SerializeObject(wanted, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            File.WriteAllText($"{parentDir}.manifest", data);

            if(!string.IsNullOrEmpty(wanted.CoverImageAddress))
            {
                if(File.Exists(wanted.CoverImageAddress))
                {
                    File.Copy(wanted.CoverImageAddress, $"{parentDir}.cover");
                }
            }

            if(!string.IsNullOrEmpty(wanted.ProcessorAddress))
            {
                if(File.Exists(wanted.ProcessorAddress))
                {
                    File.Copy(wanted.ProcessorAddress, $"{parentDir}.processor");
                }
            }

            var images = Directory.GetFiles($"{imagesDir}{testItem.Key}", "*.retadata");
            foreach (var image in images)
            {
                if(!File.Exists(image))
                {
                    continue;
                }

                File.Copy(image, $"{parentDir}{Path.GetFileName(image)}");
            }

            var files = Directory.GetFiles($"{filesDir}{testItem.Key}", "*.*");
            var dir = $"{parentDir}\\Files\\";
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                File.Copy(file, $"{parentDir}\\Files\\{Path.GetFileName(file)}");
            }

            ZipFile.CreateFromDirectory(exportDir, path);

            CleanUp(exportDir);
        }

        public async Task<TestItem> Import(string fileName, string key = "")
        {
            CleanUp(importDir);

            var parentDir = importDir;
            ZipFile.ExtractToDirectory(fileName, parentDir);
            importDir = $"{importDir}\\";

            var testItem = GetTestItem();
            if(!string.IsNullOrEmpty(key))
            {
                testItem.Key = key;
            }
            var coverAddress = SaveCoverImage();
            var dll = SaveProcessor();
            testItem.CoverImageAddress = coverAddress;
            testItem.ProcessorAddress = dll;
            var images = Directory.GetFiles(importDir, "*.retadata");
            SaveAllImages(testItem, images);

            var files = Directory.GetFiles($"{importDir}\\Files", "*.*");
            SaveAllFiles(testItem, files);

            testItem.AddDateTime = DateTime.Now;

            using (var db = new DataService())
            {
                await db.SaveTestItems(testItem);
            }

            CleanUp(importDir);

            return testItem;
        }

        private void SaveAllFiles(TestItem testItem, string[] files)
        {
            var wantedDir = $"{filesDir}{testItem.Key}";

            if (!Directory.Exists(wantedDir))
            {
                Directory.CreateDirectory(wantedDir);
            }

            foreach (var file in files)
            {
                File.Copy(file, $"{wantedDir}\\{Path.GetFileName(file)}", true);
            }
        }

        private void SaveAllImages(TestItem testItem, IEnumerable<string> images)
        {
            var wantedDir = $"{imagesDir}{testItem.Key}\\";

            if(!Directory.Exists(wantedDir))
            {
                Directory.CreateDirectory(wantedDir);
            }

            foreach (var image in images)
            {
                File.Copy(image, $"{wantedDir}{Path.GetFileName(image)}", true);
            }
        }

        private TestItem GetTestItem()
        {
            var content = File.ReadAllText($"{importDir}\\.manifest");
            return JsonConvert.DeserializeObject<TestItem>(content);
        }

        private string SaveProcessor()
        {
            if (!Directory.Exists(dllDir))
            {
                Directory.CreateDirectory(dllDir);
            }

            var newFileName = GlobalTools.GetNewFileName(dllDir, ".dll");

            var dll = $"{importDir}.processor";
            if(!File.Exists(dll))
            {
                return null;
            }

            File.Copy(dll, newFileName);

            return newFileName;
        }

        private string SaveCoverImage()
        {
            var newFileName = GlobalTools.GetNewFileName(coverDir, ".png");

            var cover = $"{importDir}.cover";
            if(!File.Exists(cover))
            {
                return null;
            }

            if(!Directory.Exists(coverDir))
            {
                Directory.CreateDirectory(coverDir);
            }

            File.Copy(cover, newFileName);

            return newFileName;
        }

        private static void CleanUp(string wantedDir)
        {
            if(Directory.Exists(wantedDir))
            {
                Directory.Delete(wantedDir, true);
            }

            Directory.CreateDirectory(wantedDir);
        }
    }
}
