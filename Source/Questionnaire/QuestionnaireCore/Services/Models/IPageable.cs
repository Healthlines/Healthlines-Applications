using System;
namespace Questionnaires.Core.Services.Models
{
    /// <summary>
    /// represents a page of objects 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPageable<T>
    {
        System.Collections.Generic.IEnumerable<T> Page { get; set; }
        int PageCount { get; set; }
        int PageIndex { get; set; }
    }
}
