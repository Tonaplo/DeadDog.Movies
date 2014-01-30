using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class TVSeriesMainPage : MainPage
    {
        public TVSeriesMainPage(string html, URL request, URL response, MovieId id, GenreCollection genreCollection)
            : base(html, request, response, id, genreCollection)
        {
            LastYear = new ParsedInfo<int>(html, parseYear);
        }

        private bool parseYear(string input, out int value)
        {
            value = 0;

            input = input.CutToTag("title", true);
            input = input.CutToBrackets(Brackets.Round, true);

            if (input.Contains(" "))
                input = input.CutToLast(' ', CutDirection.Left, true);
            else
                return false;

            if (input.Contains("&ndash;"))
                input = input.CutToFirst("&ndash;", CutDirection.Left, true);
            else
                return false;

            value = int.Parse(input.ToString());

            return true;
        }
        public readonly ParsedInfo<int> LastYear;

        private TVSeriesSeasonsPage seasons = null;
        public TVSeriesSeasonsPage Seasons
        {
            get
            {
                if (seasons == null)
                    seasons = (base.Buffer as IMDBBuffer).ReadPage(Id, "episodes/_ajax") as TVSeriesSeasonsPage;
                return seasons;
            }
        }

        public override MediaType MediaType
        {
            get { return IMDB.MediaType.TVSeries; }
        }
    }
}
