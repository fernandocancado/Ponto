using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Ponto;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

class Program
{
    private static string _chromeVersion { get; set; }
    private static string _chromeDriverVersion { get; set; }
    static void Main(string[] args)
    {
        string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

        if (File.Exists(chromePath))
        {
            FileVersionInfo chromeVersionInfo = FileVersionInfo.GetVersionInfo(chromePath);
            _chromeVersion = chromeVersionInfo.FileVersion!;
            _chromeDriverVersion = GetChomeVersion.GetChromeVersion();

            Console.WriteLine($"Versão do Chrome instalada: {_chromeVersion}");
            Console.WriteLine($"Versão do ChromeDriver instalada: {_chromeDriverVersion}");
        }
        else
        {
            Console.WriteLine("O Google Chrome não está instalado no caminho padrão.");
        }
        if (_chromeVersion != _chromeDriverVersion)
        {
            try
            {
                new DriverManager().SetUpDriver(new ChromeConfig());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao configurar o ChromeDriver: " + ex.Message);
                Console.WriteLine("Deseja continuar? (Y/N)");

                string resposta = Console.ReadLine().ToUpper();

                if (resposta == "Y")
                {
                    Console.WriteLine("Continuando...");
                }
                else if (resposta == "N")
                {
                    Console.WriteLine("Encerrando o programa...");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Entrada inválida. Encerrando o programa...");
                    Environment.Exit(0);
                }
            }
        }

        Console.WriteLine("Abrindo o Chrome...");
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
            loginField.SendKeys("0040");

            IWebElement passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='password']/input")));
            passwordField.Clear();
            passwordField.SendKeys("1190");

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
