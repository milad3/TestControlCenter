using System.Collections.Generic;
using System.IO;

namespace TestControlCenter.Tools
{
    internal class AppCleaner
    {
        public static List<string> GarbageFiles { get; set; } = new List<string>();

        public static void CleanUp()
        {
            var marked = new List<string>();
            foreach (var item in GarbageFiles)
            {
                try
                {
                    if (File.Exists(item))
                    {
                        File.Delete(item);
                    }
                    marked.Add(item);
                }
                catch (System.Exception)
                {
                }
            }
            foreach (var item in marked)
            {
                GarbageFiles.Remove(item);
            }
        }
    }
}
