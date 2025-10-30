using Microsoft.AspNetCore.Components;

namespace StuffFinder.Blazor.Components.Pagination
{
    public partial class PaginationBox
    {
        [Parameter] public int currentPage { get; set; }
        [Parameter] public int totalPages { get; set; }
        [Parameter] public EventCallback PreviosPage { get; set; }
        [Parameter] public EventCallback NextPage { get; set; }
        [Parameter] public EventCallback<int> OnPageSelected { get; set; }
        [Parameter] public EventCallback FirstPage { get; set; }
        [Parameter] public EventCallback LastPage { get; set; }

        private IEnumerable<int> GetPageNumbers()
        {
            var pages = new List<int>();
            
            if (totalPages <= 6)
            {
                return Enumerable.Range(1, totalPages);
            }

            int start, end;

            if (currentPage <= 4)
            {
                start = 1;
                end = 7;
            }
            else if (currentPage >= totalPages - 3)
            {
                start = totalPages - 6;
                end = totalPages;
            }
            else
            {
                start = currentPage - 3;
                end = currentPage + 3;
            }

            // We have to protect 1 to totalPages here
            start = Math.Max(start, 1);
            end = Math.Min(end, totalPages);

            return Enumerable.Range(start, end - start + 1);
        }

    }
}