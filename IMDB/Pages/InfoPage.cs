using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class InfoPage : IMDBPage
    {
        private MovieId id;
        public MovieId Id
        {
            get { return id; }
        }

        public InfoPage(string html, URL request, URL response, MovieId id)
            : base(html, request, response)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return string.Format("[InfoPage, {0}]", id.Id);
        }
    }
}
