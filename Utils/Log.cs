using System;

namespace AdminBot.Net.Utils
{
    internal class Logger(string NameSpace,string Work = "Common")
    {
        private readonly string NameSpace = NameSpace;

        private readonly ConsoleColor DefaultConsoleColor = Console.ForegroundColor;
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[{0}][{1}][{2}] {3}", GetFormatTime(),NameSpace, Work, message);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[{0}][{1}][{2}] {3}", GetFormatTime(), NameSpace, Work, message);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        public void Info(string message)
        {
            Console.WriteLine("[{0}][{1}][{2}] {3}", GetFormatTime(), NameSpace, Work, message);
        }
        private string GetFormatTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}