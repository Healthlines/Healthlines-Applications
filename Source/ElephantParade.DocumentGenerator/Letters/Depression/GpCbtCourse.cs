// -----------------------------------------------------------------------
// <copyright file="DepressionQuestionnaireDocument.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters.Depression
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System.IO;
    using NHSD.ElephantParade.Domain.Models;

    /// <summary>
    /// The MigraDoc document that represents the letter.
    /// </summary>
    public class GpCbtCourse : BaseLetterTemplate 
    {
       

        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Completion of a course supported by the Healthlines Service", "Header1");

            contentSection.AddParagraph("This patient has been participating in the Healthlines Service for the last few months.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph(@"The patient has been having phone calls approximately every 2 weeks for 14 to 16 weeks from a Healthlines advisor, and we have supported them in:
•	following a course of CBT (either online using Living Life to the Full or following the book ‘Overcoming depression and low mood: a five areas approach’)
•	encouraging them to take their medication, where appropriate
•	encouraging a healthy lifestyle, including moderating alcohol and taking more exercise
•	offering them other web based resources such as the patient forum ‘Big White Wall’

The patient has now reached the end of their course of fortnightly calls. 

We will be contacting them on two more occasions, at 2 -3 monthly intervals, in order to monitor their progress and to reinforce the advice they have received so far from the Healthlines Service.  We will contact you once more when we finally discharge the patient, in about 4 – 6 months time.

We hope we have been able to help you with providing support for this patient.
");
            
            string _importantInfo = values.ContainsKey("Important Information") ? (string)values["Important Information"] : null;

            if (_importantInfo != null && _importantInfo.Trim().Length > 0)
            {
                var p = contentSection.AddParagraph("Important information for GP");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline = Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();
            fields.Add("Important Information", new LetterUserContent() { Type = typeof(string), DefaultContent = "" });
            return fields;
        }

        public override string LetterName
        {
            get { return "GP CBT Course End"; }
        }

        public override string LetterDescription
        {
            get { return "GP CBT Course End Letter"; }
        }
        public override Domain.Models.LetterTarget DefaultLetterTarget
        {
            get
            {
                return Domain.Models.LetterTarget.GP;
            }
        }
    }
}
