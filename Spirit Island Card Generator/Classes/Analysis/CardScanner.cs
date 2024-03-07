using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;

namespace Spirit_Island_Card_Generator.Classes.Analysis
{
    internal class CardScanner
    {
        public static EffectReader reader = new EffectReader();
        public static void GetCardsFromKatalogSite()
        {
            // Configure ChromeOptions to run headlessly (without opening a browser window)
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            // Initialize ChromeDriver with the path to the ChromeDriver executable
            string htmlContent;
            using (IWebDriver driver = new ChromeDriver(options))
            {
                // Navigate to the URL
                driver.Navigate().GoToUrl("https://sick.oberien.de/?query=type%3Aminor");

                // Execute JavaScript code
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                jsExecutor.ExecuteScript("console.log('JavaScript executed successfully');");

                // Wait for some time to allow JavaScript to execute (adjust as needed)
                System.Threading.Thread.Sleep(500);

                // Get the HTML content after JavaScript execution
                htmlContent = driver.PageSource;
            }

            // Load the HTML string into an HtmlDocument
            if (htmlContent != null)
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                // Select all elements with class "back"
                HtmlNodeCollection backElements = htmlDoc.DocumentNode.SelectNodes("//div[@class='back']");

                if (backElements != null)
                {
                    int fullyParsedCount = 0;
                    // Iterate over the selected elements and print their inner HTML
                    foreach (HtmlNode backElement in backElements)
                    {
                        Card card = new Card();
                        card.Cost = Int32.Parse(GetProperty("cost", backElement.InnerHtml));
                        card.Name = GetProperty("name", backElement.InnerHtml);
                        card.Fast = GetProperty("speed", backElement.InnerHtml).Equals("fast") ? true : false;
                        //target
                        //range
                        //elements
                        card.elements = ElementSet.ElementSetFromElementString(GetProperty("elements", backElement.InnerHtml));
                        //description
                        card.descrition = GetProperty("description", backElement.InnerHtml);
                        if (reader.ScanDescription(card))
                        {
                            fullyParsedCount++;
                        }
                    }
                    Log.Information($"Finished parsing {backElements.Count} cards. {fullyParsedCount} successes.");
                }
                else
                {
                    Console.WriteLine("No elements with class 'back' found.");
                }
            }

        }

        /// <summary>
        /// The HTML we get back from the Katalog has the properties as <b>key</b>": value"<br>
        /// This returns the value string for the given key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetProperty(string name, string cardProperties)
        {
            Match match = Regex.Match(cardProperties, $"<b>{name}<\\/b>: ([^<]*)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim().ToLower();
            } else
            {
                return string.Empty;
            }
        }

    }
}
