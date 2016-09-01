using NHSD.ElephantParade.Duke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using NHSD.ElephantParade.Domain.Models;

namespace ElephantParade.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for DukeServicesTest and is intended
    ///to contain all DukeServicesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DukeServicesTest
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
        ///A test for EncountersList
        ///</summary>
        [TestMethod()]
        public void EncountersListTest()
        {
            CvdService target = new CvdService(); 
            string participantID = "1"; 
            IList<QuestionnaireResults> expected = null; // TODO: Initialize to an appropriate value
            IList<QuestionnaireSession> actual;
            actual = target.QuestionnaireSessionList(participantID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
