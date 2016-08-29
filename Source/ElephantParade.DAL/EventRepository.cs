// -----------------------------------------------------------------------
// <copyright file="FileRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NHSD.ElephantParade.DAL.Interfaces;
    using NHSD.ElephantParade.DAL.EntityModels;
    using NHSD.ElephantParade.DAL.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EventRepository :
        RepositoryBase<PatientEvent>,
        IEventRepository
    {
        public EventRepository() : this(new HealthlinesRepositoryContext()) { }

        public EventRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IList<PatientEvent> ListByPatient(string studyID, string patientID)
        {
            return (from f in this.GetQueryable()
                    where f.StudyID == studyID && f.PatientID == patientID
                    select f).ToList();
        }
    }
}
