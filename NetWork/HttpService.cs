using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdminBot.Net.Utils;
using AdminBot.Net.Command;
using System.Net.Sockets;

namespace AdminBot.Net.NetWork
{
    internal class HttpService
    {
        private static readonly HttpClient HClient = new();

        private static readonly Logger HServiceLogger = new("HttpService");

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
            catch (Exception ex)
            {
                HServiceLogger.Error("Error Occured, Error Information:");
                HServiceLogger.Error(ex.Message);
                HServiceLogger.Error(ex.StackTrace ?? "");
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
            catch (Exception ex)
            {
                HServiceLogger.Error("Error Occured, Error Information:");
                HServiceLogger.Error(ex.Message);
                HServiceLogger.Error(ex.StackTrace ?? "");
                return "";
            }
        }

        public static async Task<byte[]> GetBinary(string Url)
        {
            try
            {
                return await HClient.GetByteArrayAsync(Url);
            }
            catch(Exception ex)
            {
                HServiceLogger.Error("Error Occured, Error Information:");
                HServiceLogger.Error(ex.Message);
                HServiceLogger.Error(ex.StackTrace ?? "");
                return [];
            }
        }
    }

    public class HttpServer
    {

        private readonly HttpListener httpListener = new();

        private readonly Logger HServerLogger = new("HttpServer");

        public HttpServer(string Prefixe)
        {
            httpListener.Prefixes.Add(Prefixe);
        }
        public async Task Start()
        {
            httpListener.Start();
            HServerLogger.Info("Http Server Started");
            while (httpListener.IsListening)
            {
                try
                {
                    var context = await httpListener.GetContextAsync().WaitAsync(new CancellationToken());
                    _ = HandleRequestAsync(context);
                    //catch { }
                }
                catch(Exception ex)
                {
                    HServerLogger.Error("Error Occured, Error Information:");
                    HServerLogger.Error(ex.Message);
                    HServerLogger.Error(ex.StackTrace ?? "");
                }
            }
        }
        public void Stop()
        {
            HServerLogger.Info("Http Server Stopped");
            httpListener.Stop();
            httpListener.Close();
        }
        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            try
            {
                StreamReader sr = new(context.Request.InputStream);
                string TempString = sr.ReadToEnd().Replace("\\u",";/.-u").Replace("\\", "-:/&]").Replace(";/.-u","\\u");
                string ReqString = Regex.Unescape(TempString);
                sr.Close();
                context.Response.StatusCode = 200;
                context.Response.Close();
                await CommandResolver.HandleMsg(JsonConvert.DeserializeObject<MsgBodySchematics>(ReqString.Replace("-:/&]","\\")));
            }
            catch (Exception ex)
            {
                HServerLogger.Error("Error Occured, Error Information:");
                HServerLogger.Error(ex.Message);
                HServerLogger.Error(ex.StackTrace ?? "");
            }
        }
    }
}
