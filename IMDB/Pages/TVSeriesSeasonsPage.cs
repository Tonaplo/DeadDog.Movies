using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class TVSeriesSeasonsPage : InfoPage, IEnumerable<TVSeriesEpisodesPage>
    {
        public TVSeriesSeasonsPage(string html, URL request, URL response, MovieId id)
            : base(html, request, response, id)
        {
            this.SeasonNumbers = new ParsedCollection<int>(parseNumbers(html));
            this.episodes = new Dictionary<int, TVSeriesEpisodesPage>();
        }

        private IEnumerable<int> parseNumbers(string html)
        {
            string lastString = html.CutToFirst("<h3 id=\"episode_top\" itemprop=\"name\">", CutDirection.Left, true).CutToTag("h3", true);
            lastString = lastString.CutToLast(';', CutDirection.Left, true);
            int last = int.Parse(lastString);

            string select = html.CutToFirst("<select id=\"bySeason\"", CutDirection.Left, false).CutToTag("select", false);
            while (select.Contains("<option"))
            {
                string result = select.CutToTag("option", false).CutToSection("value=\"", "\"", true).Trim();
                int number;
                if (int.TryParse(result, out number))
                {
                    yield return number;
                    if (number >= last)
                        yield break;
                }
                select = select.CutToFirst("<option", CutDirection.Left, true);
            }
        }

        public readonly ParsedCollection<int> SeasonNumbers;
        private Dictionary<int, TVSeriesEpisodesPage> episodes;

        public TVSeriesEpisodesPage this[int season]
        {
            get
            {
                if (episodes.ContainsKey(season))
                    return episodes[season];
                else if (!SeasonNumbers.Contains(season))
                    return null;
                else
                {
                    TVSeriesEpisodesPage page = (base.Buffer as IMDBBuffer).ReadPage(Id, "episodes/_ajax?season=" + season) as TVSeriesEpisodesPage;
                    episodes.Add(season, page);
                    return page;
                }
            }
        }

        IEnumerator<TVSeriesEpisodesPage> IEnumerable<TVSeriesEpisodesPage>.GetEnumerator()
        {
            foreach (int i in SeasonNumbers)
                yield return this[i];
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (int i in SeasonNumbers)
                yield return this[i];
        }
    }
}
