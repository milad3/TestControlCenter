using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneralTests
{
    public class CompareNode
    {
        public Int64 First { get; set; }

        public Int64 Second { get; set; }

        public int Index { get; set; }
    }

    [TestClass]
    public class FilesComparingTests
    {
        const int BYTES_TO_READ = sizeof(Int64);

        public static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }

        public static List<CompareNode> Diff { get; set; } = new List<CompareNode>();
        public static int GetFilesDiff(FileInfo first, FileInfo second)
        {
            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return 0;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            var counter = 0;

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    {
                        counter++;
                        Diff.Add(new CompareNode
                        {
                            First = BitConverter.ToInt64(one, 0),
                            Second = BitConverter.ToInt64(two, 0),
                            Index = i
                        });
                    }
                }
            }

            return counter;
        }

        [TestMethod]
        public void TestFileContentCompararing()
        {
            var file1 = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\GeneralTests\Asset\Book1_t3.xlsx";
            var file2 = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\GeneralTests\Asset\Book1_t2.xlsx";

            var result = FilesAreEqual(new FileInfo(file1), new FileInfo(file2));

            Assert.AreEqual(false, result);

            file1 = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\GeneralTests\Asset\Book1_t3.xlsx";
            file2 = @"D:\Development\ManagedProjects\MFT\Desktop\TestControlCenter\GeneralTests\Asset\Book1_t2.xlsx";

            var result2 = GetFilesDiff(new FileInfo(file1), new FileInfo(file2));


            Debug.WriteLine(result2);
            Debug.WriteLine("*********\n\n");

            foreach (var item in Diff)
            {
                Debug.WriteLine($"{item.First}::{item.Second}({item.Index}) - ");
            }

        }
    }
}
