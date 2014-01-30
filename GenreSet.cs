using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class GenreSet : IEnumerable<Genre>
    {
        private string name;
        private List<Genre> genres;
        private GenreCollection collection;

        public GenreSet(string name, GenreCollection collection)
            : this(name, collection, new Genre[] { })
        {
        }
        public GenreSet(string name, GenreCollection collection, IEnumerable<Genre> genres)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            else if (name.Length == 0)
                throw new ArgumentException("name must be at least 3 character.", "name");

            if (collection == null)
                throw new ArgumentNullException("collection");

            this.name = name;
            this.collection = collection;
            if (genres == null)
                this.genres = new List<Genre>();
            else
            {
                foreach (Genre g in genres)
                {
                    if (!collection.Contains(g))
                        throw new ArgumentException("Genre \"" + g.Name + "\" does not exist in collection", "genres");
                }
                this.genres = new List<Genre>(genres);
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Add(Genre genre)
        {
            if (collection.Contains(genre))
            {
                if (genres.Contains(genre))
                    return false;
                genres.Add(genre);
                return true;
            }
            else
                return false;
        }
        public bool Remove(Genre genre)
        {
            if (genres.Contains(genre))
            {
                genres.Remove(genre);
                return true;
            }
            else
                return false;
        }

        public bool Contains(Genre genre)
        {
            return genres.Contains(genre);
        }

        public int Count
        {
            get { return genres.Count; }
        }

        public GenreCollection Collection
        {
            get { return collection; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Genre g in genres)
                sb.Append(g.Name + " ");
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        #region IEnumerable<Genre> Members

        public IEnumerator<Genre> GetEnumerator()
        {
            return genres.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return genres.GetEnumerator();
        }

        #endregion
    }
}
