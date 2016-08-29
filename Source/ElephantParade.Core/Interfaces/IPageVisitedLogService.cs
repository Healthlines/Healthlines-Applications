using NHSD.ElephantParade.Core.Models;

namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface IPageVisitedLogService
    {
        void Add(PageVisitedLogViewModel pageVisitedLogViewModel);
    }
}
