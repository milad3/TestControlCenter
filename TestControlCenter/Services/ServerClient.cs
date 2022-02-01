using System;
using System.Net.Http;
using System.Net.Http.Headers;
using TestControlCenter.Models;

namespace TestControlCenter.Services
{
    public class ServerClient : IDisposable
    {
        public static AuthenticationItem AuthenticationData;

        public HttpClient HttpClient { get; set; }

        public ServerClient()
        {
            HttpClient = new HttpClient();

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationData.Token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpClient.Dispose();
            }
        }
    }
}
