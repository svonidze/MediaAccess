namespace Spain.Appointment.Utility;

using System.Diagnostics;

using Common.Exceptions;
using Common.System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class ChromeWebScraper : IDisposable
{
    private readonly Lazy<WebDriver> driver;

    protected WebDriver Driver => this.driver.Value;

    public ChromeWebScraper()
    {
        this.driver = LazyExtensions.InitLazy(this.InitWebDriver);
    }

    public void Dispose()
    {
        Debug.Print("Closing browser");
        try
        {
            if (this.driver.IsValueCreated)
            {
                this.driver.Value.Close();
                this.driver.Value.Dispose();    
            }
        }
        catch (Exception e)
        {
            Debug.Print(e.GetFullDescription("Error while closing browser"));
        }
    }
    
    private WebDriver InitWebDriver()
    {
        var userProfilePreferences = new Dictionary<string, string>
            {
                //{ "download.default_directory", this.StagingDirectoryPath }
            };

        // this.Logger.Debug(
        //     "Chrome is initializing with "
        //     + new
        //         {
        //             this.browserConfig.LaunchArguments,
        //             userProfilePreferences,
        //             CapabilityType.AcceptInsecureCertificates
        //         }.ToJson());
            
        var options = new ChromeOptions()
            .AddUserProfilePreferences(userProfilePreferences)
            .AddNotNullArguments(Array.Empty<string>());

        options.AcceptInsecureCertificates = true;
            
        options.SetLoggingPreference("performance", LogLevel.All);

        return new ChromeDriver(options);
    }
}

internal static class ChromeOptionsExtensions
{
    public static ChromeOptions AddUserProfilePreferences(
        this ChromeOptions options,
        Dictionary<string, string> userProfilePreferences)
    {
        foreach (var (key, value) in userProfilePreferences)
        {
            options.AddUserProfilePreference(key, value);                
        }
        return options;
    }

    public static ChromeOptions AddNotNullArguments(this ChromeOptions options, string[]? arguments)
    {
        if (arguments?.Any() == true)
        {
            options.AddArguments(arguments.Distinct().Select(a => $"--{a}"));
        }
        return options;
    }
}
