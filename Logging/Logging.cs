using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DataProcessor.Logging
{
    public static class Logging
    {
        public static void Info(string prefix, string message)
        {
            DateTime now = DateTime.Now.ToLocalTime();

            Console.WriteLine(String.Format($"[{now.ToShortDateString()} - {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}] [{prefix}] - {message}"));
        }

        public static void Info(string message)
        {
            DateTime now = DateTime.Now.ToLocalTime();
            Console.WriteLine(String.Format($"[{now.ToShortDateString()} - {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}] [Info] - {message}"));
        }
        public static void Error(string prefix, string message, Exception error)
        {
            DateTime now = DateTime.Now.ToLocalTime();
            Console.WriteLine(String.Format($"[{now.ToShortDateString()} - {now.Hour}:{now.Minute}:{now.Second}.{now.Millisecond}] [{prefix}] - {message}" +
                String.Format($"{error.StackTrace}")));

        }
    }
}
