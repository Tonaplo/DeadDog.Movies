using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies
{
    public class MovieInfo
    {
        private string filename;

        private MovieId id;
        private string title;
        private int year;

        public MovieInfo(MovieId id)
        {
            this.id = id;
        }

        public static void ToFile(MovieInfo movie, string filename)
        {

        }
    }
}
