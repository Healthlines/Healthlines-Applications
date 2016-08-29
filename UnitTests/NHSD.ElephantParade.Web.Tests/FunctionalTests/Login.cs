using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace NHSD.ElephantParade.Web.Tests.FunctionalTests
{
    /// <summary>
    /// Summary description for Login
    /// </summary>
    [TestClass]
    public class Login
    {
        private IWebDriver _driver;

        public Login()
        {
            //#############
            // Further note that other drivers (InternetExplorerDriver,
            // ChromeDriver, etc.) will require further configuration 
            // before this example will work. See the wiki pages for the
            // individual drivers at http://code.google.com/p/selenium/wiki
            // for further information.
            _driver = new FirefoxDriver();
            //_driver = new InternetExplorerDriver();
            //_driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 30));
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
         //[ClassInitialize()]
         //public static void MyClassInitialize(TestContext testContext)
         //{
             
         //}
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
         [TestInitialize()]
         public void MyTestInitialize() 
         {

         }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void LogOn()
        {
            //Arrange 
            //#############
            var username = "Testuser";
            var password = "Password1";
            var urlbase = "http://training.nhsdirect.nhs.uk/Healthlines";

            //Act
            
            //ensure we are logged off
            _driver.Navigate().GoToUrl(urlbase + "/Account/LogOff");

            _driver.Navigate().GoToUrl(urlbase + "/Account/LogOn");

            IWebElement element = _driver.FindElement(By.Id("UserName"));
            element.SendKeys(username);
            element = _driver.FindElement(By.Id("Password"));
            element.SendKeys("incorrectPassword");            
            element.Submit();

            bool found = _driver.PageSource.Contains("User name or password incorrect");
            bool s = false;
            s = true;
            Assert.IsFalse(_driver.PageSource.Contains("User name or password incorrect"));



        }

        //[TestMethod]
        //public void Test2()
        //{
        //    //Arrange
        //    //#############
        //    var username = "Testuser";
        //    var password = "Password1";
        //    var urlbase = "http://training.nhsdirect.nhs.uk/Healthlines";

        //    ISelenium selenium;
        //    //selenium = new DefaultSelenium(username, 4444, "*chrome", "http://localhost");
        //    //selenium.Start();
        //}
    }
}
