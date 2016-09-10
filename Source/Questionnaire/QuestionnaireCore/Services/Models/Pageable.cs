// -----------------------------------------------------------------------
// <copyright file="Pageable.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a page on objects
    /// </summary>
    public class Pageable<T> : IPageable<T>
    {
        public int PageCount { get; set; }

        public IEnumerable<T> Page { get; set; }

        public int PageIndex { get; set; }
    }

}
