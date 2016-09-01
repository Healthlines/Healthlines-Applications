using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;
using NHSD.ElephantParade.DocumentGenerator.Interfaces;
using MigraDoc.Rendering;
using System.IO;
using NHSD.ElephantParade.DocumentGenerator.Questionnaire;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.DocumentGenerator.Letters;

using NHSD.ElephantParade.DocumentGenerator.Properties;
using NHSD.ElephantParade.DocumentGenerator.BloodPressureReadings;
using PdfSharp.Pdf;


namespace NHSD.ElephantParade.DocumentGenerator
{
    public class DocumentService 
        : IDocumentService
    {
        public void GenerateQuestionnairePDF(QuestionnaireResults questionnaire, Stream stream, bool closeStream, string logopath)
        {
            QuestionnaireDocument doc = new QuestionnaireDocument(questionnaire);
            Document d = doc.CreateDocument(logopath);
            //d.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
            // Set the MigraDoc document
            pdfRenderer.Document = d;
            // Create the PDF document
            pdfRenderer.RenderDocument();

            pdfRenderer.Save(stream, closeStream);
        }

        public void GenerateBloodPressurePDF(StudyPatient patient, IList<BloodPressureReadingViewModel> bpReadings, string signatoryName, string signatoryImgPath, Stream stream, bool closeStream, string logoImgPath)
        {
//            BPReadingsDocument doc = new BPReadingsDocument(bpReadings);
            BloodPressureDocument doc = new BloodPressureDocument(bpReadings.OrderByDescending(bpr => bpr.DateOfReading).ToList());

            NHSD.ElephantParade.DocumentGenerator.Letters.Address healthlinesAddress = new NHSD.ElephantParade.DocumentGenerator.Letters.Address()
            {
                Name = Resources.HealthlinesAddressName,
                Line1 = Resources.HealthlinesAddress1,
                Line2 = Resources.HealthlinesAddress2,
                Town = Resources.HealthlinesAddressTown,
                County = Resources.HealthlinesAddressCounty,
                Postcode = Resources.HealthlinesAddressPostcode
            };

            if (doc != null)
            {
                
                // Document d = doc.CreateDocument(patient, logoImgPath);
                PdfDocument d = doc.CreateDocument(patient,
                                                healthlinesAddress,
                                                signatoryName,
                                                signatoryImgPath,
                                                logoImgPath,
                                                null);



                d.Save(stream, closeStream);
            }
        }

        #region Letters
        public void GenerateLetter(NHSD.ElephantParade.Domain.Models.LetterActionData letter,string signatoryName,string signatoryImgPath, StudyPatient patient, Stream stream ,bool closeStream,string logoImgPath)
        {
            //get the letter
            //LetterType lType;
            //if (Enum.TryParse(letter.LetterTemplate, out lType) == false)
            //    throw new Exception("unknown letter");
            BaseLetterTemplate doc = GetLetter(letter.LetterTemplate);
            NHSD.ElephantParade.DocumentGenerator.Letters.Address healthlinesAddress = new NHSD.ElephantParade.DocumentGenerator.Letters.Address()
            {
                Name = Resources.HealthlinesAddressName,
                Line1 = Resources.HealthlinesAddress1,
                Line2 = Resources.HealthlinesAddress2,
                Town = Resources.HealthlinesAddressTown,
                County = Resources.HealthlinesAddressCounty,
                Postcode = Resources.HealthlinesAddressPostcode,
                Email = Resources.HealthlinesEmail ,
                Telephone = Resources.HealthlinesTelephone
            };

            if(doc!=null)
            {
                PdfDocument d = doc.CreateDocument(patient,
                                                healthlinesAddress, 
                                                signatoryName,
                                                signatoryImgPath,
                                                logoImgPath,
                                                letter.LetterValues);
                d.Save(stream, closeStream);
            }
            
        }

        public BaseLetterTemplate GetLetter(LetterType letterName)
         {
            BaseLetterTemplate letter = null;
             switch (letterName)
             {
                 case LetterType.DepressionPatientEmail:
                     break;
                 case LetterType.DepressionGpInterventionStart:
                     letter = new Letters.Depression.GpInterventionStart();                  
                     break;
                 case LetterType.DepressionGpInterventionEnd:
                     letter = new Letters.Depression.GpInterventionEnd();
                     break;
                 case LetterType.DepressionGpPHQ9:
                     letter = new Letters.Depression.GpPHQ9(false);
                     break;
                 case LetterType.DepressionGpPHQ9UserInput:
                     letter = new Letters.Depression.GpPHQ9(true);                     
                     break;
                 case LetterType.DepressionGpCbtCourse:
                     letter = new Letters.Depression.GpCbtCourse();
                     break;
                 case LetterType.DepressionGpGeneric:
                     letter = new Letters.Depression.GpGeneric();
                     break;
                 case LetterType.DepressionGpMedication:
                     letter = new Letters.Depression.GpMedication();
                     break;
                 case LetterType.DepressionGpSuicidalFeelings:
                     letter = new Letters.Depression.GpSuicidalFeelings();
                     break;
                 case LetterType.CVDGpInterventionStart:
                     letter = new Letters.CVD.GpInterventionStart();
                     break;   
                 case LetterType.CVDGpBloodPressureReadings:
                     //throw new NotImplementedException("CVDGpBloodPressureReadings");
                     //no letter as this is a
                     //letter = new BloodPressureDocument();
                     break;
                 case LetterType.CVDGpBpReview:
                     letter = new Letters.CVD.GpBpReview();
                     break;
                 case LetterType.CVDGpBpReviewPatientNotOnMeds:
                     letter = new Letters.CVD.GpBpReviewPatientNotOnMeds();
                     break;
                 case LetterType.CVDGpContinuationNote:
                     letter = new Letters.CVD.GpContinuationNote();
                     break;
                 case LetterType.CVDGpGeneric:
                     letter = new Letters.CVD.GpGeneric();
                     break;
                 case LetterType.CVDGpInterventionEnd:
                     letter = new Letters.CVD.GpInterventionEnd();
                     break;
                 case LetterType.CVDGpMedication:
                     letter = new Letters.CVD.GpMedication();
                     break;
                 case LetterType.CVDGpOrlistat:
                     letter = new Letters.CVD.GpOrlistat();
                     break;
                 case LetterType.CVDGpSmoking:
                     letter = new Letters.CVD.GpSmoking();
                     break;

                 case LetterType.CVDGpStatins:
                     letter = new Letters.CVD.GpStatins();
                     break;
                 case LetterType.CVDGpUncontrolledBp:
                     letter = new Letters.CVD.GpUncontrolledBp();
                     break;
                 case LetterType.CVDPatientEmail:
                     //letter = new Letters.CVD.CVDPatientEmail();
                     break;
                 default:
                     throw new ArgumentException("Unknown letter " + letterName.ToString());
                     break;
             }

             return letter;
         }

        

        public LetterDetails GetLetterDetails(LetterType letterName)
        {     
            LetterDetails letterDetails = null;
            BaseLetterTemplate letter = GetLetter(letterName);
            if (letter != null)
            {
                letterDetails = new LetterDetails();
                letterDetails.Description = letter.LetterDescription;
                letterDetails.LetterName = letter.LetterName;
                letterDetails.LetterTarget = letter.DefaultLetterTarget;
                letterDetails.UserFields = letter.GetFields();
                letterDetails.LetterTemplate = letterName.ToString();                
            }
            return letterDetails;
        }

        #endregion
    }
}
