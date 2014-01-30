using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class TVEpisodeMainPage : MainPage
    {
        public TVEpisodeMainPage(string html, URL request, URL response, MovieId id, GenreCollection genreCollection)
            : base(html, request, response, id, genreCollection)
        {
            AirDate = new ParsedInfo<DateTime>(html, parseDate);
            Episode = new ParsedInfo<int>(html, parseEpisode);
            Season = new ParsedInfo<int>(html, parseSeason);
            this.EpisodeTitle = new ParsedInfo<string>(html, parseEpisodeTitle);
            this.SeriesTitle = new ParsedInfo<string>(html, parseSeriesTitle);

            seriesID = parseSeries(html);
        }

        private MovieId parseSeries(string input)
        {
            input = input.CutToFirst("<h2 class=\"tv_header\">", CutDirection.Left, true);
            input = input.CutToTag("a", false);
            input = input.CutToSection("href=\"/title/tt", "/", true);
            return new MovieId(int.Parse(input));
        }

        private bool parseDate(string input, out DateTime value)
        {
            input = input.CutToFirst("<h1 class=\"header\" itemprop=\"name\">", CutDirection.Left, false);
            input = input.CutToTag("h1", true).CutToTag("span", true).CutToBrackets(Brackets.Round, true);
            return DateTime.TryParse(input, out value);
        }
        private bool parseEpisode(string input, out int value)
        {
            input = input.CutToFirst("<h2 class=\"tv_header\">", CutDirection.Left, true);
            input = input.CutToTag("span", true);

            input = input.CutToFirst("Episode", CutDirection.Left, true).Trim();

            return int.TryParse(input, out value);
        }
        private bool parseSeason(string input, out int value)
        {
            input = input.CutToFirst("<h2 class=\"tv_header\">", CutDirection.Left, true);
            input = input.CutToTag("span", true);

            input = input.CutToSection("Season", ",", true).Trim();

            return int.TryParse(input, out value);
        }
        private bool parseEpisodeTitle(string input, out string value)
        {
            input = input.CutToFirst("<h1 class=\"header\" itemprop=\"name\">", CutDirection.Left, false);
            input = input.CutToTag("h1", true);
            input = input.CutToFirst("<span", CutDirection.Right, true);

            value = decodeHTML(input.Trim());
            return true;
        }
        private bool parseSeriesTitle(string input, out string value)
        {
            input = input.CutToFirst("<h2 class=\"tv_header\">", CutDirection.Left, true);
            input = input.CutToTag("a", true);
            value = decodeHTML(input.Trim());
            return true;
        }

        public readonly ParsedInfo<DateTime> AirDate;
        public readonly ParsedInfo<int> Season;
        public readonly ParsedInfo<int> Episode;
        public readonly ParsedInfo<string> EpisodeTitle;
        public readonly ParsedInfo<string> SeriesTitle;

        private MovieId seriesID;
        private TVSeriesMainPage series = null;
        public TVSeriesMainPage Series
        {
            get
            {
                if (series == null)
                    series = (base.Buffer as IMDBBuffer).ReadMain(seriesID) as TVSeriesMainPage;
                return series;
            }
        }

        public override MediaType MediaType
        {
            get { return IMDB.MediaType.TVEpisode; }
        }

        public override string ToString()
        {
            return EpisodeTitle.Succes && SeriesTitle.Succes && Year.Succes ?
                string.Format("{0} ({1} episode {2})", EpisodeTitle.Data, SeriesTitle.Data, AirDate.Data.Year) :
                base.ToString();
        }
    }
}
