namespace Telegram.BotServer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Castle.Facilities.TypedFactory;
    using Castle.Windsor;

    using Common.Exceptions;
    using Common.Net;
    using Common.Serialization.Yaml;

    using MediaServer.Contracts;

    internal class Program
    {
        private const string ConfigFilePath = "/config/config.yaml";
        private static readonly TimeSpan AttemptDelay = TimeSpan.FromSeconds(30);

        // TODO utilize library to parse CLI args 
        static void Main(string[] args)
        {
            using var container = InitializeContainer().Install(WindsorInstaller.Register());
            
            var serverLogger = container.Resolve<IServerLogger>();
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                UnhandledExceptionTrapper(eventArgs, serverLogger);

            string configurationFilePath;
            if (args.Any())
            {
                configurationFilePath = args.First();
            }
            else
            {
                serverLogger.Log($"No configuration file path passed, trying to use the default one at {ConfigFilePath}");
                configurationFilePath = ConfigFilePath;
            }

            if (!File.Exists(configurationFilePath))
            {
                serverLogger.Log($"No configuration file found at {configurationFilePath}");
                Environment.Exit(1);
            }

            var configuration = File.ReadAllText(configurationFilePath).FromYamlTo<Configuration>();

            serverLogger.Log($"{nameof(configuration)} is loaded from {configurationFilePath}");
            container.Install(WindsorInstaller.Register(configuration));

            var telegramListener = container.Resolve<ITelegramListener>();
            RepeatIfFailed(telegramListener.StartReceiving, serverLogger);
            Thread.Sleep(int.MaxValue);
        }

        private static void UnhandledExceptionTrapper(UnhandledExceptionEventArgs e, IServerLogger serverLogger)
        {
            serverLogger.Log(e.ExceptionObject.ToString());
        }

        private static IWindsorContainer InitializeContainer()
        {
            return new WindsorContainer().AddFacility<TypedFactoryFacility>();
        }
        
        private static void RepeatIfFailed(Action action, IServerLogger serverLogger, int attempt = 0)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                var delay = AttemptDelay * Math.Pow(2, attempt); 
                
                serverLogger.Log($"Internet is{(Internet.IsAvailable() ? null : " NOT")} available");
                serverLogger.Log(e.GetFullDescription($"Attempt #{attempt} failed, next try in {delay}"));

                Thread.Sleep(delay);
                RepeatIfFailed(action, serverLogger, attempt + 1);
            }
        }
    }
}