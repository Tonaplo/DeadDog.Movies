using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class GenreCollection : IEnumerable<Genre>
    {
        public static GenreCollection CreateStandard()
        {
            GenreCollection c = new GenreCollection();
            c.setupStandard();
            return c;
        }

        private void setupStandard()
        {
            GetGenre(1, "Action");
            GetGenre(2, "Adventure");
            GetGenre(3, "Animation");

            GetGenre(4, "Biography");

            GetGenre(5, "Comedy");
            GetGenre(6, "Crime");

            GetGenre(7, "Documentary");
            GetGenre(8, "Drama");

            GetGenre(9, "Family");
            GetGenre(10, "Fantasy");
            GetGenre(11, "Film-Noir");

            GetGenre(12, "Game-Show");

            GetGenre(13, "History");
            GetGenre(14, "Horror");

            GetGenre(15, "Music");
            GetGenre(16, "Musical");
            GetGenre(17, "Mystery");

            GetGenre(18, "News");

            GetGenre(19, "Reality-TV");
            GetGenre(20, "Romance");

            GetGenre(21, "Sci-Fi");
            GetGenre(22, "Sport");

            GetGenre(23, "Talk-Show");
            GetGenre(24, "Thriller");

            GetGenre(25, "War");
            GetGenre(26, "Western");
        }

        private Dictionary<int, Genre> genres = new Dictionary<int, Genre>();

        /// <summary>
        /// Sets up an empty collection of genres. Use <see cref="GenreCollection.CreateStandard"/> instead.
        /// </summary>
        public GenreCollection()
        {
        }

        public Genre GetGenre(int id, string name)
        {
            if (genres.ContainsKey(id))
                return genres[id];
            else
            {
                Genre g = new Genre(id, name);
                genres.Add(id, g);
                return g;
            }
        }
        public Genre GetGenre(int id)
        {
            if (genres.ContainsKey(id))
                return genres[id];
            else
                return null;
        }
        public Genre GetGenre(string name)
        {
            string s = name.Trim().ToLower();
            Genre genre = null;
            foreach (Genre g in genres.Values)
            {
                if (g.Name.ToLower() == s)
                {
                    genre = g;
                    break;
                }
            }
            return genre;
        }

        public bool Contains(Genre genre)
        {
            foreach (Genre g in genres.Values)
                if (g == genre)
                    return true;
            return false;
        }

        public Genre[] GetGenres()
        {
            Genre[] g = new Genre[genres.Values.Count];
            genres.Values.CopyTo(g, 0);
            return g;
        }

        #region IEnumerable<Genre> Members

        public IEnumerator<Genre> GetEnumerator()
        {
            return genres.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return genres.Values.GetEnumerator();
        }

        #endregion
    }
}
