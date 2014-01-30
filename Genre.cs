using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    /// <summary>
    /// Describes a movie genre
    /// </summary>
    public class Genre
    {
        private int id;
        private string name;

        internal Genre(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int Id
        {
            get { return id; }
        }
        public string Name
        {
            get { return name; }
        }

        public override string ToString()
        {
            return name + " [" + id.ToString() + "]";
        }
    }
}
