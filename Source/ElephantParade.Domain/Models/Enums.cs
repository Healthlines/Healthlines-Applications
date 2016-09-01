// -----------------------------------------------------------------------
// <copyright file="Enums.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace NHSD.ElephantParade.Domain.Models
{
    /// <summary>
    /// Enum used to define letter template names
    /// </summary>
    public enum LetterType
    {
        [DescriptionAttribute("GP Letter Depression - Start of Intervention")]
        DepressionGpInterventionStart,
        [DescriptionAttribute("GP Letter Depression - End of Intervention")]
        DepressionGpInterventionEnd,
        [DescriptionAttribute("GP Letter Depression - PHQ9")]
        DepressionGpPHQ9,
        [DescriptionAttribute("GP Letter Depression - PHQ9")]
        DepressionGpPHQ9UserInput,
        [DescriptionAttribute("GP Letter Depression - CBT Course End")]
        DepressionGpCbtCourse,
        [DescriptionAttribute("GP Letter Depression - Generic")]
        DepressionGpGeneric,
        [DescriptionAttribute("GP Letter Depression - Medication")]
        DepressionGpMedication,
        [DescriptionAttribute("GP Letter Depression - Suicidal Feelings")]
        DepressionGpSuicidalFeelings,
        [DescriptionAttribute("Patient Email")]
        DepressionPatientEmail,
        [DescriptionAttribute("GP Letter CVD - Start of Intervention")]
        CVDGpInterventionStart,
        [DescriptionAttribute("GP Letter CVD - BP Review")]
        CVDGpBpReview,
        [DescriptionAttribute("GP Letter CVD - BP Review (patient not on meds)")]
        CVDGpBpReviewPatientNotOnMeds,
        [DescriptionAttribute("GP Letter CVD - Continuation Note")]
        CVDGpContinuationNote,
        [DescriptionAttribute("GP Letter CVD - Generic")]
        CVDGpGeneric,
        [DescriptionAttribute("GP Letter CVD - Intervention End")]
        CVDGpInterventionEnd,
        [DescriptionAttribute("GP Letter CVD - Medication and Side Effects")]
        CVDGpMedication,
        [DescriptionAttribute("GP Letter CVD - Orlistat")]
        CVDGpOrlistat,
        [DescriptionAttribute("GP Letter CVD - Smoking")]
        CVDGpSmoking,
        [DescriptionAttribute("GP Letter CVD - Statins")]
        CVDGpStatins,
        [DescriptionAttribute("GP Letter CVD - Uncontrolled BP")]
        CVDGpUncontrolledBp,
        [DescriptionAttribute("GP Letter CVD - Blood Pressure Readings")]
        CVDGpBloodPressureReadings,
        [DescriptionAttribute("Patient Email")]
        CVDPatientEmail
    }
    
    /// <summary>
    /// specifies intented recipent for a letter
    /// </summary>
    public enum LetterTarget
    {
        Unknown,
        GP,
        Patient        
    }

    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum BPStatus
    {
        [DescriptionAttribute("Within Target")]
        WithinTarget,
        //[Display(Name = "Reading expected")]
        [DescriptionAttribute("Reading expected")]
        ReadingExpected,
        //[Display(Name = "Reading Missed")]
        [DescriptionAttribute("Reading Missed")]
        Missed,
        //[Display(Name = "Over High Warning Threshold")]
        [DescriptionAttribute("Over High Warning Threshold")]
        AboveCriticalLimit,
        //[Display(Name = "Under Low Warning Threshold")]
        [DescriptionAttribute("Under Low Warning Threshold")]
        BelowCriticalLimit,
        //[Display(Name = "Over Target Threshold")]
        [DescriptionAttribute("Over Target Threshold")]
        AboveTarget
    }
    
}
