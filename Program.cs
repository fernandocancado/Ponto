using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        string chromeVersion = GetChromeVersion();

        if (string.IsNullOrEmpty(chromeVersion))
        {
            Console.WriteLine("Não foi possível determinar a versão do Google Chrome.");
            return;
        }

        string majorVersion = chromeVersion.Split('.')[0];

        string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string chromeDriverPath = Path.Combine(projectDirectory, "ChromeDriver", majorVersion);

        if (!Directory.Exists(chromeDriverPath))
        {
            Console.WriteLine($"O diretório ChromeDriver para a versão {majorVersion} não foi encontrado.");
            return;
        }

        ChromeOptions chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--start-maximized");
        IWebDriver driver = new ChromeDriver(chromeDriverPath, chromeOptions);

        try
        {
            driver.Navigate().GoToUrl("https://rp.fikdigital.com.br/");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            Task.Delay(2000).Wait();
            IWebElement loginField = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='login']")));
            loginField.Clear();
            loginField.SendKeys("0040");


            IWebElement passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='password']/input")));
            passwordField.Clear();
            passwordField.SendKeys("1190");

            IWebElement registerButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@aria-label='Registrar presença']")));
            registerButton.Click();

            Task.Delay(2000).Wait();
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            string directoryPath = @"G:\Meu Drive\Ponto";
            string fileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            string filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            screenshot.SaveAsFile(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        finally
        {
            driver.Quit();
        }
    }

    private static string GetChromeVersion()
    {
        string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

        if (!File.Exists(chromePath))
        {
            return null;
        }

        var versionInfo = FileVersionInfo.GetVersionInfo(chromePath);
        return versionInfo.FileVersion!;
    }
}
