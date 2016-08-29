using NHSD.ElephantParade.DAL.EntityModels;

namespace NHSD.ElephantParade.DAL.Interfaces
{
    public interface IContentSectionStatusRepository : IRepository<ContentSectionStatus>
    {
        ContentSectionStatus ContentSectionEnabled(string contentSectionName, string studyId);
    }
}
