// See https://aka.ms/new-console-template for more information

using Common.Exceptions;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

using Spain.Appointment.Utility;

new SpainAppointmentCatcher().TryDo();

class SpainAppointmentCatcher : ChromeWebScraper
{
    private static readonly TimeSpan DelayBetweenRequests = TimeSpan.FromSeconds(3);

    public void TryDo()
    {
        while (true)
        {
            try
            {
                this.Do();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            this.Sleep(TimeSpan.FromMinutes(2));
        }
    }

    public void Do()
    {
        this.Driver.Navigate().GoToUrl("https://sede.administracionespublicas.gob.es/pagina/index/directorio/icpplus");

        this.TryClick(By.Id("submit"));

        this.TryClick(By.Id("form"));

        this.SelectText(By.Id("form"), text: "Valencia");
        this.TryClick(By.Id("btnAceptar"));

        this.SelectText(
            By.Id("tramiteGrupo[1]"),
            text:
            "POLICIA-TOMA DE HUELLA (EXPEDICIÓN DE TARJETA), RENOVACIÓN DE TARJETA DE LARGA DURACIÓN Y DUPLICADO");
        if (!this.TryClick(By.Id("btnAceptar")))
        {
            return;
        }

        Console.WriteLine(this.Driver.Url);
        Console.WriteLine("https://icp.administracionelectronica.gob.es/icpplus/acInfo");
        if (!this.TryClick(By.Id("btnEntrar")))
        {
            return;
        }

        //
        this.Driver.FindElement(By.Id("txtIdCitado")).SendKeys("Z0503944L");
        this.Driver.FindElement(By.Id("txtDesCitado")).SendKeys("Karen Babayan");
        this.SelectText(By.Id("txtPaisNac"), text: "RUSIA");

        Console.WriteLine("https://icp.administracionelectronica.gob.es/icpplus/acEntrada");
        if (!this.TryClick(By.Id("btnEnviar")))
        {
            return;
        }

        Console.WriteLine("https://icp.administracionelectronica.gob.es/icpplus/acValidarEntrada");
        if (!this.TryClick(By.Id("btnEnviar")))
        {
            return;
        }

        if (this.Driver.FindElement(By.Id("mensajeInfo")).Text.Contains("En este momento no hay citas disponibles"))
        {
            Console.WriteLine("No appointments!");
        }
        else
        {
            Console.WriteLine("There is a free appointments!");
            // TODO send a notification
            this.Sleep(TimeSpan.FromHours(2));
        }
    }

    private void SelectText(By selector, string text)
    {
        var webElement = this.Driver.FindElement(selector);
        new SelectElement(webElement).SelectByText(text);
    }

    private void Sleep(TimeSpan? sleepTime = null, string? reason = null)
    {
        sleepTime ??= DelayBetweenRequests;
        Console.WriteLine($"Sleeping {sleepTime} {reason}");
        Thread.Sleep(sleepTime.Value);
    }

    private bool TryClick(By by) =>
        this.TryFindElementAndPerform(
            by,
            element =>
                {
                    Console.WriteLine($"Clicking {by}");
                    element.Click();
                });

    private bool TryFindElementAndPerform(By by, Action<IWebElement> action)
    {
        var wait = new WebDriverWait(this.Driver, DelayBetweenRequests * 2);

        return wait.Until(
            condition =>
                {
                    try
                    {
                        var element = this.Driver.FindElement(by);
                        if (!element.Displayed || !element.Enabled)
                        {
                            Console.WriteLine($"Element {by} is not Displayed nor Enabled yet.");
                            return false;
                        }

                        Console.WriteLine($"Element {by} is ready");
                        action(element);
                        return true;
                    }
                    catch (StaleElementReferenceException e)
                    {
                        Console.WriteLine(e.GetShortDescription($"Element {by} is not ready"));
                        return false;
                    }
                    catch (NoSuchElementException e)
                    {
                        Console.WriteLine(e.GetShortDescription($"Element {by} not found"));
                        return false;
                    }
                    catch (ElementClickInterceptedException e)
                    {
                        Console.WriteLine(e.GetShortDescription($"Element {by} is not clickable, try one more time"));
                        Thread.Sleep(DelayBetweenRequests);
                        return this.TryFindElementAndPerform(by, action);
                    }
                });
    }
}