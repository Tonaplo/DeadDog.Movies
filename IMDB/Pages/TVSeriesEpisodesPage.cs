using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class TVSeriesEpisodesPage : InfoPage
    {
        public TVSeriesEpisodesPage(string html, URL request, URL response, MovieId id)
            : base(html, request, response, id)
        {
            this.SeasonNumber = new ParsedInfo<int>(html, parseSeason);
            this.Episodes = new ParsedCollection<EpisodeInfo>(parseEpisodes(html));
            this.episodePages = new Dictionary<int, TVEpisodeMainPage>();
        }

        private bool parseSeason(string html, out int number)
        {
            string lastString = html.CutToFirst("<h3 id=\"episode_top\" itemprop=\"name\">", CutDirection.Left, true).CutToTag("h3", true);
            lastString = lastString.CutToLast(';', CutDirection.Left, true);
            number = int.Parse(lastString);
            return true;
        }
        private IEnumerable<EpisodeInfo> parseEpisodes(string html)
        {
            int odd = 1;
            html = html.CutToFirst("<div class=\"list detail eplist\">", CutDirection.Left, false).CutToTag("div", true);
            while (html.Contains("<div class=\"list_item"))
            {
                string next = string.Format("<div class=\"list_item {0}\">", odd++ % 2 == 0 ? "even" : "odd");
                string episode = html.CutToFirst(next, CutDirection.Left, false).CutToTag("div", true);

                yield return new EpisodeInfo(episode);
                html = html.CutToFirst("<div class=\"list_item", CutDirection.Left, true);
            }
        }

        public readonly ParsedInfo<int> SeasonNumber;
        public readonly ParsedCollection<EpisodeInfo> Episodes;

        private Dictionary<int, TVEpisodeMainPage> episodePages;

        public TVEpisodeMainPage this[int episode]
        {
            get
            {
                if (episodePages.ContainsKey(episode))
                    return episodePages[episode];
                else
                {
                    var m = Episodes.Where(x => x.Episode == episode).Select(x => x.EpisodeId);
                    if (!m.Any())
                    {
                        episodePages.Add(episode, null);
                        return null;
                    }
                    else
                    {
                        TVEpisodeMainPage page = (base.Buffer as IMDBBuffer).ReadMain(m.First()) as TVEpisodeMainPage;
                        episodePages.Add(episode, page);
                        return page;
                    }
                }
            }
        }
    }
}
