// -----------------------------------------------------------------------
// <copyright file="IDocumentService.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using NHSD.ElephantParade.Domain.Models;
    using NHSD.ElephantParade.DocumentGenerator.Letters;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IDocumentService
    {
        void GenerateQuestionnairePDF(QuestionnaireResults questionnaire, Stream stream, bool closeStream, string logoImgPath);
        void GenerateBloodPressurePDF(StudyPatient patient, IList<BloodPressureReadingViewModel> bpReadings, string signatoryName, string signatoryImgPath, Stream stream, bool closeStream, string logoImgPath);
        void GenerateLetter(NHSD.ElephantParade.Domain.Models.LetterActionData letter, string signatoryName, string signatoryImgPath, StudyPatient patient, Stream stream, bool closeStream, string logoImgPath);
        LetterDetails  GetLetterDetails(LetterType letterName);
        //LetterDetails GetLetterDetails(string letterTemplate);
    }
    
}
