
namespace ElephantParade.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using NHSD.ElephantParade.Core;
    using NHSD.ElephantParade.DAL.Interfaces;
    using Questionnaires.Core.Services;
    using Questionnaires.Core.Services.Models;

    [TestClass]
    public class DepressionServiceTests
    {
        [TestMethod]
        public void QuestionnaireActionsGet_WithResultSetWithMedicationAnswers_TriggersLetter()
        {
            var mockQuestionnaireService = new Mock<IQuestionnaireService>();

            var answerSets = new AnswerSet
                                 {
                                     AnswerSetID = 1,
                                     CurrentQuestionID = 2,
                                     OperatorID = "3",
                                     ParticipantID = "4",
                                     QuestionnaireID = 5,
                                     QuestionnaireTitle = "Test questionnaire",
                                     StartDate = DateTime.Now
                                 };
            mockQuestionnaireService.Setup(s => s.AnswerSetGet(It.IsAny<int>())).Returns(answerSets);
            mockQuestionnaireService.Setup(s => s.QuestionnaireList())
                                    .Returns(new List<Questionnaire>
                                                 {
                                                     new Questionnaire
                                                         {
                                                             QuestionnaireID = answerSets.QuestionnaireID,
                                                             QuestionSets = new Collection<QuestionnaireQuestionSet>()
                                                         }
                                                 });
            mockQuestionnaireService.Setup(s => s.AnswerSetList(It.IsAny<string>())).Returns(new List<AnswerSet>());

            //var mockQuestionnaireActionLetterRepository = new Mock<IQuestionnaireActionLetterRepository>();
            var mockPatientRepo = new Mock<IPatientRepository>();
            var mockPatientMedRepo = new Mock<IPatientMedicationRepository>();
            var mockGpPracticeAddressRepo = new Mock<IGpPracticeAddressRepository>();

            var sut = new DepressionService("",
                                            mockPatientRepo.Object,
                                            mockPatientMedRepo.Object,
                                            mockGpPracticeAddressRepo.Object,
                                            mockQuestionnaireService.Object);

            var result = sut.QuestionnaireActionsGet("555");
        }
    }
}
