using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DeadDog.Movies.IMDB
{
    //public class DataParser
    //{
    //    private Encoding encoding = Encoding.GetEncoding("iso-8859-1");
    //    private HTMLBuffer<ParsedPage> buffer;

    //    private PersonCollection personCollection;
    //    private GenreCollection genreCollection;
    //    public PersonCollection PersonCollection
    //    {
    //        get { return personCollection; }
    //    }
    //    public GenreCollection GenreCollection
    //    {
    //        get { return genreCollection; }
    //    }

    //    public DataParser()
    //        : this(new PersonCollection(), GenreCollection.CreateStandard())
    //    {
    //    }
    //    public DataParser(PersonCollection personCollection, GenreCollection genreCollection)
    //    {
    //        if (ReferenceEquals(personCollection, null))
    //            throw new ArgumentNullException("personCollection");

    //        if (ReferenceEquals(genreCollection, null))
    //            throw new ArgumentNullException("genreCollection");

    //        this.personCollection = personCollection;
    //        this.genreCollection = genreCollection;

    //        this.buffer = new HTMLBuffer<ParsedPage>(ParseHTML);
    //        this.buffer.Encoding = encoding;
    //    }

    //    private ParsedPage ParseHTML(URL url, string html)
    //    {
    //        MovieId id;
    //        if (!MovieId.TryParse(url.Address, out id))
    //        {
    //            if (url.Address.StartsWith("http://www.imdb.com/find?"))
    //                return new HTMLPage(html);
    //            else
    //                return null;
    //        }

    //        if (url.Address.Substring(0, url.Address.Length - 8).EndsWith("title/tt"))
    //            return new MainPage(id, html, genreCollection);
    //        else if (url.Address.EndsWith("/taglines"))
    //            return new TagPage(id, html);
    //        else
    //            throw new InvalidOperationException();
    //    }

    //    public MainPage Main(MovieId id)
    //    {
    //        URL url = id.GetURL();
    //        return buffer.ReadURL(url, 1) as MainPage;
    //    }
    //    public TagPage Tags(MovieId id)
    //    {
    //        URL url = new URL(id.GetURL().Address + "taglines");
    //        return buffer.ReadURL(url, 1) as TagPage;
    //    }
    //    public CreditsPage Credits(MovieId id)
    //    {
    //        URL url = new URL(id.GetURL().Address + "fullcredits");
    //        return buffer.ReadURL(url, 1) as CreditsPage;
    //    }
    //    public SeasonPage Seasons(MovieId id)
    //    {
    //        URL url = new URL(id.GetURL().Address + "episodes");
    //        return buffer.ReadURL(url, 1) as SeasonPage;
    //    }

    //    public IEnumerable<SearchResult> Search(string text)
    //    {
    //        URL currentURL = new URL("http://www.imdb.com/find?s=tt&q=" + HttpUtility.UrlEncode(text));
    //        ParsedPage page = buffer.ReadURL(currentURL, encoding, 1);

    //        if (page is MainPage)
    //        {
    //            MainPage mp = page as MainPage;
    //            ResultType res = ResultType.None;
    //            yield return new SearchResult(mp.Id, mp.Title, mp.Year, res, MatchType.ExactMatch);
    //        }
    //        else
    //        {
    //            string html = (page as HTMLPage).HTML;
    //            foreach (SearchResult s in ParseTable(html, "Popular Titles"))
    //                yield return s;
    //            foreach (SearchResult s in ParseTable(html, "Titles (Exact Matches)"))
    //                yield return s;
    //            foreach (SearchResult s in ParseTable(html, "Titles (Partial Matches)"))
    //                yield return s;
    //        }
    //    }

    //    #region SearchMethods

    //    //These methods should only be used when an exact match was found
    //    private bool ParseResultType(string input, out ResultType value)
    //    {
    //        input = input.CutToTag("title", true);
    //        input = input.CutToLast('(', CutDirection.Left, true);
    //        input = input.CutToFirst(')', CutDirection.Right, true);

    //        if (!input.Contains(" "))
    //        {
    //            value = ResultType.None;
    //            return true;
    //        }

    //        input = input.CutToLast(' ', CutDirection.Right, true);
    //        switch (input.ToString())
    //        {
    //            case "Video Game":
    //                value = ResultType.VideoGame;
    //                return true;
    //            case "Video":
    //                value = ResultType.Video;
    //                return true;
    //            case "TV":
    //                value = ResultType.TV;
    //                return true;
    //            case "TV series":
    //                value = ResultType.TVseries;
    //                return true;
    //            default:
    //                value = ResultType.None;
    //                return false;
    //        }
    //    }
    //    private bool ParseTitle(string input, out string value)
    //    {
    //        if (input.Contains("<span class=\"title-extra\">"))
    //        {
    //            string tempValue = input.CutToFirst("<span class=\"title-extra\">", CutDirection.Left, false);
    //            tempValue = tempValue.CutToTag("span", true);
    //            if (!tempValue.Contains("(original title)"))
    //                return ParseStandardTitle(input, out value);

    //            tempValue = tempValue.CutToFirst("<i>(original title)</i>", CutDirection.Right, true);
    //            if (tempValue.Contains(">"))
    //                tempValue = tempValue.CutToLast('>', CutDirection.Left, true);

    //            value = System.Web.HttpUtility.HtmlDecode(tempValue.ToString().Trim());
    //            return true;
    //        }
    //        else
    //            return ParseStandardTitle(input, out value);
    //    }
    //    private bool ParseStandardTitle(string input, out string value)
    //    {
    //        input = input.CutToTag("title", true);
    //        input = input.CutToLast('(', CutDirection.Right, true, 1);
    //        value = System.Web.HttpUtility.HtmlDecode(input.ToString());
    //        return true;
    //    }
    //    private bool ParseYear(string input, out int value)
    //    {
    //        input = input.CutToTag("title", true);
    //        input = input.CutToLast('(', CutDirection.Left, true);
    //        input = input.CutToFirst(')', CutDirection.Right, true);

    //        if (input.Contains(" "))
    //            value = int.Parse(input.CutToLast(' ', CutDirection.Left, true).ToString());
    //        else
    //            value = int.Parse(input.ToString());

    //        return true;
    //    }

    //    #endregion

    //    private MovieId FindId(string html)
    //    {
    //        html = html.CutToFirst("<a href=\"/title/tt", CutDirection.Left, true);
    //        return new MovieId(int.Parse(html.CutToFirst('/', CutDirection.Right, true)));
    //    }

    //    private IEnumerable<SearchResult> ParseTable(string html, string label)
    //    {
    //        MatchType matchType = GetMatchType(label);
    //        if (!html.Contains("<p><b>" + label + "</b>"))
    //            yield break;
    //        html = html.CutToFirst("<p><b>" + label + "</b>", CutDirection.Left, true);
    //        int count = int.Parse(html.CutToFirst("(Displaying ", CutDirection.Left, true).CutToFirst(" ", CutDirection.Right, true).ToString());

    //        html = html.CutToTag("table", true);
    //        for (int i = 0; i < count; i++)
    //        {
    //            string row = html.CutToTag("tr", true);
    //            yield return ParseTR(html.CutToTag("tr", true), matchType);
    //            html = html.CutToFirst("<tr", CutDirection.Left, true);
    //        }
    //    }
    //    private MatchType GetMatchType(string label)
    //    {
    //        switch (label)
    //        {
    //            case "Popular Titles": return MatchType.Popular;
    //            case "Titles (Exact Matches)": return MatchType.ExactMatch;
    //            case "Titles (Partial Matches)": return MatchType.PartialMatch;
    //            default:
    //                return MatchType.Other;
    //        }
    //    }
    //    private SearchResult ParseTR(string html, MatchType match)
    //    {
    //        MovieId id = FindId(html);

    //        string title = HttpUtility.HtmlDecode(html.CutToTag("a", true));
    //        if (title.Length >= 4 && title.Substring(0, 4) == "<img")
    //        {
    //            html = html.CutToFirst("<a", CutDirection.Left, true);
    //            title = HttpUtility.HtmlDecode(html.CutToTag("a", true));
    //            html = html.CutToFirst("<a", CutDirection.Left, false);
    //        }
    //        html = html.CutToFirst("</a>", CutDirection.Left, true);
    //        if (html.Contains("<em>(original title)</em>"))
    //        {
    //            title = html.CutToFirst("<em>(original title)</em>", CutDirection.Right, true);
    //            title = title.CutToLast("<p class=\"find-aka\">", CutDirection.Left, true);
    //            title = HttpUtility.HtmlDecode(title);
    //            title = title.CutToFirst('\"', CutDirection.Left, true);
    //            title = HttpUtility.HtmlDecode(title.CutToLast('\"', CutDirection.Right, true));
    //        }

    //        if (html.Contains("<p"))
    //            html = html.CutToFirst("<p", CutDirection.Right, true);

    //        int year;
    //        if (!int.TryParse(html.CutToSection("(", ")", true).ToString().Substring(0, 4), out year))
    //            year = -1;

    //        html = html.CutToFirst(')', CutDirection.Left, true);
    //        if (html.Contains("("))
    //        {
    //            string type = html.CutToSection("(", ")", true).ToString();
    //            return new SearchResult(id, title, year, type, match);
    //        }
    //        else
    //            return new SearchResult(id, title, year, ResultType.None, match);
    //    }
    //}
}
