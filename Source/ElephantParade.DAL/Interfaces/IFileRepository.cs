// -----------------------------------------------------------------------
// <copyright file="IFileRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DAL.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NHSD.ElephantParade.DAL.EntityModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IFileRepository : IRepository<File>
    {

        IList<File> ListByPatient(string studyID, string patientID);
    }
}
