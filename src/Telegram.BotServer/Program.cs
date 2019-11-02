namespace Telegram.FirstBot
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Common.Serialization.Yaml;

    using MediaServer.Contracts;
    using MediaServer.Workflow;

    class Program
    {
        // TODO utilize library to parse CLI args 
        // TODO restrict users which can have access to Telegram
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            if (!args.Any())
            {
                Console.WriteLine("No configuration file path passed");
                Environment.Exit(1);
            }

            var configurationFilePath = args.First();
            if (!File.Exists(configurationFilePath))
            {
                Console.WriteLine($"No configuration file found at {configurationFilePath}");
                Environment.Exit(1);
            }

            var configuration = File.ReadAllText(configurationFilePath).FromYamlTo<Configuration>();
            Console.WriteLine($"{nameof(configuration)} is loaded from {configurationFilePath}");

            var telegramListener = new TelegramListener(configuration);
            telegramListener.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
        }
    }
}