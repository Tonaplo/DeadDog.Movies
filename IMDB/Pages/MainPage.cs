using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DeadDog;

namespace DeadDog.Movies.IMDB
{
    public abstract class MainPage : InfoPage
    {
        public MainPage(string html, URL request, URL response, MovieId id, GenreCollection genreCollection)
            : base(html, request, response, id)
        {
            this.Title = new ParsedInfo<string>(html, parseTitle);
            this.Year = new ParsedInfo<int>(html, parseYear);
            this.PosterURL = new ParsedInfo<URL>(html, parsePosterURL);

            this.IMDbRating = new ParsedInfo<Rating>(html, parseIMDBRating);
            this.MetacriticRating = new ParsedInfo<Rating>(html, parseMetacriticRating);

            this.Tagline = new ParsedInfo<string>(html, parseTagline);
            this.Plot = new ParsedInfo<string>(html, parsePlot);
            this.Runtime = new ParsedInfo<TimeSpan>(html, parseRuntime);

            this.Genres = new ParsedInfo<GenreSet>(html, (input) => parseGenres(input, genreCollection));
        }

        public static MediaType ParseMediaType(string html)
        {
            html = html.CutToFirst("<head>", CutDirection.Left, false);
            html = html.CutToTag("title", true);
            html = html.CutToBrackets(Brackets.Round, true).Trim();
            if (!html.Contains(" "))
                return MediaType.Movie;
            else
            {
                if (Regex.IsMatch(html, @".? [0-9]{4,4}"))
                    html = html.CutToLast(' ', CutDirection.Right, true).Remove(" ");
                else
                    html = html.Remove(" ");
                if (html == "I")
                    return MediaType.Movie;
#if NET3
                object o = Enum.Parse(typeof(MediaType), html, true);
                if (o is MediaType)
                    return (MediaType)o;
                else
                    return MediaType.Other;
#elif NET4
                MediaType media = MediaType.Other;
                if (Enum.TryParse(html, true, out media))
                    return media;
                else
                    return MediaType.Other;
#endif
            }
        }

        #region Parsing methods

        private bool parseTitle(string input, out string value)
        {
            if (input.Contains("<span class=\"title-extra\">"))
            {
                string title = input.CutToFirst("<span class=\"title-extra\">", CutDirection.Left, false);
                title = title.CutToTag("span", true);
                if (!title.Contains("(original title)"))
                    return parseStandardTitle(input, out value);

                title = title.CutToFirst("<i>(original title)</i>", CutDirection.Right, true);
                if (title.Contains(">"))
                    title = title.CutToLast('>', CutDirection.Left, true);

                value = decodeHTML(title.ToString().Trim()).Trim('\"');
                return true;
            }
            else if (input.Contains("<h1 class=\"header\" itemprop=\"name\">"))
            {
                input = input.CutToFirst("<h1 class=\"header\" itemprop=\"name\">", CutDirection.Left, false);
                input = input.CutToTag("h1", true);
                input = input.CutToFirst("<span", CutDirection.Right, true);

                value = decodeHTML(input.ToString().Trim()).Trim('\"');
                return true;
            }
            else
                return parseStandardTitle(input, out value);
        }
        private bool parseStandardTitle(string input, out string value)
        {
            input = input.CutToTag("title", true);
            input = input.CutToLast('(', CutDirection.Right, true, 1);
            value = decodeHTML(input.ToString().Trim()).Trim('\"');
            return true;
        }
        private bool parseYear(string input, out int value)
        {
            Regex regex = new Regex("<title>.*?(?<year>[0-9]{4,4})");
            Match m = regex.Match(input);
            if (m.Success)
                return int.TryParse(m.Groups["year"].Value, out value);
            value = 0;
            return false;
        }
        private bool parsePosterURL(string input, out URL value)
        {
            Regex regex = new Regex("id=\"img_primary\">.*?<img.*?src=\"(?<url>.*?)_S", RegexOptions.Singleline);
            Match m = regex.Match(input);
            if (m.Success)
            {
                value = new URL(m.Groups["url"].Value + "_SY5000_.jpg");
                return true;
            }
            value = null;
            return false;
        }

