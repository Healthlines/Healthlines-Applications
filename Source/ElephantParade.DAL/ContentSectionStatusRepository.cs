using System;
using System.Linq;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Infrastructure;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.DAL
{
    public class ContentSectionStatusRepository : RepositoryBase<ContentSectionStatus>, IContentSectionStatusRepository
    {
        public ContentSectionStatusRepository() : this(new HealthlinesRepositoryContext()) { }

        public ContentSectionStatusRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }


        public ContentSectionStatus ContentSectionEnabled(string contentSectionName, string studyId)
        {
            var query = RepositoryContext.GetObjectSet<ContentSectionStatus>().Where
                (css => css.SectionName == contentSectionName
                && css.StudyId == studyId);

            return query.FirstOrDefault();
        }
    }
}
