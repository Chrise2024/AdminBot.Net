using System;
using Newtonsoft.Json;
using System.Text;

namespace AdminBot.Net.NetWork
{
    internal class HttpService
    {
        private static readonly HttpClient HClient = new();

        public static string POST(string Url,object? Content = null)
        {
            HttpResponseMessage response = HClient.PostAsync(Url, Content == null ? null : new StringContent(
                       JsonConvert.SerializeObject(Content),
                       Encoding.UTF8,
                       "application/json"
                   )).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public static string Get(string Url)
        {
            HttpResponseMessage response = HClient.GetAsync(Url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public static byte[] GetBinary(string Url)
        {
            return HClient.GetByteArrayAsync(Url).Result;
        }
    }
}
