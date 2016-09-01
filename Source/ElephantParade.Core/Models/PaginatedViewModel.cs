using System.Collections.Generic;

namespace NHSD.ElephantParade.Core.Models
{
    public class PaginatedViewModel<T>
    {
        public IList<T> Data { get; set; }
        public int TotalRecordsCount { get; set; }
    }
}