namespace Telegram.FirstBot
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

    public class WindsorInstaller : IWindsorInstaller
    {
        private readonly Configuration configuration;

        public WindsorInstaller(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ITelegramFactory>().AsFactory());
            
            container.Register(
                Component.For<ITelegramBotClient>().UsingFactoryMethod(
                    () =>
                        {
                            var proxy = this.configuration.Proxy;
                            var webProxy = new WebProxy(proxy.Host, proxy.Port)
                                {
                                    Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
                                };
                            return new TelegramBotClient(this.configuration.TelegramBot.Token, webProxy)
                                {
                                    Timeout = this.configuration.TelegramBot.Timeout ?? TimeSpan.FromMinutes(1)
                                };

                        }).LifestyleSingleton());
            
            container.Register(
                Component.For<ITelegramListener>().ImplementedBy<TelegramListener>()
                    .DependsOn(Dependency.OnValue<IEnumerable<long>>(this.configuration.TelegramBot.AllowedChats))
                    .LifestyleTransient(),
                Component.For<ITelegramClientAndServerLogger>().ImplementedBy<TelegramClientAndServerLogger>()
                    .LifestyleTransient(),
                Component.For<ITelegramChatListener>().ImplementedBy<TelegramChatListener>()
                    .DependsOn(Dependency.OnValue<ViewFilterConfiguration>(this.configuration.ViewFilter))
                    .LifestyleTransient(),
                Component.For<IJackettIntegration>().ImplementedBy<JackettIntegration>()
                    .DependsOn(Dependency.OnValue<IJackettAccessConfiguration>(this.configuration.Jackett))
                    .LifestyleTransient());

            if (this.configuration.BitTorrent == null)
                container.Register(
                    Component.For<IBitTorrentClient>().ImplementedBy<NotSetUpBitTorrentClient>().LifestyleSingleton());
            else
                container.Register(
                    Component.For<IBitTorrentClient>().ImplementedBy<TransmissionClient>()
                        .DependsOn(Dependency.OnValue<BitTorrentClientConfiguration>(this.configuration.BitTorrent))
                        .LifestyleTransient());
        }
    }
}