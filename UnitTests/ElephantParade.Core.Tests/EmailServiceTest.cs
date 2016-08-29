using NHSD.ElephantParade.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NHSD.ElephantParade.Domain.Models;

namespace ElephantParade.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for EmailServiceTest and is intended
    ///to contain all EmailServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EmailServiceTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for EmailService Constructor
        ///</summary>
        [TestMethod()]
        public void EmailServiceConstructorTest()
        {
            string host = "MadeUphost";            
            string senderAccountAddress = "tom.axworthy@nhs.net";
            string senderEmailAddress = "tom.axworthy@nhs.net";
            string senderAccountPassword = "BLAAAAHHHHH";
            EmailService target = new EmailService(host, 587, senderAccountAddress, senderAccountPassword, senderEmailAddress);
        }

        /// <summary>
        ///A test for ReadFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NHSD.ElephantParade.Core.dll")]
        public void ReadFileTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            EmailService_Accessor target = new EmailService_Accessor(param0); // TODO: Initialize to an appropriate value
            string filename = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual = null;
            //actual = target.ReadFile(filename);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendEmail
        ///</summary>
        [TestMethod()]
        public void SendEmailTest()
        {
            const string senderEmailAddress = "nhsdirect.ict@nhs.net";
            const string senderAccountPassword = "y-ce8h5chePrat-hu6exa";
            const string host = "send.nhs.net";
            const int port = 587;
            EmailService target = new EmailService(host, port, senderEmailAddress, senderAccountPassword, senderEmailAddress);
            const string recipientEmailAddress = "tom.axworthy@nhsdirect.nhs.uk";
            const string subject = "Test from 1pm on 17th May 2012";
            const string body = "Some sample Body New line";
            const string attachmentFileName = "";
            target.SendEmail(recipientEmailAddress, subject, body, attachmentFileName, true);
        }
    }
}
