using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeadDog;

namespace DeadDog.Movies.IMDB
{
    public class IMDBBuffer : HTMLBuffer<IMDBPage>
    {
        private PersonCollection personCollection;
        private GenreCollection genreCollection;
        public PersonCollection PersonCollection
        {
            get { return personCollection; }
        }
        public GenreCollection GenreCollection
        {
            get { return genreCollection; }
        }

        public IMDBBuffer()
            : base()
        {
            this.Encoding = Encoding.UTF8;
            this.personCollection = new PersonCollection();
            this.genreCollection = GenreCollection.CreateStandard();
        }
        public IMDBBuffer(string file)
            : base(file)
        {
            this.Encoding = Encoding.UTF8;
            this.personCollection = new PersonCollection();
            this.genreCollection = GenreCollection.CreateStandard();
        }

        public MainPage ReadMain(int id)
        {
            return ReadMain(new MovieId(id));
        }
        public MainPage ReadMain(MovieId movie)
        {
            URL url = movie.GetURL();
            return base.ReadURL(url) as MainPage;
        }
        public TagPage ReadTags(MovieId movie)
        {
            URL url = new URL(movie.GetURL().Address + "taglines");
            return base.ReadURL(url) as TagPage;
        }
        public CreditsPage ReadCredits(MovieId movie)
        {
            URL url = new URL(movie.GetURL().Address + "fullcredits");
            return base.ReadURL(url) as CreditsPage;
        }

        internal IMDBPage ReadPage(URL url)
        {
            return base.ReadURL(url);
        }
        internal IMDBPage ReadPage(MovieId id, string suffix)
        {
            return base.ReadURL(id.GetURL().Address + suffix);
        }

        public SearchTitlePage SearchTitle(string text)
        {
            URL url = new URL(string.Format("http://www.imdb.com/find?q={0}&s=tt", System.Web.HttpUtility.UrlEncode(text)));
            return base.ReadURL(url) as SearchTitlePage;
        }

        protected override void OnTranslateURL(TranslateURLEventArgs e)
        {
            if (e.Original.Address.StartsWith("http://www.imdb.com/title/tt") &&
                e.Original.Address.Length > 36 &&
                e.Original.Address.Substring(35, 2) == "/?")
                e.Result = new URL(e.Original.Address.Substring(0, 36));
            base.OnTranslateURL(e);
        }

        protected override IMDBPage Convert(string html, URL request, URL response)
        {
            if (!response.Address.StartsWith("http://www.imdb.com"))
                throw new InvalidOperationException();

            MediaType type;
            try
            {
                type = MainPage.ParseMediaType(html);
            }
            catch
            {
                type = MediaType.Other;
            }
            bool infoPage = request.Address.StartsWith("http://www.imdb.com/title/tt");
            bool simpleURL = infoPage && response.Address.Length == 36;
            bool searchPage = request.Address.StartsWith("http://www.imdb.com/find?q=") && request.Address.EndsWith("&s=tt");

            MovieId id;
            MovieId.TryParse(response.Address, out id);

            bool istags = html.Contains("<div id=\"taglines_content\" class=\"header\">");
            bool iscredits = html.Contains("<div id=\"fullcredits_content\" class=\"header\">");
            bool seasons = response.Address.EndsWith("/episodes/_ajax");
            bool episodes = response.Address.Contains("/episodes/_ajax?season=");

            if (infoPage)
            {
                if (istags)
                    return new TagPage(html, request, response, id);
                else if (iscredits)
                    return new CreditsPage(html, request, response, id, personCollection);
                else if (seasons)
                    return new TVSeriesSeasonsPage(html, request, response, id);
                else if (episodes)
                    return new TVSeriesEpisodesPage(html, request, response, id);
                else if (simpleURL)
                    return GetMainPage(html, request, response, id);
            }
            else if (searchPage)
            {
                return new SearchTitlePage(html, request, response, request.Address.CutToSection("find?q=", "&s=tt", true));
            }

            throw new NotImplementedException("Not supported url: \"" + request + "\".");
        }

        private MainPage GetMainPage(string html, URL request, URL response, MovieId id)
        {
            MediaType type = MainPage.ParseMediaType(html);

            switch (type)
            {
                case MediaType.Other:
                    break;
                case MediaType.Movie:
                    return new MovieMainPage(html, request, response, id, genreCollection);
                case MediaType.Video:
                    break;
                case MediaType.VideoGame:
                    break;
                case MediaType.TV:
                    break;
                case MediaType.TVEpisode:
                    return new TVEpisodeMainPage(html, request, response, id, genreCollection);
                case MediaType.TVSeries:
                    return new TVSeriesMainPage(html, request, response, id, genreCollection);
            }
            throw new NotImplementedException("Unable to load media type \"" + type + "\".");
        }
    }
}
