using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.Web.Areas.Administration.Models;
using OfficeOpenXml;

namespace NHSD.ElephantParade.Web.Areas.Administration.Helpers
{
    /// <summary>
    /// Reads an excel spread to create a a list of patients to import 
    /// </summary>
    public class StudyReaderXlsx
    {
        const int StartRow = 1;
        private Dictionary<string, int> _columnsPos = new Dictionary<string, int>();
        private readonly String[] _columnNames = { "condition", "trial id", "study site", "nhs number", "title", "first name", 
                                        "surname", "date of birth","participant address 1","participant address 2",
                                        "participant address 3","participant postcode","home phone no",
                                        "mobile phone no","other phone no","e-mail address","contact preference",
                                        "best contact times","pct","gp name","gp email address",
                                        "gp practice name","gp practice address 1","gp practice address 2",
                                        "gp practice address 3","gp postcode","gp practice phone number",
                                        "date consent form received","consent status","education attainment",
                                        "gender","ethnicity","referral date","notes","baseline phq9 (depression)",
                                        "baseline gad7 (anxiety)","on antidepressants","baseline qrisk score",
                                        "baseline height","baseline weight","baseline bmi",
                                        "on bp meds","basline systolic bp","basline diastolic bp",
                                        "baseline smoking status","total cholesterol/hdl ratio",
                                        "diabetes","chronic kidney disease","atrial fibrillation (af)",
                                        "target systolic bp","target diastolic bp"};


        private string cvdStudyId;
        private string depressionStudyId;
        public StudyReaderXlsx(string depressionStudyId,string cvdStudyId)
        {
            this.cvdStudyId = cvdStudyId;
            this.depressionStudyId = depressionStudyId;
        }
        /// <summary>
        /// Validates the supplied worksheet contains the correct columns required for import
        /// </summary>
        /// <param name="currentWorksheet"></param>
        /// <returns></returns>
        private void ValidateSpreadSheetColums(ExcelWorksheet currentWorksheet)
        {
            _columnsPos.Clear();
            for (int colNumber = 1; colNumber <= currentWorksheet.Dimension.End.Column; colNumber++)
            {
                if (currentWorksheet.Cells[StartRow, colNumber].Value!=null 
                    && _columnNames.Contains(((string)currentWorksheet.Cells[StartRow, colNumber].Value).ToLower()))
                    _columnsPos.Add(((string)currentWorksheet.Cells[StartRow, colNumber].Value).ToLower(), colNumber);
            }
            if (_columnsPos.Count() != _columnNames.Count())
            {
                var t = _columnNames.Select(c => c).Except(_columnsPos.Select(k=>k.Key)).ToArray();
                throw new InvalidWorkBook("Missing columns: " + String.Join(", ",t));
            }
        }


