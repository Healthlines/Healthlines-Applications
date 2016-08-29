// -----------------------------------------------------------------------
// <copyright file="GpInterventionStart.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters.CVD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MigraDoc.DocumentObjectModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GpMedication : BaseLetterTemplate
    {
        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service", "Header2");
            contentSection.AddParagraph("Information for GP re: medication problems", "Header2");

            contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This aims to support people with long term conditions and consists of regular phone calls from a Healthlines advisor, along with helping the patient to make good use of resources available over the internet. For patients with hypertension (and who do not have atrial fibrillation) we also monitor their home BP readings.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("As part of this service, we regularly assess the patient’s use of medication. This has raised the following issues:");
            var p = contentSection.AddParagraph("");

            string value = values.ContainsKey("Side Effects") ? (string)values["Side Effects"] : string.Empty;
            if (value != string.Empty)
            {
                p.Format.SpaceAfter = 10;
                p = contentSection.AddParagraph();
                p.AddText(value);
                p.Format.LeftIndent = "15";
            }

            value = values.ContainsKey("Medication Issues") ? (string)values["Medication Issues"] : string.Empty;
            if (value != string.Empty)
            {
                p.Format.SpaceAfter = 10;
                p = contentSection.AddParagraph();
                p.AddText(value);
                p.Format.LeftIndent = "15";
            }

            string[] problems = values.ContainsKey("Problems/Difficulty") ? (string[])values["Problems/Difficulty"] : new string[] { string.Empty };
            if (problems[0] != string.Empty)
            {
                p = contentSection.AddParagraph("The patient is having difficulty taking their medication as prescribed, as described below:");
                p.Format.SpaceAfter = 10;
                
                foreach (var item in problems)
                {
                    p = contentSection.AddParagraph();
                    p.AddText(item);
                    p.Format.LeftIndent = "15";
                }
            }

            if (values.ContainsKey("StoppedTakingOnOwn") && values["StoppedTakingOnOwn"] != null && ((string)values["StoppedTakingOnOwn"]).Trim() != string.Empty)
            {
                p.AddText("Stopped taking on own- " + (string)values["StoppedTakingOnOwn"] + ". ") ;
            }
            if (values.ContainsKey("DoctorAware") && values["DoctorAware"] != null && ((string)values["DoctorAware"]).Trim() != string.Empty)
            {
                p.AddText("Doctor is aware of this- " + (string)values["DoctorAware"] + ". ");
            }

            value = values.ContainsKey("Pregnancy") ? (string)values["Pregnancy"] : string.Empty;
            if (value != string.Empty)
            {
                p.Format.SpaceAfter = 10;
                p = contentSection.AddParagraph();
                p.AddText(value);
                p.Format.LeftIndent = "15";
            }

            p = contentSection.AddParagraph();
            p = contentSection.AddParagraph("");
            p = contentSection.AddParagraph(@"We have advised the patient to contact you and we would be grateful if you would review their medication.");

            string _importantInfo = values.ContainsKey("Important Information") ? (string)values["Important Information"] : string.Empty;

            if (_importantInfo.Trim() != string.Empty)
            {
                contentSection.AddParagraph();
                p = contentSection.AddParagraph("Other Important information for GP");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline = Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }

            contentSection.AddParagraph();
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();
            fields.Add("Side Effects", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"The patient is experiencing side effects from their medication as described below:"
            });
            
            fields.Add("Medication Issues", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"•   Patient is unsure what medication they are supposed to be taking, or why they are taking them. 

•   The patient is not taking their medication as prescribed.

•   The patient is having difficulty taking their medication as prescribed, as described below:"
            });
            fields.Add("Pregnancy", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"•    Patient has informed us that they are or could be pregnant. We have advised them to contact you to discuss whether any of their prescriptions need to be changed or stopped."
            });
            fields.Add("Important Information", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            return fields;
        }

        public override string LetterName
        {
            get { return "GP Medication"; }
        }

        public override string LetterDescription
        {
            get { return "GP Medication and Side Effects"; }
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
