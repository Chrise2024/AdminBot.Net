using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdminBot.Net.Utils;
using AdminBot.Net.Command;
using AdminBot.Net.NetWork;
using System.Drawing;
using AdminBot.Net.Extra;

namespace AdminBot.Net
{
    internal class Program
    {
        private static readonly string ProgramRoot = Environment.CurrentDirectory;

        private static readonly string ProgramCahce = Path.Join(ProgramRoot, "cache");

        private static readonly ConfigManager MainConfigManager = new();

        private static readonly OPManager MainOPManager = new();

        private static readonly PermissionManager MainPermissionManager = new();
        
        static void Main(string[] args)
        {
            FileIO.EnsurePath(ProgramCahce);
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(MainConfigManager.GetHttpServerUrl());
            httpListener.Start();
            Task.Run(() =>
            {
                httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
            });
            Console.WriteLine("Launched");
            string TempString;
            while (true)
            {
                TempString = Console.ReadLine() ?? "";
                if (TempString.Equals("stop"))
                {
                    break;
                }
            }
        }
        static void GetContextCallBack(IAsyncResult result)
        {
            try
            {
                //Console.WriteLine("Received");
                HttpListener listener = (HttpListener)result.AsyncState;
                if (listener.IsListening)
                {
                    listener.BeginGetContext(new AsyncCallback(GetContextCallBack), listener);
                }
                HttpListenerContext context = listener.EndGetContext(result);
                StreamReader sr = new(context.Request.InputStream);
                string ReqString = Regex.Unescape(sr.ReadToEnd());
                sr.Close();
                context.Response.StatusCode = 200;
                context.Response.Close();
                //Console.WriteLine(Regex.Unescape(ReqString));
                CommandResolver.HandleMsg(JObject.Parse(ReqString));
            }
            catch { }
        }
        public static string GetProgramRoot()
        {
            return ProgramRoot;
        }
        public static string GetProgramCahce()
        {
            return ProgramCahce;
        }
        public static ConfigManager GetConfigManager()
        {
            return MainConfigManager;
        }
        public static OPManager GetOPManager()
        {
            return MainOPManager;
        }
        public static PermissionManager GetPermissionManager()
        {
            return MainPermissionManager;
        }
    }
}
