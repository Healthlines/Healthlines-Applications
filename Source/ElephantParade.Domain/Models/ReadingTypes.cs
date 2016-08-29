namespace NHSD.ElephantParade.Domain.Models
{
    public enum ReadingTypes
    {
        DiastolicBP = 1,
        SystolicBP = 2,
        Weight = 3,
        BloodPressure = 4 //when separate records are not needed for high/low BP readings.
    }
}