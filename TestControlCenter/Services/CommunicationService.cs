using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TestControlCenter.Infrastructure;
using TestControlCenter.Models;
using TestControlCenter.Tools;
using TestControlCenterDomain;

namespace TestControlCenter.Services
{
    public class CommunicationService
    {
        public static async Task<CommunicationResult> GetInfo()
        {
            var result = new CommunicationResult()
            {
                Type = LoginResultType.NotStarted
            };

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(StaticValues.InfoUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            result.Type = LoginResultType.Failed;
                        }
                        else
                        {
                            result.Type = LoginResultType.CommunicationProblem;
                        }
                    }
                    else
                    {
                        result.Type = LoginResultType.Success;
                        result.Response = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Type = LoginResultType.CommunicationProblem;
                result.Message = ex.Message;
            }

            return result;
        }

        public static async Task<TestItem> GetTestItem(string key)
        {
            var result = new TestItem();

            var data = new[]
                    {
                        new KeyValuePair<string, string>("key", key)
                    };

            ExamItem exam = null;
            try
            {
                using (var server = new ServerClient())
                {
                    using (var content = new FormUrlEncodedContent(data))
                    {
                        var response = await server.HttpClient.PostAsync(StaticValues.GetExamUrl, content);

                        var rawData = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(rawData))
                        {
                            exam = JsonConvert.DeserializeObject<ExamItem>(rawData);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            if (exam == null)
            {
                return null;
            }

            using (var db = new DataService())
            {
                using (var client = new WebClient())
                {
                    var item = exam;

                    if (string.IsNullOrEmpty(item.File))
                    {
                        return null;
                    }

                    try
                    {
                        string fileName;
                        var dir = $"{StaticValues.RootPath}\\temp\\";

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        do
                        {
                            fileName = $"{dir}{Guid.NewGuid()}";
                        } while (File.Exists(fileName));

                        client.DownloadFile(item.File, fileName);

                        var importer = new ImportExportHelper();
                        result = await importer.Import(fileName, item.Id);
                    }
                    catch (Exception)
                    {
                        NotificationsHelper.Error($"خطا در دانلود یا ثبت {item.Title}", "خطا");
                        return null;
                    }
                }
            }

            return result;
        }

        public static async Task<CommunicationResult> Login(string username, string password)
        {
            var result = new CommunicationResult()
            {
                Type = LoginResultType.NotStarted
            };

            try
            {
                using (var client = new HttpClient())
                {
                    var data = new[]
                    {
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password)
                    };

                    using (var content = new FormUrlEncodedContent(data))
                    {
                        var response = await client.PostAsync(StaticValues.LoginUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.Type = LoginResultType.Failed;
                            }
                            else
                            {
                                result.Type = LoginResultType.CommunicationProblem;
                            }
                        }
                        else
                        {
                            result.Type = LoginResultType.Success;
                            result.Response = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Type = LoginResultType.CommunicationProblem;
                result.Message = ex.Message;
            }

            return result;
        }

        public static async Task<CommunicationResult> StudentLogin(string username, string password)
        {
            var result = new CommunicationResult()
            {
                Type = LoginResultType.NotStarted
            };

            try
            {
                using (var client = new HttpClient())
                {
                    var data = new[]
                    {
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password)
                    };

                    using (var content = new FormUrlEncodedContent(data))
                    {
                        var response = await client.PostAsync(StaticValues.StudentLoginUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.Type = LoginResultType.Failed;
                            }
                            else
                            {
                                result.Type = LoginResultType.CommunicationProblem;
                            }
                        }
                        else
                        {
                            result.Type = LoginResultType.Success;
                            result.Response = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Type = LoginResultType.CommunicationProblem;
                result.Message = ex.Message;
            }

            return result;
        }

        public static async Task<List<Student>> GetStudents(TestItem testItem)
        {
            var result = new List<Student>();

            using (var server = new ServerClient())
            {
                var formData = new[]
                    {
                        new KeyValuePair<string, string>("key", testItem.Key)
                    };

                using (var content = new FormUrlEncodedContent(formData))
                {
                    var response = await server.HttpClient.PostAsync(StaticValues.GetStudentsUrl, content);

                    var data = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(data))
                    {
                        result = JsonConvert.DeserializeObject<List<Student>>(data);
                    }
                }

            }

            return result;
        }

        public static async Task<List<TestItem>> GetNewTestItems()
        {
            return await GetNewTestItemsFromServer();
        }

        public static async Task<bool> StartTest(TestItem testItem, Student student)
        {
            try
            {
                using (var server = new ServerClient())
                {
                    var formData = new[]
                        {
                            new KeyValuePair<string, string>("key", testItem.Key),
                            new KeyValuePair<string, string>("token", student.Token),
                            new KeyValuePair<string, string>("member", student.IdInServer)
                        };

                    using (var content = new FormUrlEncodedContent(formData))
                    {
                        var response = await server.HttpClient.PostAsync(StaticValues.SetTestStartedUrl, content);

                        response.EnsureSuccessStatusCode();

                        var data = await response.Content.ReadAsStringAsync();
                        if (data != "ok")
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> SyncTest(TestMark testMark)
        {
            using (var server = new ServerClient())
            {
                var formData = new[]
                    {
                        new KeyValuePair<string, string>("key", testMark.TestItem.Key),
                        new KeyValuePair<string, string>("token", testMark.Student.Token),
                        new KeyValuePair<string, string>("member", testMark.Student.IdInServer),
                        new KeyValuePair<string, string>("score", testMark.Score.ToString()),
                        new KeyValuePair<string, string>("finished", GlobalTools.GetIranTimeZoneNow().ToString())
                    };

                using (var content = new FormUrlEncodedContent(formData))
                {
                    var response = await server.HttpClient.PostAsync(StaticValues.SyncTestUrl, content);

                    response.EnsureSuccessStatusCode();

                    var data = await response.Content.ReadAsStringAsync();
                    if (data != "ok")
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static async Task<List<TestItem>> GetTestItems()
        {
            var result = new List<TestItem>();

            var exams = new List<ExamItem>();
            using (var server = new ServerClient())
            {
                var response = await server.HttpClient.PostAsync(StaticValues.GetExamsUrl, null);

                var rawData = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(rawData))
                {
                    exams = JsonConvert.DeserializeObject<List<ExamItem>>(rawData);
                }
            }

            if (exams.Count == 0)
            {
                return result;
            }

            var data = new List<KeyValuePair<string, ExamItem>>();

            using (var db = new DataService())
            {
                using (var client = new WebClient())
                {
                    foreach (var item in exams)
                    {
                        if (string.IsNullOrEmpty(item.File))
                        {
                            continue;
                        }

                        try
                        {
                            string fileName;
                            var dir = $"{StaticValues.RootPath}\\temp\\";

                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }

                            do
                            {
                                fileName = $"{dir}{Guid.NewGuid()}";
                            } while (File.Exists(fileName));

                            client.DownloadFile(item.File, fileName);

                            data.Add(new KeyValuePair<string, ExamItem>(fileName, item));
                        }
                        catch (Exception)
                        {
                            NotificationsHelper.Error($"خطا در دانلود {item.Title}", "خطا دانلود");
                        }
                    }
                }
            }

            var importer = new ImportExportHelper();

            foreach (var item in data)
            {
                var file = item.Key;

                if (!File.Exists(file))
                {
                    continue;
                }

                try
                {
                    var testItem = await importer.Import(file, item.Value.Id);
                    result.Add(testItem);
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    NotificationsHelper.Error($"خطا در ثبت '{Path.GetFileName(file)}'", $"خطا {ex.HResult}");
                }
            }

            return result;
        }

        private static async Task<List<TestItem>> GetNewTestItemsFromServer()
        {
            var result = new List<TestItem>();

            var exams = new List<ExamItem>();
            using (var server = new ServerClient())
            {
                var response = await server.HttpClient.PostAsync(StaticValues.GetExamsUrl, null);

                var rawData = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(rawData))
                {
                    exams = JsonConvert.DeserializeObject<List<ExamItem>>(rawData);
                }
            }

            if (exams.Count == 0)
            {
                return result;
            }

            var data = new List<KeyValuePair<string, ExamItem>>();

            using (var db = new DataService())
            {
                using (var client = new WebClient())
                {
                    foreach (var item in exams)
                    {
                        if (string.IsNullOrEmpty(item.File))
                        {
                            continue;
                        }

                        if (db.TestIsThere(item.Id))
                        {
                            continue;
                        }

                        try
                        {
                            string fileName;
                            var dir = $"{StaticValues.RootPath}\\temp\\";

                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }

                            do
                            {
                                fileName = $"{dir}{Guid.NewGuid()}";
                            } while (File.Exists(fileName));

                            client.DownloadFile(item.File, fileName);

                            data.Add(new KeyValuePair<string, ExamItem>(fileName, item));
                        }
                        catch (Exception)
                        {
                            NotificationsHelper.Error($"خطا در دانلود {item.Title}", "خطا دانلود");
                        }
                    }
                }
            }

            var importer = new ImportExportHelper();

            foreach (var item in data)
            {
                var file = item.Key;

                if (!File.Exists(file))
                {
                    continue;
                }

                try
                {
                    var testItem = await importer.Import(file, item.Value.Id);
                    result.Add(testItem);
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    NotificationsHelper.Error($"خطا در ثبت '{Path.GetFileName(file)}'", $"خطا {ex.HResult}");
                }
            }

            return result;
        }
    }
}
