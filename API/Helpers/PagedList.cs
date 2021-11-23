using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items,int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            //total count 10 and page size 5 so we have 2 pages to show
            TotalPages = (int)Math.Ceiling(count/(double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            //add elements to end of list
            AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        //actual paging logic implemented here skip and take 
        public static async Task<PagedList<T>> CreatAsync(IQueryable<T> source,int pageNumber,int pageSize)
        {
            //give back no of records from database
            var count = await source.CountAsync();
            //initially pagenumber(currentpage) i.e 1-1 so skip 0 and pagesize=5 (no of records (contents)  could be 5  and take 5
            //if we are on page 2 i.e 2-1 so skip (1*5) so skip=5 and take 5
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
