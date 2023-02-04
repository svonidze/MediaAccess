namespace MediaAccess.Workflow.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using BitTorrent.Contracts;

    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using Jackett.Contracts;

    using MediaServer.Contracts;
    using MediaServer.Workflow;

    using NUnit.Framework;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class TelegramChatListenerTests
    {
        [Test]
        public void SendMessage()
        {
            var configuration = new Configuration
                {
                    ViewFilter = new ViewFilterConfiguration
                        {
                            ResultNumberOnPage = 5
                        },
                };
            var message = new Message
                {
                    Text = "/t xxx"
                };

            using var container = InitializeContainer().Install(Register(configuration));
            var factory = container.Resolve<ITelegramFactory>();
            var chatListener = factory.CreateChatListener();

            var log = factory.CreateClientAndServerLogger(message);
            chatListener.Handle(message, log);
        }

        private static IWindsorContainer InitializeContainer() =>
            new WindsorContainer().AddFacility<TypedFactoryFacility>();

        private static IWindsorInstaller Register(Configuration configuration) => new WithConfiguration(configuration);

        private class WithConfiguration : IWindsorInstaller
        {
            private readonly Configuration configuration;

            public WithConfiguration(Configuration configuration)
            {
                this.configuration = configuration;
            }

            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.Register(Component.For<ITelegramFactory>().AsFactory());

                container.Register(
                    Component.For<IBitTorrentClient>().ImplementedBy<NotSetUpBitTorrentClient>().LifestyleSingleton());

                container.Register(
                    Component.For<ITelegramClientAndServerLogger>().ImplementedBy<ConsoleLogger>().LifestyleTransient(),
                    Component.For<IServerLogger>().ImplementedBy<ServerConsoleLogger>().LifestyleTransient(),
                    Component.For<IJackettIntegration>().ImplementedBy<FakeJackettIntegration>().LifestyleTransient());

                container.Register(
                    Component.For<ITelegramChatListener>().ImplementedBy<TelegramChatListener>()
                        .DependsOn(Dependency.OnValue<ViewFilterConfiguration>(this.configuration.ViewFilter))
                        .LifestyleTransient());
            }
        }

        //TODO use Moq and TelegramClientAndServerLogger
        private class ConsoleLogger : ServerConsoleLogger, ITelegramClientAndServerLogger
        {
            private string lastText;

            public void Text(string text)
            {
                this.lastText = text;
                ClientLogAll(text);
            }

            public void TextWithAction(
                string text,
                ChatAction chatAction,
                CancellationToken cancellationToken = default)
            {
                this.lastText = text;
                ClientLogAll(nameof(this.TextWithAction), chatAction, text);
            }

            public void ReplyBack(string text, IReplyMarkup replyMarkup = default)
            {
                this.lastText = text;
                ClientLogAll(nameof(this.ReplyBack), text);
            }

            public void LogLastMessage()
            {
                this.Log(this.lastText);
            }

            public Task TrySendDocumentBackAsync(Uri @from)
            {
                ClientLogAll(nameof(this.TrySendDocumentBackAsync), @from);
                return Task.CompletedTask;
            }

            private static void ClientLogAll(params object[] texts)
            {
                LogAll("Client", texts);
            }
        }

        private class FakeJackettIntegration : IJackettIntegration
        {
            public ManualSearchResult SearchTorrents(string searchRequest, params string?[] trackerNames)
            {
                return new ManualSearchResult
                    {
                        Indexers = new List<ManualSearchResultIndexer>(),
                        Results = new List<TrackerCacheResult>()
                    };
            }
        }
    }
}