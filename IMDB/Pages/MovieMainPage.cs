using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class MovieMainPage : MainPage
    {
        public MovieMainPage(string html, URL request, URL response, MovieId id, GenreCollection genreCollection)
            : base(html, request, response, id, genreCollection)
        {
            
        }

        public override MediaType MediaType
        {
            get { return IMDB.MediaType.Movie; }
        }
    }
}
