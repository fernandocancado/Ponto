using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

class Program
{
    static void Main(string[] args)
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        ChromeOptions chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--start-maximized");
        IWebDriver driver = new ChromeDriver(chromeOptions);

        try
        {
            driver.Navigate().GoToUrl("https://rp.fikdigital.com.br/");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            Task.Delay(2000).Wait();

            IWebElement loginField = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='login']")));
            loginField.Clear();
            loginField.SendKeys("");

            IWebElement passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='password']/input")));
            passwordField.Clear();
            passwordField.SendKeys("");

            IWebElement registerButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@aria-label='Registrar presença']")));
            registerButton.Click();

            (bool, IWebElement) sucesso = WaitElementSafe(By.XPath("//*[contains(text(), 'sucesso')]"), wait);
            (bool, IWebElement) sucessoM = WaitElementSafe(By.XPath("//*[contains(text(), 'Sucesso')]"), wait);
            if (sucesso.Item1 || sucessoM.Item1)
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ponto");
                string fileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
                string filePath = Path.Combine(directoryPath, fileName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                screenshot.SaveAsFile(filePath);
                Console.WriteLine($"Screenshot salvo em: {filePath}");
            }
            else
            {
                Console.WriteLine("Elemento de sucesso não foi encontrado dentro do tempo limite.");
            }            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            driver.Quit();
        }
        Console.ReadKey();
    }

    private static (bool, IWebElement) WaitElementSafe(By by, WebDriverWait wait)
    {
        try
        {
            IWebElement successElement = wait.Until(ExpectedConditions.ElementIsVisible(by));
            return (true, successElement);
        }
        catch (NoSuchElementException)
        {
            return (false, null);
        }
    }
}
