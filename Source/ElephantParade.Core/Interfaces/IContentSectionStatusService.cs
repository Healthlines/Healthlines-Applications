using System;
using System.Collections.Generic;
using NHSD.ElephantParade.Domain.Models;

namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface IContentSectionStatusService
    {
        bool SectionEnabled(ContentSectionTypes contentSectionTypes, string studyId);
        IList<DateTime> SectionDisabledDates(ContentSectionTypes contentSectionType, string studyId);
    }
}
