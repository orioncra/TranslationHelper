using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//selenium
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TranslationHelper.Model
{
    public class Crawl
    {
        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _options = null;
        protected ChromeDriver _driver = null;

        public Crawl()
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;
            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
            _options.AddArgument("headless");
        }

        public string GetWord(string TW)
        {
            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl("https://en.dict.naver.com/#/search?range=all&query="+TW);
            Thread.Sleep(1000);
            var element = _driver.FindElement(By.XPath("//*[@id='searchPage_entry']/div/div[1]/ul/li/p"));
            TW = element.Text;
            try
            {
                element = _driver.FindElement(By.XPath("//*[@id='searchPage_entry']/div"));
                TW = element.Text;
            }
            catch
            {

            }
            Debug.WriteLine(TW);
            return TW;
        }

        public string GetTranslatedText(string TT, string st, string ob)
        {
            _driver = new ChromeDriver(_driverService, _options);
            Debug.WriteLine("https://papago.naver.com/?sk=" + st + "&tk=" + ob + "&st=" + TT);
            _driver.Navigate().GoToUrl("https://papago.naver.com/?sk=" + st + "&tk=" + ob + "&st="+TT);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Thread.Sleep(1000);
            var element = _driver.FindElement(By.XPath("//*[@id='btnTranslate']"));
            element.Click();
            Thread.Sleep(1000);
            element = _driver.FindElement(By.XPath("//*[@id='txtTarget']"));
            Debug.WriteLine(element.Text);
            return element.Text;
        }

        public void GetIdiomReference(string q)
        {
            int LineCount = 0;
            //q = "\"" + q.Replace(" ", "\"+\"") + "\"";
            q = "\"" + q + "\"";
            Debug.WriteLine(q);
            string temp;
            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl("https://www.google.co.jp/search?q=" + q);
            List<string> lines = new List<string>();
            List<string> links = new List<string>();
            //아래 코드 정리 필요
            for(int i = 0; i < 10; i++)
            {
                Debug.WriteLine(i.ToString());
                for(int j = 1; j <= 10; j++)
                {
                    try
                    {
                        Debug.WriteLine("//*[@id='rso']/div[" + j.ToString() + "]/div/div/div[2]/div/span[2]");
                        if(j==10)
                        {
                            temp = _driver.FindElement(By.XPath("//*[@id='rso']/div[10]/div/div/div/div[2]/div/span[2]")).Text;
                        }
                        else
                        {
                            temp = _driver.FindElement(By.XPath("//*[@id='rso']/div[" + j.ToString() + "]/div/div/div[2]/div/span[2]")).Text;
                        }
                        Debug.WriteLine(temp);
                        Debug.WriteLine("add line");
                        if (lines.Contains(temp))
                        {
                            Debug.WriteLine("alredy in");
                        }
                        else
                        {
                            lines.Add(temp);
                            temp = _driver.FindElement(By.XPath("//*[@id='rso']/div[" + j.ToString() + "]/div/div/div[1]/a")).GetAttribute("href");
                            links.Add(temp);
                            LineCount++;
                        }
                    }
                    catch
                    {
                        try
                        {
                            Debug.WriteLine("//*[@id='rso']/div[" + j.ToString() + "]/div/div/div[2]/div/span[2]");
                            if (j == 10)
                            {
                                temp = _driver.FindElement(By.XPath("//*[@id='rso']/div[10]/div/div/div/div[2]/div")).Text;
                            }
                            else
                            {
                                temp = _driver.FindElement(By.XPath("//*[@id='rso']/div[" + j.ToString() + "]/div/div/div[2]/div")).Text;
                            }
                            Debug.WriteLine(temp);
                            Debug.WriteLine("add line");
                            if (lines.Contains(temp) != false)
                            {
                                lines.Add(temp);
                                LineCount++;
                                try
                                {
                                    temp = _driver.FindElement(By.XPath("//*[@id='rso']/div[" + j.ToString() + "]/div/div/div[1]/a")).GetAttribute("href");
                                    links.Add(temp);
                                }
                                catch
                                {
                                    links.Add("no link");
                                }
                            }
                            else
                            {
                                Debug.WriteLine("already in");
                            }
                            
                        }
                        catch
                        {

                        }
                        Debug.WriteLine("No more");
                    }
                }
                try
                {
                    _driver.FindElement(By.XPath("//*[@id='pnnext']/span[2]")).Click();
                }
                catch
                {
                    Debug.WriteLine("No more page");
                    break;
                }
            }
            //************************************************
            Debug.WriteLine("LINECOUNT = "+LineCount.ToString());
            using (StreamWriter OutputFile = new StreamWriter(@"IdiomReference.txt"))
            {
                for(int i = 0; i < LineCount; i++)
                {
                    Debug.WriteLine(i.ToString());
                    Debug.WriteLine(links[i]);
                    Debug.WriteLine(lines[i]);
                    OutputFile.WriteLine(links[i]);
                    OutputFile.WriteLine(lines[i]+"\n");
                }
            }
        }
    }
}
