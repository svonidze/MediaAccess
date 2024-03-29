namespace Telegram.BotServer
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using BitTorrent.Contracts;
    using BitTorrent.Transmission;

    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using Jackett.Contracts;

    using MediaServer.Contracts;
    using MediaServer.Workflow;

    using Telegram.Bot;

    public static class WindsorInstaller
    {
        public static IWindsorInstaller Register(Configuration? configuration = default) =>
            configuration == default
                ? new WithNoConfiguration()
                : new WithConfiguration(configuration);

        private class WithConfiguration : IWindsorInstaller
        {
            private readonly Configuration configuration;

            public WithConfiguration(Configuration configuration)
            {
                this.configuration = configuration;
            }

            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.Register(
                    Component.For<ITelegramBotClient>().UsingFactoryMethod(
                        () =>
                            {
                                var telegramBotTimeout =
                                    this.configuration.TelegramBot.Timeout ?? TimeSpan.FromMinutes(1);

                                var proxy = this.configuration.TelegramBot.Proxy;
                                if (proxy != null)
                                {
                                    var webProxy = new WebProxy(proxy.Host, proxy.Port)
                                        {
                                            Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
                                        };
                                    return new TelegramBotClient(this.configuration.TelegramBot.Token, webProxy)
                                        {
                                            Timeout = telegramBotTimeout
                                        };
                                }
                                return new TelegramBotClient(this.configuration.TelegramBot.Token)
                                    {
                                        Timeout = telegramBotTimeout
                                    };
                            }).LifestyleSingleton());

                container.Register(
                    Component.For<ITelegramListener>().ImplementedBy<TelegramListener>().DependsOn(
                            Dependency.OnValue<IEnumerable<long>>(this.configuration.TelegramBot.AllowedChats))
                        .LifestyleTransient(),
                    Component.For<ITelegramChatListener>().ImplementedBy<TelegramChatListener>()
                        .DependsOn(Dependency.OnValue<ViewFilterConfiguration>(this.configuration.ViewFilter))
                        .LifestyleTransient(),
                    Component.For<IJackettIntegration>().ImplementedBy<JackettIntegration>()
                        .DependsOn(Dependency.OnValue<IJackettAccessConfiguration>(this.configuration.Jackett))
                        .LifestyleTransient());

                if (this.configuration.BitTorrent == null)
                    container.Register(
                        Component.For<IBitTorrentClient>().ImplementedBy<NotSetUpBitTorrentClient>()
                            .LifestyleSingleton());
                else
                    container.Register(
                        Component.For<IBitTorrentClient>().ImplementedBy<TransmissionClient>().DependsOn(
                                Dependency.OnValue<BitTorrentClientConfiguration>(this.configuration.BitTorrent))
                            .LifestyleTransient());
            }
        }

        private class WithNoConfiguration : IWindsorInstaller
        {
            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.Register(Component.For<ITelegramFactory>().AsFactory());

                container.Register(
                    Component.For<ITelegramClientAndServerLogger>().ImplementedBy<TelegramClientAndServerLogger>()
                        .LifestyleTransient(),
                    Component.For<IServerLogger>().ImplementedBy<ServerConsoleLogger>().LifestyleTransient());
            }
        }
    }
}