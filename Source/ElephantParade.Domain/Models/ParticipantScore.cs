using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NHSD.ElephantParade.Domain.Properties;
using System.ComponentModel;

namespace NHSD.ElephantParade.Domain.Models
{
    public class ParticipantScore
    {
        #region enums
        public enum ParticipantScoreIdType
        {
            Systolic,
            Diastolic,
            Smoker,
            [Description("Total/HDL Cholesterol Ratio")]
            TotalCholesterolRatio,
            QRISK,
            Diabetes,
            BMI,
            CKD,
            AF,
            [Description("On BP Treatment")]
            OnBPTreatment,
            Weight,
            Height,
            [Description("Target Systolic")]
            TargetSystolic,
            [Description("Target Diastolic")]
            TargetDiastolic
        }
        #endregion

        /// <summary>
        /// Gets or sets the identifier for the user.
        /// </summary>
        [ScaffoldColumn(true)]
        public int ParticipantId { get; set; }
        public int ParticipantScoreId { get; set; }
        public string ScoreId { get; set; }
        public string ScoreValue { get; set; }
        public DateTime ScoreDate { get; set; }
    }
}
