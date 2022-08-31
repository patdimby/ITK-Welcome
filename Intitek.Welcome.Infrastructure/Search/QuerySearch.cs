using Intitek.Welcome.Infrastructure.Domain;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Infrastructure.Search
{
    public class QuerySearch<T> where T: class
    {
        public QuerySearch()
        {
            Keyword = string.Empty;
            Criterium = new Dictionary<string, object>();
        }
        public string Keyword { get; set; }
        public Dictionary<string, object> Criterium { get; set; }
    }
}