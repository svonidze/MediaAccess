namespace ModulDengi.Core.Tests.Integration
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;

    using NUnit.Framework;

    public class EmailConfirmationManagerTests
    {
        class Options<T> : IOptions<T>
            where T : class, new()
        {
            public Options()
            {
            }

            public Options(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }
        
        [Test]
        public void Test1()
        {
            var emailSettings = new EmailSettings
                {
                    Credential = new Credential
                        {
                            Login = "",
                            Password = ""
                        },
                    Host = "",
                    Port = 993,
                    UseSsl = true,
                    EmailType = EmailType.Imap
                };
            
            //Arrange
            
            
                
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<EmailConfirmationManager>();
            services.AddSingleton(emailSettings);

            
            var serviceProvider = services.BuildServiceProvider();
            var confirmationManager = serviceProvider.GetService<EmailConfirmationManager>();

            var success = confirmationManager.CheckConfirmationReceived(DateTime.UtcNow.AddMinutes(-5), "308306", out var confirmationCode);

            Console.WriteLine(confirmationCode);
            Assert.True(success);
        }
    }
}