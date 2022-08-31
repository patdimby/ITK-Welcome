using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridMvc.Pagination
{
    /// <summary>
    /// Items Processors pour affichage des items de la base 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagerGridItemsProcessorItemsCount<T> : IGridItemsProcessor<T> where T : class
    {
        private readonly IGridPager _pager;

        public PagerGridItemsProcessorItemsCount(IGridPager pager)
        {
            _pager = pager;
        }
        public IQueryable<T> Process(IQueryable<T> items)
        {
            return items;
        }
    }
}