        /// <summary>
        /// Read and validate the spread sheet
        /// </summary>
        /// <param name="uploadedDoc"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public IList<PatientImportData> Read(Stream uploadedDoc, out List<Exception> errors)
        {
            IList<PatientImportData> patientImportDataList = new List<PatientImportData>();           
            errors = new List<Exception>();
            try
            {
                using (var package = new ExcelPackage(uploadedDoc))
                {
                    ExcelWorkbook workBook = package.Workbook;
                    if (workBook == null)
                        throw new InvalidWorkBook();

                    if (workBook.Worksheets.Count == 0)
                        throw new InvalidWorkBook("no worksheets found");
                    
                    ExcelWorksheet currentWorksheet = workBook.Worksheets.First();
                            
                    //Validate the format of the worksheet
                    ValidateSpreadSheetColums(currentWorksheet);
                    
                    // read each row from the start of the data (start row + 1 header row) to the end of the spreadsheet.
                    for (int rowNumber = StartRow + 1; rowNumber <= currentWorksheet.Dimension.End.Row; rowNumber++)
                    {
                        if (!isEmptyRow(currentWorksheet, rowNumber))
                        {
                            List<Exception> entryErrors;
                            PatientImportData patientImportData = ReadDataOnExcelSheet(currentWorksheet, rowNumber,
                                                                                       out entryErrors);

                            if (entryErrors.Count == 0)
                                patientImportDataList.Add(patientImportData);
                            else
                                errors.AddRange(entryErrors);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }

            return patientImportDataList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentWorksheet"></param>
        /// <param name="rowNumber"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private PatientImportData ReadDataOnExcelSheet(ExcelWorksheet currentWorksheet, int rowNumber, out List<Exception> errors)
        {
            
            errors = new List<Exception>();
            
            //SY ToDo map to _columnNames index instead of using the string key? This would increase speed but reduce readability
            #region Read Columns

            object col2Value = currentWorksheet.Cells[rowNumber, _columnsPos["condition"]].Value;
            object col3Value = currentWorksheet.Cells[rowNumber, _columnsPos["trial id"]].Value;
            object col4Value = currentWorksheet.Cells[rowNumber, _columnsPos["study site"]].Value;
            object col5Value = currentWorksheet.Cells[rowNumber, _columnsPos["nhs number"]].Value;
            object col6Value = currentWorksheet.Cells[rowNumber, _columnsPos["title"]].Value;
            object col7Value = currentWorksheet.Cells[rowNumber, _columnsPos["first name"]].Value;
            object col8Value = currentWorksheet.Cells[rowNumber, _columnsPos["surname"]].Value;
            object col9Value = currentWorksheet.Cells[rowNumber, _columnsPos["date of birth"]].Value;
            object col10Value = currentWorksheet.Cells[rowNumber, _columnsPos["participant address 1"]].Value;
            object col11Value = currentWorksheet.Cells[rowNumber, _columnsPos["participant address 2"]].Value;
            object col12Value = currentWorksheet.Cells[rowNumber, _columnsPos["participant address 3"]].Value;
            object col13Value = currentWorksheet.Cells[rowNumber, _columnsPos["participant postcode"]].Value;
            object col14Value = currentWorksheet.Cells[rowNumber, _columnsPos["home phone no"]].Value;
            object col15Value = currentWorksheet.Cells[rowNumber, _columnsPos["mobile phone no"]].Value;
            object col16Value = currentWorksheet.Cells[rowNumber, _columnsPos["other phone no"]].Value;
            object colEmailValue = currentWorksheet.Cells[rowNumber, _columnsPos["e-mail address"]].Value;
            object col18Value = currentWorksheet.Cells[rowNumber, _columnsPos["contact preference"]].Value;
            object col19Value = currentWorksheet.Cells[rowNumber, _columnsPos["best contact times"]].Value;
            object col20Value = currentWorksheet.Cells[rowNumber, _columnsPos["pct"]].Value;
            object col21Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp name"]].Value;
            object col22Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp email address"]].Value;
            object col23Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp practice name"]].Value;
            object col24Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp practice address 1"]].Value;
            object col25Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp practice address 2"]].Value;
            object col26Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp practice address 3"]].Value;
            object col27Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp postcode"]].Value;
            object col28Value = currentWorksheet.Cells[rowNumber, _columnsPos["gp practice phone number"]].Value;
            object col29Value = currentWorksheet.Cells[rowNumber, _columnsPos["date consent form received"]].Value;
            object col30Value = currentWorksheet.Cells[rowNumber, _columnsPos["consent status"]].Value;
            object col31Value = currentWorksheet.Cells[rowNumber, _columnsPos["education attainment"]].Value;
            object col32Value = currentWorksheet.Cells[rowNumber, _columnsPos["gender"]].Value;
            object col33Value = currentWorksheet.Cells[rowNumber, _columnsPos["ethnicity"]].Value;
            object col34Value = currentWorksheet.Cells[rowNumber, _columnsPos["referral date"]].Value;
            object col35Value = currentWorksheet.Cells[rowNumber, _columnsPos["notes"]].Value;
            object col36Value = currentWorksheet.Cells[rowNumber, _columnsPos["baseline phq9 (depression)"]].Value;
            object col37Value = currentWorksheet.Cells[rowNumber, _columnsPos["baseline gad7 (anxiety)"]].Value;
            object col38Value = currentWorksheet.Cells[rowNumber, _columnsPos["on antidepressants"]].Value;
            object col39Value = currentWorksheet.Cells[rowNumber, _columnsPos["baseline qrisk score"]].Value;
            object col40Value = currentWorksheet.Cells[rowNumber, _columnsPos["baseline height"]].Value;
            object col41Value = currentWorksheet.Cells[rowNumber, _columnsPos["baseline weight"]].Value;
            object col42Value = currentWorksheet.Cells[rowNumber, _columnsPos["baseline bmi"]].Value;
            object col43Value = currentWorksheet.Cells[rowNumber, _columnsPos["on bp meds"]].Value;
            object col44Value = currentWorksheet.Cells[rowNumber, _columnsPos["basline systolic bp"]].Value;
            object col45Value = currentWorksheet.Cells[rowNumber, _columnsPos["basline diastolic bp"]].Value;
            object col46Value = currentWorksheet.Cells[rowNumber, _columnsPos["baseline smoking status"]].Value;
            object col47Value = currentWorksheet.Cells[rowNumber, _columnsPos["total cholesterol/hdl ratio"]].Value;
            object col48Value = currentWorksheet.Cells[rowNumber, _columnsPos["diabetes"]].Value;
            object col49Value = currentWorksheet.Cells[rowNumber, _columnsPos["chronic kidney disease"]].Value;
            object col50Value = currentWorksheet.Cells[rowNumber, _columnsPos["atrial fibrillation (af)"]].Value;
            object col51Value = currentWorksheet.Cells[rowNumber, _columnsPos["target systolic bp"]].Value;
            object col52Value = currentWorksheet.Cells[rowNumber, _columnsPos["target diastolic bp"]].Value;

            //check the consent status
            int colConsentStatusValue = 0;
            try
            {
                colConsentStatusValue = Convert.ToInt32(col30Value);
            }
            catch
            {
                //do nothing as validated later
            }

            //Check for a study site represented by the enum value or the site name
            String studySite = String.Empty;
            if (col4Value is String)
            {
                int studySiteId;
                studySite = (string)col4Value;
                if (int.TryParse(studySite, out studySiteId))
                    col4Value = studySiteId;
            }
            try
            {
                if (Convert.ToInt32(col4Value) == 1)
                    studySite = "Bristol";
                else if (Convert.ToInt32(col4Value) == 2)
                    studySite = "Sheffield";
                else if (Convert.ToInt32(col4Value) == 3)
                    studySite = "Southampton";
            }
            catch { //do nothing validated later 
            }
            bool? onAntidepressants = null;
            if (col38Value is String)
            {
                if (((String)col38Value).Trim().ToLower() == "yes")
                    onAntidepressants = true;
                if (((String)col38Value).Trim().ToLower() == "no")
                    onAntidepressants = false;
            }
            else
            {
                try
                {
                    onAntidepressants = Convert.ToBoolean(col38Value);
                }
                catch
                {
                    //do nothing as we will validate later
                }
            }
            string studyID = String.Empty;
            if (col2Value != null)
            {
                if (Convert.ToInt32(col2Value) == 1)
                    studyID = "Depression";
                else if (Convert.ToInt32(col2Value) == 2)
                    studyID = "CVD";
            }
            decimal baselineSystolicBP = 0;
            if (col44Value != null)
            {
                try
                {
                    baselineSystolicBP = Convert.ToInt32(col44Value);
                }
                catch
                {
                }
            }
            decimal baselineDiastolicBP = 0;
            if (col45Value != null)
            {
                try
                {
                    baselineDiastolicBP = Convert.ToInt32(col45Value);
                }
                catch
                {
                }
            }
            decimal targetSystolic = 0;
            if (col51Value != null)
            {
                try
                {
                    targetSystolic = Convert.ToInt32(col51Value);
                }
                catch
                {
                }
            }
            decimal targetDiastolic = 0;
            if (col52Value != null)
            {
                try
                {
                    targetDiastolic = Convert.ToInt32(col52Value);
                }
                catch
                {
                }
            }
            decimal totalCholesterolRatio = 0;
            if (col47Value != null)
            {
                try
                {
                    totalCholesterolRatio = Convert.ToInt32(col47Value);
                }
                catch
                {
                }
            }
            #endregion
            
            #region Populate models
            var studyPatient = new StudyPatient
                                   {
                                       StudyID =studyID,
                                       StudyTrialNumber = (col3Value != null ? col3Value.ToString() : ""),
                                       StudySite = studySite,
                                       NhsNumber = (col5Value != null ? col5Value.ToString() : ""),
                                       Title = (col6Value != null ? col6Value.ToString() : ""),
                                       Forename = (col7Value != null ? col7Value.ToString() : ""),
                                       Surname = (col8Value != null ? col8Value.ToString() : ""),
                                       DOB = (col9Value != null ? Convert.ToDateTime(col9Value) : DateTime.MinValue),

                                       Address = new Address()
                                                     {
                                                         Line1 = (col10Value != null ? col10Value.ToString() : ""),
                                                         Line2 = (col11Value != null ? col11Value.ToString() : ""),
                                                         County = (col12Value != null ? col12Value.ToString() : ""),
                                                         PostCode = (col13Value != null ? col13Value.ToString() : "")
                                                     },

                                       TelephoneNumber = (col14Value != null ? col14Value.ToString() : ""),
                                       TelephoneNumberMobile = (col15Value != null ? col15Value.ToString() : ""),
                                       TelephoneNumberOther = (col16Value != null ? col16Value.ToString() : ""),
                                       Email = (colEmailValue != null ? colEmailValue.ToString() : ""),
                                       PreferredContactNumber =
                                           (col18Value != null
                                                ? (StudyPatient.PreferredContactNumberType) Convert.ToInt32(col18Value)
                                                : StudyPatient.PreferredContactNumberType.Home),
                                       PrederredContactTime = (col19Value != null ? col19Value.ToString() : ""),

                                       GPPractice = new GeneralPractitioner()
                                                        {
                                                            PrimaryCareTrust =
                                                                (col20Value != null ? col20Value.ToString() : ""),
                                                            Practice = (col21Value != null ? col21Value.ToString() : ""),
                                                            EmailAddress =
                                                                (col22Value != null ? col22Value.ToString() : ""),
                                                            Name = (col23Value != null ? col23Value.ToString() : ""),

                                                            Address = new Address()
                                                                          {
                                                                              Line1 =
                                                                                  (col24Value != null
                                                                                       ? col24Value.ToString()
                                                                                       : ""),
                                                                              Line2 =
                                                                                  (col25Value != null
                                                                                       ? col25Value.ToString()
                                                                                       : ""),
                                                                              County =
                                                                                  (col26Value != null
                                                                                       ? col26Value.ToString()
                                                                                       : ""),
                                                                              PostCode =
                                                                                  (col27Value != null
                                                                                       ? col27Value.ToString()
                                                                                       : "")
                                                                          },

                                                            TelephoneNumber =
                                                                (col28Value != null ? col28Value.ToString() : ""),
                                                        },
                                       StudyConsentedDate = (col29Value != null ? Convert.ToDateTime(col29Value) : DateTime.MinValue),

                                       Education = "Declined to answer",

                                       Gender = (col32Value != null &&
                                                 Convert.ToInt32(col32Value) == 1 ? "Male" :
                                                                                    "Female"),

                                       Ethnicity = (col33Value != null &&
                                                   Convert.ToInt32(col33Value) == 1 ? "White" :
                                                   Convert.ToInt32(col33Value) == 2 ? "Mixed" :
                                                   Convert.ToInt32(col33Value) == 3 ? "Asian or Asian British" :
                                                   Convert.ToInt32(col33Value) == 4 ? "Black or Black British" :
                                                   Convert.ToInt32(col33Value) == 5 ? "Other Ethnic Group" :
                                                   "Decline"),

                                       StudyReferralDate = (col34Value != null ? Convert.ToDateTime(col34Value) : DateTime.MinValue),
                                       BaselinePHQ9 = (col36Value != null ? Convert.ToDecimal(col36Value) : 0),
                                       BaselineGAD7 = (col37Value != null ? Convert.ToDecimal(col37Value) : 0),
                                       OnAntidepressants = (col38Value != null && Convert.ToBoolean(col38Value)),
                                       RegisteredDate = DateTime.Today,
                                       Status = StudyPatient.PatientStudyStatusType.Active
                                   };


            
                

            //create the import data object
            var patientImportData = new PatientImportData(depressionStudyId,cvdStudyId)
                        {
                            Patient = studyPatient,
                            Notes = (col35Value != null ? col35Value.ToString() : ""),
                            BaselineQriskScore = (col39Value != null ? col39Value.ToString() : ""),
                            BaselineHeight = (col40Value != null ? col40Value.ToString() : ""),
                            BaselineWeight = (col41Value != null ? col41Value.ToString() : ""),
                            BaselineBMI = (col42Value != null ? col42Value.ToString() : ""),
                            OnBPMeds = (col43Value != null ? col43Value.ToString() : ""),
                            BaselineSystolicBP = baselineSystolicBP,
                            BaselineDiastolicBP = baselineDiastolicBP,
                            BaselineSmokingStatus = (col46Value != null ? col46Value.ToString() : ""),
                            TotalCholesterolRatio = totalCholesterolRatio,
                            Diabetes = (col48Value != null ? col48Value.ToString() : ""),
                            ChronicKidneyDisease = (col49Value != null ? col49Value.ToString() : ""),
                            AtrialFibrillation = (col50Value != null ? col50Value.ToString() : ""),
                            TargetSystolic = targetSystolic,
                            TargetDiastolic = targetDiastolic
                        };

            #endregion
            
            #region Validate Spreadsheet Specific Values

            // If Consent Status is not set to valid
            if (colConsentStatusValue != 1)
                errors.Add(new InvalidEntry(rowNumber, "Invalid Consent Status"));

            if (onAntidepressants.HasValue == false)
                errors.Add(new InvalidEntry(rowNumber, "Invalid value for On antidepressants"));

            #endregion

            #region Validate models
            //validate the import data Object. This will also validate the Patient object
            var context = new ValidationContext(patientImportData, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(patientImportData, context, results);
            if (!isValid)
            {
                foreach (var validationResult in results)
                {
                    errors.Add(new InvalidEntry(rowNumber, validationResult.ErrorMessage));
                }
                patientImportData = null;
            }
            #endregion
            
            return patientImportData;
        }

        private bool isEmptyRow(ExcelWorksheet currentWorksheet, int rowNumber)
        {

            foreach (var col in _columnsPos)
            {
                if (currentWorksheet.Cells[rowNumber, col.Value].Value != null)
                {
                    var s = currentWorksheet.Cells[rowNumber, col.Value].Value as string;
                    if (s != null)
                    {
                        if (!String.IsNullOrWhiteSpace(s))
                            return false;
                    }
                    else
                        return false;
                }
            }
            
            return true;
        }


        #region ExceptionClasses
        [Serializable]
        public class InvalidWorkBook : Exception
        {
            public InvalidWorkBook()
            {
            }

            public InvalidWorkBook(string message) : base(message)
            {
            }

            public InvalidWorkBook(string message, Exception inner) : base(message, inner)
            {
            }

            protected InvalidWorkBook(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }

        [Serializable]
        public class InvalidEntry : Exception
        {
            protected int _rowNumber;

            public int RowNumber { get { return _rowNumber; } }

            public InvalidEntry(int rowNumber)
            {
                _rowNumber = rowNumber;
            }

            public InvalidEntry(int rowNumber,string message)
                : base(message)
            {
                _rowNumber = rowNumber;
            }

            public InvalidEntry(int rowNumber,string message, Exception inner)
                : base(message, inner)
            {
                _rowNumber = rowNumber;
            }

            protected InvalidEntry(
                SerializationInfo info,
                StreamingContext context)
                : base(info, context)
            {
            }
        }
        #endregion
    }
}