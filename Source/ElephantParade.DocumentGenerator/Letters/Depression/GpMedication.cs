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
    public class GpMedication: BaseLetterTemplate 
    {
        

       
        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Information for GP re medication", "Header1");

            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This aims to support people with long term conditions and consists of regular phone calls from a Healthlines advisor, along with helping the patient to make good use of resources available over the internet such as cognitive behaviour therapy.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("As part of this service, we regularly assess the patient’s use of medication. This has raised the following issues: ");
            var p = contentSection.AddParagraph("");

            //p = contentSection.AddParagraph("");
            string value = values.ContainsKey("Side-effects") ? (string)values["Side-effects"] : "";
           
            if (value != "")
            {
                contentSection.AddParagraph("Side Effects", "Header3");
                p = contentSection.AddParagraph("The patient is experiencing side-effects from their medication as described below:");
                p.Format.SpaceAfter = 10;
                p = contentSection.AddParagraph();
                
                p.AddText(value);
                p.Format.LineSpacing = "100";                
                p.Format.LeftIndent = "15";
                //p = contentSection.AddParagraph("");
            }
            value = values.ContainsKey("Problems/Difficulty") ? (string)values["Problems/Difficulty"] : "";
            if (value != "")
            {
                contentSection.AddParagraph("Problems / Difficulty", "Header3");
                p = contentSection.AddParagraph("The patient is having difficulty taking their medication as prescribed, as described below:");
                p.Format.SpaceAfter = 10;
                p = contentSection.AddParagraph();
                p.AddText(value);              
                p.Format.LeftIndent = "15";
                //p = contentSection.AddParagraph("");
            }
            value = values.ContainsKey("Pregnancy") ? (string)values["Pregnancy"] : "";
            if (value != "")
            {
                contentSection.AddParagraph("Pregnancy", "Header3");
                //p = contentSection.AddParagraph("The patient is having difficulty taking their medication as prescribed, as described below:");
                p.Format.SpaceAfter = 10;
                p = contentSection.AddParagraph();
                p.AddText(value);
                p.Format.LeftIndent = "15";
                //p = contentSection.AddParagraph("");
            }
            p = contentSection.AddParagraph("");
            p = contentSection.AddParagraph(@"We have advised the patient to contact you and we would be grateful if you would review their medication. 

You may find the following link for further advice about prescribing antidepressants helpful:");
            p.AddFormattedText("http://prodigy.clarity.co.uk/depression/prescribing_information/prescribing_information", new Font() { Size = 9, Color = Colors.Blue,Italic=true });
            p.Format.SpaceAfter = 10;
            string _importantInfo = values.ContainsKey("Important Information")?(string)values["Important Information"]:"";

            if (_importantInfo.Trim()!="")
            {
                p = contentSection.AddParagraph("Important information for GP");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline=  Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }
           
                        
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();

            fields.Add("Side-effects", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            fields.Add("Problems/Difficulty", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"Patient has been taking antidepressants for several weeks but their depression does not seem to be improving. Their medication may need to be changed or increased.

Patient is unsure what medication they are supposed to be taking, or why they are taking them. 

The patient is not taking their medication as prescribed.
"
            });
            fields.Add("Pregnancy", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"Patient has informed us that they are or could be pregnant, and they are taking antidepressants. We have advised them to contact you to discuss whether these need to be changed or stopped.
"
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
            get { return "GP Medication Letter"; }
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
