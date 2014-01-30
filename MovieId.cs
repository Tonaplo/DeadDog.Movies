using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    /// <summary>
    /// Identifies a movie in terms of a unique integer.
    /// This integer will match that of IMDb.com
    /// </summary>
    public struct MovieId : IEquatable<MovieId>
    {
        private int id;

        /// <summary>
        /// Gets a <see cref="MovieId"/> object where the identifier is zero.
        /// </summary>
        public static MovieId Empty
        {
            get { return new MovieId() { id = 0 }; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieId"/> struct from an indentifying integer.
        /// </summary>
        /// <param name="id">The integer identifying the movie.</param>
        public MovieId(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "Identifier must be greater than zero.");
            this.id = id;
        }

        /// <summary>
        /// Converts a string to a <see cref="MovieId"/> instance. 
        /// A return value indicates whether the conversion was succeeded.
        /// </summary>
        /// <param name="s">A string containing a number or an url address to convert.</param>
        /// <param name="result">When the method returns, contains the converted <see cref="MovieId"/>, if the conversion succeeded; otherwise, contains <see cref="MovieId.Empty"/>.</param>
        /// <returns>true if <paramref name="s"/> was converted successfully; otherwise false.</returns>
        public static bool TryParse(string s, out MovieId result)
        {
            int id;
            string t = s.ToLower().Trim();
            if (t.StartsWith("http://"))
                t = t.Substring(7);
            if (t.StartsWith("www."))
                t = t.Substring(4);
            if (t.StartsWith("imdb.com"))
                t = t.Substring(8);
            if (t.StartsWith("/"))
                t = t.Substring(1);
            if (t.StartsWith("title"))
                t = t.Substring(5);
            if (t.StartsWith("/"))
                t = t.Substring(1);

            if (t.StartsWith("tt"))
            {
                bool ok = int.TryParse(t.CutToFirst('/', CutDirection.Right, true).Substring(2), out id);
                if (ok)
                    result = new MovieId(id);
                else
                    result = new MovieId();
                return ok;
            }
            else
            {
                result = new MovieId();
                return false;
            }
        }
        /// <summary>
        /// Converts a <see cref="URL"/> to a <see cref="MovieId"/> instance. 
        /// A return value indicates whether the conversion was succeeded.
        /// </summary>
        /// <param name="url">A <see cref="URL"/> containing the address to convert. This should be an address identifying a movie on IMDb.com.</param>
        /// <param name="result">When the method returns, contains the converted <see cref="MovieId"/>, if the conversion succeeded; otherwise, contains <see cref="MovieId.Empty"/>.</param>
        /// <returns>true if <paramref name="s"/> was converted successfully; otherwise false.</returns>
        public static bool TryParse(URL url, out MovieId result)
        {
            return TryParse(url.Address, out result);
        }

        /// <summary>
        /// Gets the identifying integer associated with this <see cref="MovieId"/>.
        /// </summary>
        public int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Compares two <see cref="MovieId"/> objects. 
        /// The result specifies whether the identifier <see cref="MovieId.Id"/> of the two <see cref="MovieId"/> are equal.
        /// </summary>
        /// <param name="a">A <see cref="MovieId"/> to compare.</param>
        /// <param name="b">A <see cref="MovieId"/> to compare.</param>
        /// <returns>true if the identifier of <paramref name="a"/> and <paramref name="b"/> are equal; otherwise false.</returns>
        public static bool operator ==(MovieId a, MovieId b)
        {
            return a.id == b.id;
        }
        /// <summary>
        /// Compares two <see cref="MovieId"/> objects. 
        /// The result specifies whether the identifier <see cref="MovieId.Id"/> of the two <see cref="MovieId"/> are not equal.
        /// </summary>
        /// <param name="a">A <see cref="MovieId"/> to compare.</param>
        /// <param name="b">A <see cref="MovieId"/> to compare.</param>
        /// <returns>true if the identifier of <paramref name="a"/> and <paramref name="b"/> are not equal; otherwise false.</returns>
        public static bool operator !=(MovieId a, MovieId b)
        {
            return a.id != b.id;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="MovieId"/> and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (!(obj is MovieId))
                return false;

            return this.Equals((MovieId)obj);
        }
        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="MovieId"/> value.
        /// </summary>
        /// <param name="other">A <see cref="MovieId"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public bool Equals(MovieId other)
        {
            if (ReferenceEquals(other, null))
                return false;
            else
                return this.id.Equals(other.id);
        }
        /// <summary>
        /// Converts the identifier of this value to a string.
        /// </summary>
        /// <returns>A string on the form "tt0000000" where the range of zero's represent the actual identifier.</returns>
        public override string ToString()
        {
            return "tt" + String.Format("{0:0000000}", id);
        }

        /// <summary>
        /// Returns http://www.imdb.com/title/tt0000000/ for the movie as an <see cref="URL"/>.
        /// </summary>
        /// <returns>Returns an <see cref="URL"/> to the IMDB page with basic movie info.</returns>
        public URL GetURL()
        {
            return new URL("http://www.imdb.com/title/tt" + String.Format("{0:0000000}", id) + "/");
        }
    }
}
