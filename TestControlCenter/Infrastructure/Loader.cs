using System;
using System.IO;
using System.IO.Compression;
using TestControlCenterDomain;

namespace TestControlCenter.Infrastructure
{
    public class Loader
    {
        static string parentDirectory = $"{StaticValues.RootPath}\\Temp\\Test";

        //public static TestItem LoadTestItemFromFile(string address)
        //{
        //    CleanUp();

        //    using (var zip = ZipFile.OpenRead(address))
        //    {
        //        zip.ExtractToDirectory(parentDirectory);
        //    }


        //}

        private static void CleanUp()
        {
            if (Directory.Exists(parentDirectory))
            {
                Directory.Delete(parentDirectory, true);
            }

            Directory.CreateDirectory(parentDirectory);
        }
    }
}
