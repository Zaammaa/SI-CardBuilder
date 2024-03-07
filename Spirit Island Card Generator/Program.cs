using Serilog;

namespace Spirit_Island_Card_Generator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("C:\\Users\\tim\\Dev\\Logs\\Spirit Island\\MainLog.txt").CreateLogger();

            Log.Information("************************************************************Start*************************************************************************");

            //Log.CloseAndFlush();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new CardGeneratorForm());
        }
    }
}