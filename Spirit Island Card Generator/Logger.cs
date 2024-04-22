using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Spirit_Island_Card_Generator
{
    internal class Logger
    {
        public static LoggerConfiguration DefaultConfig => new LoggerConfiguration().WriteTo.File(@"C:\Users\tim\Dev\Logs\Spirit Island\MainLog.txt");

        public static LoggerConfiguration MakeDeckLogger(string path)
        {
            return new LoggerConfiguration().WriteTo.File(path);
        }

        public static void ChangeLogConfig(LoggerConfiguration config)
        {
            Log.Logger = config.CreateLogger();
        }
    }
}