        private bool parseIMDBRating(string input, out Rating value)
        {
            input = input.CutToFirst("<div class=\"star-box-details\">", CutDirection.Left, true);
            input = input.CutToSection("<span itemprop=\"ratingValue\">", "</span>", true);
            float r = float.Parse(input.Replace('.', ',').ToString());
            byte b = (byte)(r * 10);

            value = new Rating("IMDb", true, 0, 100, b);
            return true;
        }
        private bool parseMetacriticRating(string input, out Rating value)
        {
            input = input.CutToFirst("<div class=\"star-box-details\">", CutDirection.Left, true);
            if (!input.Contains("<a href=\"criticreviews\">"))
            {
                value = Rating.Empty;
                return false;
            }
            input = input.CutToSection("<a href=\"criticreviews\">", "/", true);
            byte b = byte.Parse(input.ToString());

            value = new Rating("Metacritic", true, 0, 100, b);
            return true;
        }

        private bool parseTagline(string input, out string value)
        {
            input = input.CutToFirst("<h4 class=\"inline\">Taglines:</h4>", CutDirection.Left, true);
            input = input.CutToFirst("</div>", CutDirection.Right, true);
            if (input.Contains("<span"))
                input = input.CutToFirst("<span", CutDirection.Right, true);
            //parser = parser.CutToTag("p", true);
            //parser = parser.CutToFirst("<em class=\"nobr\">", CutDirection.Right, true);
            value = decodeHTML(input.ToString().Trim());
            return true;
        }
        private bool parsePlot(string input, out string value)
        {
            input = input.CutToFirst("<h2>Storyline</h2>", CutDirection.Left, true);
            input = input.CutToTag("p", true);
            if (input.Contains("<em class=\"nobr\">"))
                input = input.CutToFirst("<em class=\"nobr\">", CutDirection.Right, true);
            value = decodeHTML(input.ToString().Trim());
            return true;
        }
        private bool parseRuntime(string input, out TimeSpan value)
        {
            input = input.CutToFirst("<h4 class=\"inline\">Runtime:</h4>", CutDirection.Left, true);
            input = input.CutToTag("time", true).Trim();
            if (input.EndsWith("min"))
            {
                input = input.CutToFirst("min", CutDirection.Right, true).Trim();
                value = new TimeSpan(0, int.Parse(input), 0);
                return true;
            }
            else
                throw new NotImplementedException();
        }

        private GenreSet parseGenres(string input, GenreCollection collection)
        {
            List<Genre> genres = new List<Genre>();
            input = input.CutToFirst("<h4 class=\"inline\">Genres:</h4>", CutDirection.Left, true);
            input = input.CutToFirst("</div>", CutDirection.Right, true);
            while (input.Contains("<a"))
            {
                Genre g = collection.GetGenre(decodeHTML(input.CutToTag("a", true).ToString()));
                if (g != null)
                    genres.Add(g);
                input = input.CutToFirst("</a>", CutDirection.Left, false, 1);
            }

            return new GenreSet("IMDb", collection, genres);
        }

        #endregion

        public readonly ParsedInfo<string> Title;
        public readonly ParsedInfo<int> Year;
        public readonly ParsedInfo<URL> PosterURL;

        public readonly ParsedInfo<Rating> IMDbRating;
        public readonly ParsedInfo<Rating> MetacriticRating;

        public readonly ParsedInfo<string> Tagline;
        public readonly ParsedInfo<string> Plot;
        public readonly ParsedInfo<TimeSpan> Runtime;

        public readonly ParsedInfo<GenreSet> Genres;

        public URL GetPosterURL(int height)
        {
            if (height <= 10)
                throw new ArgumentOutOfRangeException("height");

            if (!this.PosterURL.Succes)
                return null;

            string add = this.PosterURL.Data.Address;
            return new URL(add.Substring(0, add.Length - 9) + height.ToString() + "_.jpg");
        }

        private TagPage tags = null;
        public TagPage Tags
        {
            get
            {
                if (tags == null)
                    tags = (base.Buffer as IMDBBuffer).ReadTags(this.Id);
                return tags;
            }
        }
        private CreditsPage credits = null;
        public CreditsPage Credits
        {
            get
            {
                if (credits == null)
                    credits = (base.Buffer as IMDBBuffer).ReadCredits(this.Id);
                return credits;
            }
        }

        public abstract MediaType MediaType { get; }

        public override string ToString()
        {
            return Title.Succes && Year.Succes ?
                string.Format("{0} ({1} {2})", Title.Data, MediaType.ToString(), Year.Data) :
                base.ToString();
        }
    }
}
