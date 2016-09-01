using NHSD.ElephantParade.Core.EncounterHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Questionnaires.Core.Services;
using Questionnaires.Core.Services.Models;
using System.Collections.Generic;
using NHSD.ElephantParade.Domain.Models;

namespace ElephantParade.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for QuestionSetHandlerTest and is intended
    ///to contain all QuestionSetHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QuestionSetHandlerTest
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
        ///A test for Process
        ///</summary>
        [TestMethod()]
        public void ProcessTest()
        {
            IQuestionnaireService _questionnaireService = null; // TODO: Initialize to an appropriate value
            QuestionSetHandler target = new QuestionSetHandler(_questionnaireService); // TODO: Initialize to an appropriate value
            QuestionnaireQuestionSet item = null; // TODO: Initialize to an appropriate value
            AnswerSet answerSet = null; // TODO: Initialize to an appropriate value
            IList<AnswerSetAnswer> answers = null; // TODO: Initialize to an appropriate value
            IEnumerable<ModuleLetter> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<ModuleLetter> actual;
            actual = target.Process(item, answerSet, answers);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
