using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class TagPage : ExtraInfoPage
    {
        public TagPage(string html, URL request, URL response, MovieId id)
            : base(html, request, response, id)
        {
            Taglines = new ParsedCollection<string>(parseTaglines(html));
        }

        private IEnumerable<string> parseTaglines(string input)
        {
            input = input.CutToFirst("<!-- End TOP_RHS -->", CutDirection.Left, true);
            input = input.CutToFirst("<p>", CutDirection.Left, false);
            if (!input.Contains("<hr/>"))
                yield break;
            input = input.CutToFirst("<hr/>", CutDirection.Right, false);
            while (input.Contains("<hr"))
            {
                yield return decodeHTML(input.CutToTag("p", true).ToString());
                input = input.CutToFirst("<hr", CutDirection.Left, true);
            }
        }

        public readonly ParsedCollection<string> Taglines;

        public override string ToString()
        {
            return Taglines.Count == 0 ? "[N/A]" : string.Format("[{0}] {1}", Taglines.Count, Taglines[0]);
        }
    }
}
