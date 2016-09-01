using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.DAL;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Core
{
    public class ContentSectionStatusService : IContentSectionStatusService
    {

        #region fields and constructors
        //Member variables
        private IContentSectionStatusRepository _contentSectionStatusRepository;

        //Constructors
        public ContentSectionStatusService(): this(new ContentSectionStatusRepository())
        { }

        public ContentSectionStatusService(IContentSectionStatusRepository contentSectionStatusRepository)
        {
            _contentSectionStatusRepository = contentSectionStatusRepository;
        }
        #endregion

        /// <summary>
        /// Checks database entry for given content section and studyId - to show or hide sections as appropriate.
        /// </summary>
        public bool SectionEnabled(ContentSectionTypes contentSectionTypes, string studyId)
        {
            var contentSectionStatus = _contentSectionStatusRepository.ContentSectionEnabled(contentSectionTypes.ToString(), studyId);

            if (contentSectionStatus == null)
                return true; //we have no database record for this section, so assume it will be enabled by default.
            else
                if (contentSectionStatus.Status == null)
                    return true;
                else
                    return contentSectionStatus.Status.Value;
        }

        /// <summary>
        /// Checks database entry for given content section and studyId - to get all dates that need to be disabled for that section.
        /// </summary>
        /// <param name="contentSectionType">Type of section</param>
        /// <param name="studyId">Study Identifier (can be null)</param>
        public IList<DateTime> SectionDisabledDates(ContentSectionTypes contentSectionType, string studyId)
        {
            //have to do this outside of linq query
            string contentSectionTypeStr = contentSectionType.ToString(); 

            var disabledSections = _contentSectionStatusRepository.GetAll(css => (studyId == null || css.StudyId == null || css.StudyId == studyId)
                                                                        && css.SectionName == contentSectionTypeStr
                                                                        && css.DateAffected != null
                                                                        && css.Status != null
                                                                        && !css.Status.Value);//only interested in disabled dates.

            return disabledSections.Select(section => section.DateAffected.Value).ToList();
        }
    }
}