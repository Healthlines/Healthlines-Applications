using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Areas.Administration.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using NHSD.ElephantParade.Core.Interfaces;
using System.Web.Mvc;
using NHSD.ElephantParade.Web.Areas.Administration.Models;
using NSubstitute;

namespace NHSD.ElephantParade.Web.Tests
{
    
    
    /// <summary>
    ///This is a test class for AdministrationControllerTest and is intended
    ///to contain all AdministrationControllerTest Unit Tests
    ///</summary>
    [TestClass]
    public class AdministrationControllerTest
    {


        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
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
        ///A test for Users
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\TFSWorkspace\\Healthlines\\Source\\NHSD.ElephantParade\\Source\\ElephantParade.Web", "/")]
        [UrlToTest("http://localhost:4136/")]
        public void UsersTest()
        {
            IStudiesService studiesService = null; // TODO: Initialize to an appropriate value
            ICallbackService callbackService = null; // TODO: Initialize to an appropriate value
            IReadingService readingService = null; // TODO: Initialize to an appropriate value
            IMembershipService membershipService = null; // TODO: Initialize to an appropriate value
            INonSecureEmailService nonSecureEmailService = null; // TODO: Initialize to an appropriate value
            AdministrationController target = new AdministrationController(studiesService, callbackService, readingService, membershipService, nonSecureEmailService); // TODO: Initialize to an appropriate value
            string search = string.Empty; // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = target.Users(search);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///  <summary>
        /// A test for StudyPatientDataUpload
        /// </summary>
        [TestMethod]
        public void StudyPatientDataUploadTestInvalidColumns()
        {
            //Arrange
            var depressionService = Substitute.For<IDepressionService>();
            var cvdService = Substitute.For<ICvdService>();
            var fileRepository = Substitute.For<IFileRepository>();
            var eventRepository = Substitute.For<IEventRepository>();
            IStudiesService studiesService = new StudyService(depressionService, cvdService, fileRepository, eventRepository);

            ICallbackService callbackService = Substitute.For<ICallbackService>(); 
            IReadingService readingService = Substitute.For<IReadingService>(); 
            IMembershipService membershipService = Substitute.For<IMembershipService>();
            INonSecureEmailService nonSecureEmailService = Substitute.For<INonSecureEmailService>();
            AdministrationController target = new AdministrationController(studiesService, callbackService, readingService, membershipService, nonSecureEmailService);

            var assembly = Assembly.GetExecutingAssembly();
            var fileStream = assembly.GetManifestResourceStream("NHSD.ElephantParade.Web.Tests.Resources.Testsheet2-Invalid_columns.xlsx");
            HttpPostedFileBase file = Substitute.For<HttpPostedFileBase>();
            file.InputStream.Returns(fileStream);
            file.FileName.Returns("Testsheet2-Invalid_columns.xlsx");
            file.ContentLength.Returns((int)fileStream.Length);

            //Act
            ActionResult actual = target.StudyPatientDataUpload("Testuser",file);
            
            //Assert
            Assert.IsInstanceOfType(actual, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)actual;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImportResults));
            ImportResults model = (ImportResults)viewResult.ViewData.Model;
            Assert.AreEqual(model.FileErrors.Length, 1);
            
        }

        ///  <summary>
        /// A test for StudyPatientDataUpload
        /// </summary>
        [TestMethod]
        public void StudyPatientDataUploadTest()
        {
            //Arrange
            var added = new List<StudyPatient>();

            //configure depressionService mocking
            var depressionService = Substitute.For<IDepressionService>();
            depressionService.StudyID.Returns("Depression");
            depressionService.AddPatient(Arg.Any<StudyPatient>(), Arg.Any<string>()).Returns(x =>
                                                                                                 {
                                                                                                     ((StudyPatient)x[0]).PatientId = Guid.NewGuid().ToString();
                                                                                                     return(StudyPatient)x[0];
                                                                                                 });
            depressionService.When(x => x.AddPatient(Arg.Any<StudyPatient>(), Arg.Any<string>())).Do(x => added.Add(((StudyPatient)x[0])));
            depressionService.DoesPatientExistInTheDatabase(Arg.Any<StudyPatient>()).Returns(x =>
                                                                                                {
                                                                                                    var t =(from p in added where p.Email == 
                                                                                                                ((StudyPatient)x[0]).Email
                                                                                                        select 1).ToArray();
                                                                                                    return t.Length > 0;
                                                                                                });

          
            var fileRepository = Substitute.For<IFileRepository>();
            var eventRepository = Substitute.For<IEventRepository>();
            IStudiesService studiesService = new StudyService(depressionService, null, fileRepository, eventRepository);

            ICallbackService callbackService = Substitute.For<ICallbackService>();
            IReadingService readingService = Substitute.For<IReadingService>();
            IMembershipService membershipService = Substitute.For<IMembershipService>();
            INonSecureEmailService nonSecureEmailService = Substitute.For<INonSecureEmailService>();
            AdministrationController target = new AdministrationController(studiesService, callbackService, readingService, membershipService, nonSecureEmailService);

            Assembly assembly = Assembly.GetExecutingAssembly();
            var fileStream = assembly.GetManifestResourceStream("NHSD.ElephantParade.Web.Tests.Resources.testsheet1.xlsx");
            //var fileStream = assembly.GetManifestResourceStream("NHSD.ElephantParade.Web.Tests.Resources.emptytest.xlsx");
            HttpPostedFileBase file = Substitute.For<HttpPostedFileBase>();
            file.InputStream.Returns(fileStream);
            file.FileName.Returns("testsheet1.xlsx");
            file.ContentLength.Returns((int)fileStream.Length);

            //Act
            ActionResult actual = target.StudyPatientDataUpload("TestUser",file);

            //Assert
            Assert.IsInstanceOfType(actual, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)actual;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImportResults));
            ImportResults model = (ImportResults)viewResult.ViewData.Model;


            Assert.AreEqual(21, model.FileErrors.Length);
            Assert.AreEqual(1,model.ImportErrors.Length);
            
        }
    }
}
