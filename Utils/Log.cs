using System;

namespace AdminBot.Net.Utils
{
    internal class Logger(string NameSpace)
    {
        private readonly string NameSpace = NameSpace;

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[{0}][{1}] {2}",GetFormatTime(),NameSpace,message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[{0}][{1}] {2}", GetFormatTime(), NameSpace, message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Info(string message)
        {
            Console.WriteLine("[{0}][{1}] {2}", GetFormatTime(), NameSpace, message);
        }
        private string GetFormatTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}