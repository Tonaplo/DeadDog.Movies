using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class SearchResult
    {
        private MovieId id;
        private string title;
        private int year;
        private MediaType type;
        private MatchType match;

        public SearchResult(MovieId id, string title, int year, MediaType type, MatchType match)
        {
            this.id = id;
            this.title = title;
            this.year = year;
            this.type = type;
            this.match = match;
        }
        public SearchResult(MovieId id, string title, int year, string type, MatchType match)
            : this(id, title, year, ParseResultType(type.ToLower()), match)
        {
        }

        private static MediaType ParseResultType(string text)
        {
            switch (text)
            {
                case "i":
                case "movie":
                    return MediaType.Movie;
                case "vg":
                    return MediaType.VideoGame;
                case "v":
                    return MediaType.Video;
                case "tv":
                    return MediaType.TV;
                case "tv series":
                case "tv mini-series":
                    return MediaType.TVSeries;
                default:
                    return MediaType.Other;
            }
        }

        public MovieId Id
        {
            get { return id; }
        }
        public string Title
        {
            get { return title; }
        }
        public int Year
        {
            get { return year; }
        }
        public bool YearUnknown
        {
            get { return year == -1; }
        }
        public MediaType Type
        {
            get { return type; }
        }
        public MatchType Match
        {
            get { return match; }
        }

        public override string ToString()
        {
            return string.Format("[{2}] {0} ({1:0000})", title, year, type);
        }
    }
}
