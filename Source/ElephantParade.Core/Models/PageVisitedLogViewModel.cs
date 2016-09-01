using System;

namespace NHSD.ElephantParade.Core.Models
{
    public class PageVisitedLogViewModel
    {
        public string Username { get; set; }
        public string PageURL { get; set; }
        public string IPAddress { get; set; }
        public DateTime DateVisited { get; set; }
    }
}
