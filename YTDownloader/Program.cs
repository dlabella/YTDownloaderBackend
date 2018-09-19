using Common.Logging;
using Common.Tracing;
using SimpleWebApiServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using System.Threading;

namespace YTDownloader
{
    internal class Program
    {

        private static readonly ManualResetEvent ResetEvent = new ManualResetEvent(false);
        private static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += CancelHandler;
            System.AppDomain.CurrentDomain.UnhandledException += ProcessUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += ProcessFirstChanceException;
            try
            {
                AppContext.Initialize();
                PrintConfig();

                var config = AppContext.Config;
                if (config.Debug == "true")
                {
                    Logger.LogLevel = Logger.LogLevelEnum.Debug;
                    Trace.Listeners.Add(new ConsoleTraceListener());
                }

                var server = new WebServer(config.Ip, config.Port, config.UrlPrefix);
                RegisterRequestHandlers(server);
                
                server.Start();
                Logger.Debug($"Listening at {config.Ip}:{config.Port} with urlPrefix {config.UrlPrefix}");
                Logger.Debug("Press Ctrl+C to exit ...");
                ResetEvent.WaitOne();
                server.Stop();

            }
            catch (Exception ex)
            {
                Logger.Exception("General failure exception: " + ex.Message, ex);
            }
        }

        private static void ProcessFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            Logger.Exception("First chance exception", e.Exception);
        }

        private static void ProcessUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Exception("Unhandled exception", e.ExceptionObject as Exception);
        }

        private static void PrintConfig()
        {
            Logger.Debug("** CONFIG **");
            Logger.Debug($"Temp path:    {AppContext.Config.TempPath}");
            Logger.Debug("************");
        }

        private static string GetStringArgument(IReadOnlyList<string> arguments, int index)
        {
            return arguments.Count > index ? arguments[index] : string.Empty;
        }

        private static int GetIntArgument(IReadOnlyList<string> arguments, int index)
        {
            if (arguments.Count <= index)
            {
                return 0;
            }

            var num = GetStringArgument(arguments, index);
            return int.TryParse(num, out var inum) ? inum : 0;
        }

        private static void RegisterRequestHandlers(WebServer server)
        {
            server.RegisterRequestHandler(new Api.GetDownloads(WebServer.Cache));
            server.RegisterRequestHandler(new Api.Download(WebServer.Cache));
            server.RegisterRequestHandler(new Api.CancelDownload(WebServer.Cache));
        }

        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            ResetEvent.Set();
            Logger.Debug("Unloading...");
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ResetEvent.Set();
            Logger.Debug("Exiting...");
        }
    }
}
