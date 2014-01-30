using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class EpisodeInfo
    {
        internal EpisodeInfo(string html)
        {
            this.EpisodeId = new ParsedInfo<MovieId>(html, parseID);
            this.AirDate = new ParsedInfo<DateTime>(html, parseDate);
            this.Season = new ParsedInfo<int>(html, parseSeason);
            this.Episode = new ParsedInfo<int>(html, parseEpisode);
            this.Title = new ParsedInfo<string>(html, parseTitle);
            this.Plot = new ParsedInfo<string>(html, parsePlot);
            this.PosterURL = new ParsedInfo<URL>(html, parsePosterURL);
        }

        private bool parseID(string html, out MovieId id)
        {
            html = html.CutToFirst("<div class=\"image\">", CutDirection.Left, true);
            html = html.CutToSection("href=\"", "\"", true);
            return MovieId.TryParse(html, out id);
        }
        private bool parseDate(string html, out DateTime date)
        {
            return DateTime.TryParse(html.CutToSection("<div class=\"airdate\">", "</div>", true).Trim(), out date);
        }
        private bool parseSeason(string html, out int season)
        {
            html = html.CutToSection("<div>", "</div>", true);
            return int.TryParse(html.CutToSection("S", ",", true), out season);
        }
        private bool parseEpisode(string html, out int episode)
        {
            html = html.CutToSection("<div>", "</div>", true);
            return int.TryParse(html.CutToFirst("Ep", CutDirection.Left, true), out episode);
        }
        private bool parseTitle(string html, out string title)
        {
            title = decodeHTML(html.CutToTag("strong", true).CutToTag("a", true).Trim());
            return true;
        }
        private bool parsePlot(string html, out string plot)
        {
            string tag = "<div class=\"item_description\" itemprop=\"description\">";
            plot = decodeHTML(html.CutToFirst(tag, CutDirection.Left, false).CutToTag("div", true).Trim());
            return true;
        }
        private bool parsePosterURL(string html, out URL url)
        {
            html = html.CutToSection("<img", ">", true);
            html = html.CutToSection("src=\"", "\"", true);
            if (html.Contains("/nopicture/"))
            {
                url = new URL(html);
                return false;
            }

            html = html.CutToLast("SY", CutDirection.Right, false);

            url = new URL(html.ToString() + "5000_.jpg");
            return true;
        }

        public readonly ParsedInfo<MovieId> EpisodeId;
        public readonly ParsedInfo<DateTime> AirDate;
        public readonly ParsedInfo<int> Season;
        public readonly ParsedInfo<int> Episode;
        public readonly ParsedInfo<string> Title;
        public readonly ParsedInfo<string> Plot;
        public readonly ParsedInfo<URL> PosterURL;

        private string decodeHTML(string html)
        {
            if (html == null)
                throw new ArgumentNullException("html");
            if (html == string.Empty)
                return string.Empty;

            string result;
#if NET3
            try { result = System.Web.HttpUtility.HtmlDecode(html); }
#elif NET4
            try { result = System.Net.WebUtility.HtmlDecode(html); }
#endif
            catch { result = html; }
            return result;
        }
    }
}
