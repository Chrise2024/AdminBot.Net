using System.Net;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdminBot.Net.Utils;
using AdminBot.Net.Command;
using AdminBot.Net.NetWork;
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

        private static readonly HttpServer HServer = new(MainConfigManager.GetHttpServerUrl());

        private static readonly Logger MainLogger = new("Program");

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            MainLogger.Info("Bot Launched");
            FileIO.EnsurePath(ProgramCahce);
            Task.Run(HServer.Start);
            string TempString;
            while (true)
            {
                TempString = Console.ReadLine() ?? "";
                if (TempString.Equals("stop"))
                {
                    HServer.Stop();
                    break;
                }
            }
            MainLogger.Info("Bot Colsed");
            Console.ReadKey();
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
