using System;
using Newtonsoft.Json;
using System.Text;

namespace AdminBot.Net.NetWork
{
    internal class HttpService
    {
        private static readonly HttpClient HClient = new();

        public static async Task<string> POST(string Url, object? Content = null)
        {
            try
            {
                HttpResponseMessage response = await HClient.PostAsync(Url, Content == null ? null : new StringContent(
                       JsonConvert.SerializeObject(Content),
                       Encoding.UTF8,
                       "application/json"
                   ));
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return "";
            }
        }

        public static async Task<string> Get(string Url)
        {
            try
            {
                HttpResponseMessage response = await HClient.GetAsync(Url);
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return "";
            }
        }

        public static async Task<byte[]> GetBinary(string Url)
        {
            try
            {
                return await HClient.GetByteArrayAsync(Url);
            }
            catch
            {
                return [];
            }
        }
    }
}
