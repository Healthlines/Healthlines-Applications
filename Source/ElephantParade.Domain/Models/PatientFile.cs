// -----------------------------------------------------------------------
// <copyright file="PatientFile.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PatientFile
    {
        public int FileID
        { get; set; }

        public string StudyID
        { get; set; }

        public string PatientID
        { get; set; }

        public string Filename
        { get; set; }

        public string Extension
        { get; set; }

        public long Length
        { get; set; }

        public DateTime Date
        { get;set; }
    }

    public class PatientFileData
        : PatientFile
    {
        public byte[] Data
        { get; set; }
    }
}
