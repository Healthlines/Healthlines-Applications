namespace NHSD.ElephantParade.Web.Areas.Advisor.Helpers
{
    public static class PaginationHelper
    {
        public static int GetNumberOfPages(int totalRecordCount, int recordsOnEachPage)
        {
            //get number of "full" pages.
            int pageWholeCount = (totalRecordCount / recordsOnEachPage);
            //see if there are any records left over to go on an additional page
            int pageExtraCount = (totalRecordCount % recordsOnEachPage == 0) ? 0 : 1;

            return pageWholeCount + pageExtraCount;
        }

        public static int PageIndexFromPage(int? page)
        {
            return page.HasValue ? page.Value - 1 : 0;
        }
    }
}