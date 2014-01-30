using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class ExtraInfoPage : InfoPage
    {
        public ExtraInfoPage(string html, URL request, URL response, MovieId id)
            : base(html, request, response, id)
        {
            this.Title = new ParsedInfo<string>(html, parseStandardTitle);
        }

        public readonly ParsedInfo<string> Title;

        private bool parseStandardTitle(string input, out string value)
        {
            input = input.CutToTag("title", true);
            input = input.CutToLast('(', CutDirection.Right, true, 1);
            value = decodeHTML(input.ToString());
            return true;
        }

        private MainPage main = null;
        public MainPage MainPage
        {
            get
            {
                if (main == null)
                    main = (base.Buffer as IMDBBuffer).ReadMain(this.Id);
                return main;
            }
        }

        public override string ToString()
        {
            return Title.Succes ? string.Format("[Extra] {0}", Title.Data) : base.ToString();
        }
    }
}
