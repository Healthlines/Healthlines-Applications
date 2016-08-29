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
    public class GpInterventionStart: BaseLetterTemplate 
    {


        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Introduction to the Healthlines Service", "Header1");

            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This is a Telecoaching Case Management programme designed to support patients with long term health conditions.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is taking part in the Healthlines service because of depression.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("Through regular phone calls from a Healthlines advisor, the Healthlines Service helps patients to make use of all the resources which are available on the internet to support people in managing their health problems. The Service incorporates many of the components of long term condition management which are known to be important, including empowering patients by providing information and advice, improving access to effective treatments, optimizing the use of medication, addressing problems with medication adherence, and helping to co-ordinate care offered by different services.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("In the case of depression, the Healthlines service will particularly support patients in going through a course of cognitive behaviour therapy, (CBT). This may be delivered in an interactive online version, (Living Live to the Full) or, if the patient prefers, through a course book, (Overcoming depression and low mood, a five areas approach) with regular phone calls from the Healthlines advisor.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("The Healthlines Service is intended to support the work you are doing with this patient. We will contact you if we identify any issues which may need your attention or if the patient would appear to be suitable for a new or altered prescription.");
            contentSection.AddParagraph("");

        }



        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();            
            return fields;
        }

        public override string LetterName
        {
            get { return "GP Intervention Start"; }
        }

        public override string LetterDescription
        {
            get
            {
                return "Start of intervention";
            }
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
