function returnPagingAndHtml(callbacksPageUrl, callbackDivId, callbackTableDivId) {
    $.getJSON(callbacksPageUrl, null, function (d) {

        if (d != null) {
            // add the dynamic html to the div
            $(callbackDivId).append(d.Data);
            // create the footer
            createCallbacksFooter(d.TotalPageCount, d.CurrentPage, callbackTableDivId);

            $(callbackTableDivId + " tfoot a").live("click", function (evt) {
                evt.preventDefault();
                var data = {
                    page: $(this).attr("alt")
                };

                $.getJSON(callbacksPageUrl, data, function (html) {
                    // add the data to the table
                    $(callbackTableDivId).remove();
                    $(callbackDivId).append(html.Data);

                    // re-create the footer
                    createCallbacksFooter(html.TotalPageCount, html.CurrentPage, callbackTableDivId);
                });
            });
        }
    });
}

/* create and add footer to table, after tbody.
pagesOfRecords is count of all pages of records (not just the ones we're displaying)
tblId is the id of the div in the table we're adding this footer to.
*/
function createCallbacksFooter(pagesOfRecords, currentpage, tblId) {
    if (pagesOfRecords == 1) {
        return; //no need to display any paging, only 1 page of records in total
    }

    //default values - will be replaced further down...
    var pagingStart = 1; //first page on screen e.g the '1' in: 1 2 3 4 5 ..>
    var pagingEnd = 5; //last page on screen e.g the '5' in: 1 2 3 4 5 ..>

    var pagingPagesTotal = 5; //total number of pages to show e.g. 5 pages <... 4 5 6 7 8

    if (currentpage < (pagingPagesTotal)) { //we're on the first set of pages e.g. 1 2 3 4 5 ..> or 1 2 3
        pagingStart = 1;
        pagingEnd = (pagesOfRecords > pagingPagesTotal) ? pagingPagesTotal : pagesOfRecords;
    }
    else if (currentpage + Math.floor(pagingPagesTotal / 2) > pagesOfRecords) { //we're at the end e.g. where there are 35 pages in total: <... 31 32 33 34 35
        pagingEnd = pagesOfRecords;
        pagingStart = (pagingEnd - pagingPagesTotal + 1);
    }
    else { //we're in the middle somewhere
        pagingStart = currentpage - Math.floor(pagingPagesTotal / 2);
        pagingEnd = currentpage + Math.floor(pagingPagesTotal / 2);
    }

    var footer = "<tfoot><tr><td colspan='7'>";

    if (currentpage > 1) {
        var link = currentpage - 1;
        //add a previous page icon, so long as we're not on page 1
        footer = footer + "<a alt='" + link + "' href=#>&lt;...</a>&nbsp;";
    }

    //display the page numbers we want to show, and ensure the current page is not a link.
    for (var i = pagingStart; i < (pagingEnd + 1); i++) {
        if (i == currentpage) {
            footer = footer + i + "&nbsp;";
        }
        else {
            footer = footer + "<a alt='" + i + "' href=#>" + i + "</a>&nbsp;";
        }
    }

    if (pagesOfRecords > pagingPagesTotal && currentpage != pagesOfRecords) {
        var link = currentpage + 1;
        //add a next page icon, so long as we have more pages than shown on screen & we're not on the last page.
        footer = footer + "<a alt='" + link + "' href=#>...&gt;</a>&nbsp;";
    }

    footer = footer + "</td></tr></tfoot>";
    //add footer into table 
    $(tblId + " tbody").after(footer);
}
