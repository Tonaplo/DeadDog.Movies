using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public static class SearchResultExtension
    {
        public static IEnumerable<SearchResult> MediaType(this IEnumerable<SearchResult> collection, MediaType mediatype)
        {
            return collection.Where(r => r.Type == mediatype);
        }
        public static IEnumerable<SearchResult> Year(this IEnumerable<SearchResult> collection, int min, int max)
        {
            return collection.Where(r => r.Year >= min && r.Year <= max);
        }
    }
}
