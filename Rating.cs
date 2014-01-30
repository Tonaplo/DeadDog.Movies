using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    /// <summary>
    /// Represents a rating of a moving on a selected scale.
    /// </summary>
    public struct Rating
    {
        private string name;
        private bool seen;
        private byte min, max, val;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Rating"/> is empty. I.e. min and max are equal.
        /// </summary>
        public bool IsEmpty
        {
            get { return min == max; }
        }
        /// <summary>
        /// Gets the name associated with this rating.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// Gets a value indicating whether the movie has been seen.
        /// </summary>
        public bool Seen
        {
            get { return seen; }
        }
        /// <summary>
        /// Gets the minimum rating in this <see cref="Rating"/>.
        /// </summary>
        public byte Min
        {
            get { return min; }
        }
        /// <summary>
        /// Gets the maximum rating in this <see cref="Rating"/>.
        /// </summary>
        public byte Max
        {
            get { return max; }
        }
        /// <summary>
        /// Gets the rating value of this <see cref="Rating"/> relative to min and max.
        /// </summary>
        public byte Value
        {
            get { return val; }
        }

        private static Rating empty = new Rating(0, 0, 0);
        /// <summary>
        /// Represents a <see cref="Rating"/> where min and max are equal (zero).
        /// </summary>
        public static Rating Empty
        {
            get { return empty; }
        }

        public static bool operator ==(Rating a, Rating b)
        {
            if (a.IsEmpty && b.IsEmpty)
                return true;
            else
                return a.min == b.min && a.max == b.max && a.val == b.val && a.seen == b.seen && a.name == b.name;
        }
        public static bool operator !=(Rating a, Rating b)
        {
            if (a.IsEmpty ^ b.IsEmpty)
                return false;
            else
                return a.min != b.min || a.max != b.max || a.val != b.val || a.seen != b.seen || a.name != b.name;
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Rating"/>.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this <see cref="Rating"/>.</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        /// <summary>
        /// Specifies wheter this <see cref="Rating"/> has the same information (name, seen, min, max, value) as the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to test.</param>
        /// <returns>true if obj is a <see cref="Rating"/> and if it is completely similar to this <see cref="Rating"/></returns>
        public override bool Equals(object obj)
        {
            if (obj is Rating)
                return ((Rating)obj) == this;
            else
                return false;
        }

        private Rating(byte min, byte max, byte value)
        {
            this.name = "N/A";
            this.seen = false;
            this.min = min;
            this.max = max;
            this.val = value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Rating"/> struct with the specified rating and rating scale.
        /// </summary>
        /// <param name="name">The username associated with the rating. E.g. IMDb, Metacritic, Neo, Mark etc.</param>
        /// <param name="seen">Whether or not the movie has been seen. Unrelated to the actual rating.</param>
        /// <param name="min">The minimum allowed rating on the scale. Commonly set to 0.</param>
        /// <param name="max">The maximum allowed rating on the scale. Commonly set to 6, 10 or other.</param>
        /// <param name="value">The actual rating. This should be relative to min and max.</param>
        public Rating(string name, bool seen, byte min, byte max, byte value)
        {
            if (max < min)
                throw new ArgumentException("min must be less than or equal to max.");
            if (value < min || value > max)
                throw new ArgumentException("Rating can be no less than min and no greater than max.");

            if (name == null)
                throw new ArgumentNullException("name");
            else if (name.Length == 0)
                throw new ArgumentException("name must be at least 3 character.", "name");
            else if (name == "N/A")
                throw new ArgumentException("name cannot be N/A, use Rating.Empty instead.", "name");

            this.name = name;
            this.seen = seen;
            this.min = min;
            this.max = max;
            this.val = value;
        }

        /// <summary>
        /// Re-calculates this rating to fit another scale.
        /// </summary>
        /// <param name="newMax">The new maximum.</param>
        /// <param name="newMin">The new minimum.</param>
        /// <returns>The re-calculated rating.</returns>
        public float GetRating(float newMax, float newMin)
        {
            if (newMax < newMin)
                throw new ArgumentException("newMin must be less than or equal to newMax.");
            if (newMin == newMax)
                return newMin;
            if (IsEmpty)
                return newMin;

            float i = (float)min;
            float a = (float)max;
            float v = (float)val;
            return ((v - i) / (a - i)) * (newMax - newMin) + newMin;
        }
        /// <summary>
        /// Re-calculates this rating to fit another scale where 0 is the minimum.
        /// </summary>
        /// <param name="newMax">The new maximum.</param>
        /// <returns>The re-calculated rating.</returns>
        public float GetRating(float newMax)
        {
            return GetRating(newMax, 0);
        }

        public override string ToString()
        {
            if (min == 0)
                return val + "/" + max;
            else
                return min + " < " + val + " > " + max;
        }
    }
}
