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
    public class GpInterventionStart : BaseLetterTemplate
    {


        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service", "Header1");

            contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This is a Telecoaching Case Management programme designed to help patients improve their risk of cardiovascular disease.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("It consists of monthly phone calls from a Healthlines advisor for 12 months to address various aspects of the patient’s management including adherence to medication, BP control, smoking, diet and lifestyle issues.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("We aim to support the work you are doing with this patient. We will be sending you brief reports after each contact with the patient. We will also contact you if we identify any issues which may need your attention (e.g. poorly controlled BP) or if the patient would appear to be suitable for a new prescription (e.g. nicotine replacement therapy).");
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
            get { return "Intervention Start"; }
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
