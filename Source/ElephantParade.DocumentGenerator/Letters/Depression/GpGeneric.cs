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
    public class GpGeneric: BaseLetterTemplate 
    {
        

       
        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Information for GP", "Header1");

            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This aims to support people with long term conditions and consists of regular phone calls from a Healthlines advisor, along with helping the patient to make good use of resources available over the internet such as cognitive behaviour therapy.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("We are writing to inform you about the following issue that has arisen from our phone calls with the patient:");
            contentSection.AddParagraph("");

            string _importantInfo = values.ContainsKey("Important Information")?(string)values["Important Information"]:null;

            if (_importantInfo != null && _importantInfo.Trim().Length > 0)
            {
                var p = contentSection.AddParagraph("Important information for GP");
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
            fields.Add("Important Information", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"--(CONTENT)-- 

The patient has/has not been advised to seek other assessment/advice

We have/have not advised the patient to contact you to discuss this
"
            });
            return fields;
        }

        public override string LetterName
        {
            get { return "GP Generic"; }
        }

        public override string LetterDescription
        {
            get { return "GP Generic Letter"; }
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
