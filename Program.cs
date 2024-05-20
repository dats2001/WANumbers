using File = System.IO.File;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WaNumbers
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        static IWebDriver driver = null;

        static void Main(string[] args)
        {
            RunWa();
            string[] arr = File.ReadAllLines(args[0]);
            // Проверим существует ли такой номер в WhatsApp
            for (int i = 0; i < arr.Length; i++)
            {
                int wa = CheckWaNumber(arr[i]);
                arr[i] = $"{arr[i]};{wa}";
            }
            File.WriteAllLines(args[0], arr);
            driver!.Dispose();
        }


        static void RunWa()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-infobars");
            //options.AddArgument("--disable-dev-shm-usage");
            //options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            //save client session
            options.AddArgument("--user-data-dir=C:\\Temp");
            options.AddArguments("chrome.switches", "--disable-extensions");

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();            
        }


        static int CheckWaNumber(string s)
        {
            try
            {
                string uri = $"https://web.whatsapp.com/send?phone={s}&text=test";               
 
                driver.Navigate().GoToUrl(uri);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                IWebElement dialog = driver.FindElements(By.XPath("//div[contains(text(),'ссылке, недействительный.')]")).FirstOrDefault();
                if (dialog != null)
                {
                    Console.WriteLine($"{s} - нет");
                    return 0;
                }
                Console.WriteLine($"{s} - да");
                return 1;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return 0;
            }
        }
    }
}