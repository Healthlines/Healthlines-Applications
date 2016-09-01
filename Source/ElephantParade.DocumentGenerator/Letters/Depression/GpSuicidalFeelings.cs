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
    public class GpSuicidalFeelings: BaseLetterTemplate 
    {
       

        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Information regarding potential suicidal risk", "Header1");

            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This is a Telecoaching Case Management programme designed to support patients with long term health conditions. This patient is taking part in the Healthlines service because of depression.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("We are writing to inform you that this patient has expressed some feelings which suggest that they may be at risk of suicide.  This information was obtained:");
            contentSection.AddParagraph("");

           
            var p = contentSection.AddParagraph("");            
            p.Format.SpaceAfter = 10;

            string value = values.ContainsKey("Delete if not applicable - Automated Alert") ? (string)values["Delete if not applicable - Automated Alert"] : "";
            if (value != "")
            {
                p = contentSection.AddParagraph();
                p.AddCharacter(SymbolName.Bullet);
                p.AddText(value);
            }
            value = values.ContainsKey("Delete if not applicable - PHQ9") ? (string)values["Delete if not applicable - PHQ9"] : "";
            if (value != "")
            {
                p = contentSection.AddParagraph();
                p.AddCharacter(SymbolName.Bullet);
                p.AddText(value);
            }
            value = values.ContainsKey("Delete if not applicable - Discussion") ? (string)values["Delete if not applicable - Discussion"] : "";
            if (value != "")
            {
                p = contentSection.AddParagraph();
                p.AddCharacter(SymbolName.Bullet);
                p.AddText(value);             
            }

            p = contentSection.AddParagraph("");
            p.Format.SpaceAfter = 10;


            p = contentSection.AddParagraph("Please note that the Healthlines advisors are not clinicians and are not qualified to make a formal assessment of suicidal risk. ");
            p.Format.Font.Bold = true;
            p.Format.SpaceAfter = 10;

            value = values.ContainsKey("Delete if not applicable - Content") ? (string)values["Delete if not applicable - Content"] : "";
            if (value != "")
            {
                p = contentSection.AddParagraph();
                p.AddCharacter(SymbolName.Bullet);
                p.AddText(value);
                p = contentSection.AddParagraph("");
                p.Format.SpaceAfter = 10;
            }

            string _importantInfo = values.ContainsKey("Other Important Information") ? (string)values["Other Important Information"] : null;
            if (_importantInfo != null && _importantInfo.Trim().Length >0)
            {
                p = contentSection.AddParagraph("Other Important information for GP");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline = Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }

            p = contentSection.AddParagraph("Please could you assess and decide whether or not you need to take any further action in response to this information about possible suicidal risk in this patient.");
            p.Format.SpaceAfter = 10;
            p = contentSection.AddParagraph("We will be contacting the patient again in due course. If you feel that we should withdraw this patient from the Healthlines service and not continue to contact this patient please let us know within the next two weeks.");
            p.Format.SpaceAfter = 10;
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();
            fields.Add("Other Important Information", new LetterUserContent() { Type = typeof(string), DefaultContent = "" });
            fields.Add("Delete if not applicable - Automated Alert", new LetterUserContent() { Type = typeof(string), DefaultContent = "Through an automated alert generated by the Living Life to the Full programme because the patient indicated that they are experiencing suicidal feelings. At the same time, the programme offers advice to the patient according to the intensity of the suicidal feelings. The patient may or may not have followed this advice." });
            fields.Add("Delete if not applicable - PHQ9", new LetterUserContent() { Type = typeof(string), DefaultContent = "Through a response on the PHQ9 which indicates that they thought they would be better off dead, or thought about hurting themselves in some way, on several days/more than half the days/ nearly every day in the last two weeks." });
            fields.Add("Delete if not applicable - Discussion", new LetterUserContent() { Type = typeof(string), DefaultContent = "Through discussion with a Healthlines advisor." });
            fields.Add("Delete if not applicable - Content", new LetterUserContent() { Type = typeof(string), DefaultContent = @"We 		have/have not been able to contact the patient to discuss this by telephone.
This patient 	was/was not 	advised to seek other assessment/advice of/for their suicidal feelings.

We 	have/have not advised the patient to contact you to discuss their suicidal feelings, if they have not already discussed these feelings with you recently. "
            });
            return fields;
        }

        public override string LetterName
        {
            get { return "GP Suicidal Feelings"; }
        }

        public override string LetterDescription
        {
            get { return "GP Suicidal Feelings Letter"; }
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
