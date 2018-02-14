using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Sofco.WebTest.Login
{
    [TestFixture]
    public class LoginTest
    {
        private string baseUrl = "http://azsof01wd:8000";

        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new FirefoxOptions();
            options.SetPreference("security.sandbox.content.level", 5);
            driver = new FirefoxDriver(options) { Url = baseUrl };

            driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }

        [Test]
        public void ShouldPassLogin()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            driver.Navigate().GoToUrl("");

            var usernameElement = driver.FindElement(By.Name("username"));
            usernameElement.Click();
            usernameElement.SendKeys("jlarenze");

            var passwordElement = driver.FindElement(By.Name("password"));
            passwordElement.Click();
            passwordElement.SendKeys("Sofco2018");

            var submit = driver.FindElement(By.TagName("button"));
            submit.Click();

            driver.FindElement(By.ClassName("app-version"));

            Assert.True(driver.Url.EndsWith("/inicio"));
        }

        [Test]
        public void ShouldPassGoToUsers()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl("");

            var usernameElement = driver.FindElement(By.Name("username"));
            usernameElement.Click();
            usernameElement.SendKeys("jlarenze");

            var passwordElement = driver.FindElement(By.Name("password"));
            passwordElement.Click();
            passwordElement.SendKeys("Sofco2018");

            var submit = driver.FindElement(By.TagName("button"));
            submit.Click();

            driver.FindElement(By.ClassName("app-version"));

            var menuFirstElement = driver.FindElement(By.ClassName("nav-label"));
            menuFirstElement.Click();

            var userMenuElement = driver.FindElement(By.XPath("//a[contains(text(),'Usuarios')]"));
            userMenuElement.Click();

            Assert.True(driver.Url.EndsWith("#/admin/users"));
        }
    }
}
