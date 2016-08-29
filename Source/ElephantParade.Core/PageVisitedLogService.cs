using System;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Mapping;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.DAL;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.Core
{
    public class PageVisitedLogService : IPageVisitedLogService
    {
        private IPageVisitedLogRepository _pageVisitedLogRepository;

        public PageVisitedLogService(): this(new PageVisitedLogRepository())
        { }

        public PageVisitedLogService(IPageVisitedLogRepository pageVisitedLogRepository)
        {
            _pageVisitedLogRepository = pageVisitedLogRepository ?? new PageVisitedLogRepository();
        }
        
        public void Add(PageVisitedLogViewModel pageVisitedLogViewModel)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            
            var pageVisitedLog = cvmo.ConvertFromPageVisitedLogViewModel(pageVisitedLogViewModel);

            _pageVisitedLogRepository.Add(pageVisitedLog);

            _pageVisitedLogRepository.SaveChanges();
        }
    }
}
