using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DeadDog.Movies.IMDB
{
    /// <summary>
    /// Represent a search-results webpage.
    /// </summary>
    public class SearchTitlePage : IMDBPage
    {
        private string search;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTitlePage"/> class.
        /// </summary>
        /// <param name="html">The html-code associated with the <see cref="SearchTitlePage"/>.</param>
        /// <param name="request">The requested url that is associated with the <see cref="SearchTitlePage"/>.</param>
        /// <param name="response">The response url that is associated with the <see cref="SearchTitlePage"/>.</param>
        /// <param name="search">The string that was searched for.</param>
        public SearchTitlePage(string html, URL request, URL response, string search)
            : base(html, request, response)
        {
            this.search = search;
        }

        private ParsedCollection<SearchResult> res;
        /// <summary>
        /// Gets a collection of the search results.
        /// </summary>
        public ParsedCollection<SearchResult> Results
        {
            get
            {
                if (res == null)
                    res = new ParsedCollection<SearchResult>(parseResults(this.HTML, this.RequestedURL, this.ResponseURL, this.search));
                return res;
            }
        }

        private SearchResult[] parseResults(string html, URL request, URL response, string search)
        {
            List<SearchResult> results = new List<SearchResult>();

            string title = html.CutToTag("title", true);
            if (html.Contains("No results found for "))
            {
                // Do nothing, which will return an empty result set...
            }
            else if (title == "Find - IMDb")
                results.AddRange(from SearchResult s in ParseTable(html) select s);
            else if (request != response && response.Address.StartsWith("http://www.imdb.com/title/tt"))
            {
                MovieId id;
                if (MovieId.TryParse(response.Address, out id))
                {
                    MainPage mp = (base.Buffer as IMDBBuffer).ReadMain(id);
                    results.Add(new SearchResult(id, mp.Title, mp.Year, mp.MediaType, MatchType.ExactMatch));
                }
            }

            return results.ToArray();
        }

        #region SearchMethods

        //These methods should only be used when an exact match was found
        private bool ParseMediaType(string input, out MediaType value)
        {
            input = input.CutToTag("title", true);
            input = input.CutToLast('(', CutDirection.Left, true);
            input = input.CutToFirst(')', CutDirection.Right, true);

            value = MediaType.Movie;
            return true;
            //if (!input.Contains(" "))
            //{
            //    value = ResultType.None;
            //    return true;
            //}

            //input = input.CutToLast(' ', CutDirection.Right, true);
            //switch (input.ToString())
            //{
            //    case "Video Game":
            //        value = ResultType.VideoGame;
            //        return true;
            //    case "Video":
            //        value = ResultType.Video;
            //        return true;
            //    case "TV":
            //        value = ResultType.TV;
            //        return true;
            //    case "TV series":
            //        value = ResultType.TVseries;
            //        return true;
            //    default:
            //        value = ResultType.None;
            //        return false;
            //}
        }
        private bool ParseTitle(string input, out string value)
        {
            if (input.Contains("<span class=\"title-extra\">"))
            {
                string tempValue = input.CutToFirst("<span class=\"title-extra\">", CutDirection.Left, false);
                tempValue = tempValue.CutToTag("span", true);
                if (!tempValue.Contains("(original title)"))
                    return ParseStandardTitle(input, out value);

                tempValue = tempValue.CutToFirst("<i>(original title)</i>", CutDirection.Right, true);
                if (tempValue.Contains(">"))
                    tempValue = tempValue.CutToLast('>', CutDirection.Left, true);

                value = System.Web.HttpUtility.HtmlDecode(tempValue.ToString().Trim().Trim('\"'));
                return true;
            }
            else
                return ParseStandardTitle(input, out value);
        }
        private bool ParseStandardTitle(string input, out string value)
        {
            input = input.CutToTag("title", true);
            input = input.CutToLast('(', CutDirection.Right, true, 1);
            value = System.Web.HttpUtility.HtmlDecode(input.ToString()).Trim().Trim('\"');
            return true;
        }
        private bool ParseYear(string input, out int value)
        {
            input = input.CutToTag("title", true);
            input = input.CutToLast('(', CutDirection.Left, true);
            input = input.CutToFirst(')', CutDirection.Right, true);

            if (input.Contains(" "))
                value = int.Parse(input.CutToLast(' ', CutDirection.Left, true).ToString());
            else
                value = int.Parse(input.ToString());

            return true;
        }

        #endregion

        private MovieId FindId(string html)
        {
            html = html.CutToFirst("<a href=\"/title/tt", CutDirection.Left, true);
            return new MovieId(int.Parse(html.CutToFirst('/', CutDirection.Right, true)));
        }

        private IEnumerable<SearchResult> ParseTable(string html)
        {
            html = html.CutToFirst("<div class=\"findSection\">", CutDirection.Left, true);
            html = html.CutToSection("<table class=\"findList\">", "</table>", true);

            string search = "<td class=\"result_text\">";

            while (html.Contains(search))
            {
                string line = html.CutToSection(search, "</td>", true);
                html = html.CutToFirst(search, CutDirection.Left, true);

                string title = line.CutToTag("a", true);
                MovieId id = FindId(line);

                MatchType matchtype = MatchType.Other;
                if (title == this.search)
                    matchtype = MatchType.ExactMatch;

                string additionalInfo = line.CutToFirst("</a>", CutDirection.Left, true);
                Match match = Regex.Match(additionalInfo, @"\([0-9]{4,4}\)");
                int year;
                if (match.Success)
                    year = int.Parse(match.Value.Trim('(', ')'));
                else
                    continue;

                string type = additionalInfo;
                bool isEpisode = Regex.IsMatch(additionalInfo, "(TV Episode)");
                if (isEpisode)
                {
                    yield return new SearchResult(id, title, year, MediaType.TVEpisode, matchtype);
                    continue;
                }

                Match typeMatch = Regex.Match(additionalInfo, @"\([0-9]{4,4}\).*?\((?<type>.*)\)");

                if (typeMatch.Success)
                {
                    type = typeMatch.Groups["type"].Value;
                    yield return new SearchResult(id, title, year, type, matchtype);
                }
                else
                    yield return new SearchResult(id, title, year, MediaType.Movie, matchtype);
            }
        }

        [Obsolete("Change in IMDb.com search results renders this method obsolete.")]
        private IEnumerable<SearchResult> ParseTable(string html, string label)
        {
            MatchType matchType = GetMatchType(label);
            if (!html.Contains("<p><b>" + label + "</b>"))
                yield break;
            html = html.CutToFirst("<p><b>" + label + "</b>", CutDirection.Left, true);
            int count = int.Parse(html.CutToFirst("(Displaying ", CutDirection.Left, true).CutToFirst(" ", CutDirection.Right, true).ToString());

            html = html.CutToTag("table", true);
            for (int i = 0; i < count; i++)
            {
                string row = html.CutToTag("tr", true);
                yield return ParseTR(html.CutToTag("tr", true), matchType);
                html = html.CutToFirst("<tr", CutDirection.Left, true);
            }
        }
        private MatchType GetMatchType(string label)
        {
            switch (label)
            {
                case "Popular Titles": return MatchType.Popular;
                case "Titles (Exact Matches)": return MatchType.ExactMatch;
                case "Titles (Partial Matches)": return MatchType.PartialMatch;
                default:
                    return MatchType.Other;
            }
        }
        private SearchResult ParseTR(string html, MatchType match)
        {
            MovieId id = FindId(html);

            string title = HttpUtility.HtmlDecode(html.CutToTag("a", true));
            if (title.Length >= 4 && title.Substring(0, 4) == "<img")
            {
                html = html.CutToFirst("<a", CutDirection.Left, true);
                title = HttpUtility.HtmlDecode(html.CutToTag("a", true));
                html = html.CutToFirst("<a", CutDirection.Left, false);
            }
            html = html.CutToFirst("</a>", CutDirection.Left, true);
            if (html.Contains("<em>(original title)</em>"))
            {
                title = html.CutToFirst("<em>(original title)</em>", CutDirection.Right, true);
                title = title.CutToLast("<p class=\"find-aka\">", CutDirection.Left, true);
                title = HttpUtility.HtmlDecode(title);
                title = title.CutToFirst('\"', CutDirection.Left, true);
                title = HttpUtility.HtmlDecode(title.CutToLast('\"', CutDirection.Right, true));
            }
            title = title.Trim().Trim('\"');

            if (html.Contains("<p"))
                html = html.CutToFirst("<p", CutDirection.Right, true);

            int year;
            if (!int.TryParse(html.CutToSection("(", ")", true).ToString().Substring(0, 4), out year))
                year = -1;

            html = html.CutToFirst(')', CutDirection.Left, true);
            if (html.Contains("("))
            {
                string type = html.CutToSection("(", ")", true).ToString();
                return new SearchResult(id, title, year, type, match);
            }
            else
                return new SearchResult(id, title, year, MediaType.Movie, match);
        }
    }
}
