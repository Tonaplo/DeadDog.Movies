using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies.IO
{
    /// <summary>
    /// Used to identify frames of data when reading/writing movies.
    /// </summary>
    public class FrameIdentifier
    {
        private FrameTypes type;
        private string identifier;
        private byte[] buffer;

        private static char GetPrefix(FrameTypes frametype)
        {
            switch (frametype)
            {
                case FrameTypes.Image:
                    return 'I';
                case FrameTypes.Number:
                    return 'N';
                case FrameTypes.Rating:
                    return 'R';
                case FrameTypes.Text:
                    return 'T';
                case FrameTypes.Actor:
                    return 'A';
                case FrameTypes.Direcor:
                    return 'D';
                case FrameTypes.Producer:
                    return 'P';
                case FrameTypes.Writer:
                    return 'W';
                case FrameTypes.Genreset:
                    return 'G';
                default:
                    return '\0';
            }
        }
        private static FrameTypes GetFrameType(char c)
        {
            switch (c)
            {
                case 'I':
                    return FrameTypes.Image;
                case 'N':
                    return FrameTypes.Number;
                case 'R':
                    return FrameTypes.Rating;
                case 'T':
                    return FrameTypes.Text;
                case 'A':
                    return FrameTypes.Actor;
                case 'D':
                    return FrameTypes.Direcor;
                case 'P':
                    return FrameTypes.Producer;
                case 'W':
                    return FrameTypes.Writer;
                case 'G':
                    return FrameTypes.Genreset;
                default:
                    return FrameTypes.None;
            }
        }

        #region Static FrameIdentifier

        private static FrameIdentifier titleidentifier = new FrameIdentifier(FrameTypes.Text, "TITLE");
        private static FrameIdentifier yearidentifier = new FrameIdentifier(FrameTypes.Number, "YEAR");
        private static FrameIdentifier posteridentifier = new FrameIdentifier(FrameTypes.Image, "POSTER");
        private static FrameIdentifier imdbgenresidentifier = new FrameIdentifier(FrameTypes.Genreset, "IMDb");
        private static FrameIdentifier imdbratingidentifier = new FrameIdentifier(FrameTypes.Rating, "IMDb");
        private static FrameIdentifier metacriticratingidentifier = new FrameIdentifier(FrameTypes.Rating, "Metacritic");
        private static FrameIdentifier runtimeidentifier = new FrameIdentifier(FrameTypes.Number, "RUNTIME");

        /// <summary>
        /// The standard identifier used for reading/writing the title of a movie.
        /// </summary>
        public static FrameIdentifier TitleIdentifier
        {
            get { return titleidentifier; }
        }
        /// <summary>
        /// The standard identifier used for reading/writing the release year of a movie.
        /// </summary>
        public static FrameIdentifier YearIdentifier
        {
            get { return yearidentifier; }
        }
        /// <summary>
        /// The standard identifier used for reading/writing the poster for a movie.
        /// </summary>
        public static FrameIdentifier PosterIdentifier
        {
            get { return posteridentifier; }
        }
        /// <summary>
        /// The standard identifier used for reading/writing the genres associated with a movie as specified by IMDb.
        /// </summary>
        public static FrameIdentifier IMDbGenresIdentifier
        {
            get { return imdbgenresidentifier; }
        }
        /// <summary>
        /// The standard identifier used for reading/writing the IMDb rating of a movie
        /// </summary>
        public static FrameIdentifier IMDbRatingIdentifier
        {
            get { return imdbratingidentifier; }
        }
        /// <summary>
        /// The standard identifier used for reading/writing the Metacritic rating of a movie
        /// </summary>
        public static FrameIdentifier MetacriticRatingIdentifier
        {
            get { return metacriticratingidentifier; }
        }
        /// <summary>
        /// The standard identifier used for reading/writing the runtime (in minutes) of a movie
        /// </summary>
        public static FrameIdentifier RuntimeIdentifier
        {
            get { return runtimeidentifier; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameIdentifier"/> from a <see cref="Role"/> item.
        /// </summary>
        /// <param name="person">The item from which to construct a unique <see cref="FrameIdentifier"/>.</param>
        public FrameIdentifier(Role person)
            : this(FrameTypes.Actor, person.Person.Id.ToString("X3"))
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameIdentifier"/> from a <see cref="DirectorCredit"/> item.
        /// </summary>
        /// <param name="person">The item from which to construct a unique <see cref="FrameIdentifier"/>.</param>
        public FrameIdentifier(DirectorCredit person)
            : this(FrameTypes.Direcor, person.Person.Id.ToString("X3"))
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameIdentifier"/> from a <see cref="ProducerCredit"/> item.
        /// </summary>
        /// <param name="person">The item from which to construct a unique <see cref="FrameIdentifier"/>.</param>
        public FrameIdentifier(ProducerCredit person)
            : this(FrameTypes.Producer, person.Person.Id.ToString("X3"))
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameIdentifier"/> from a <see cref="WriterCredit"/> item.
        /// </summary>
        /// <param name="person">The item from which to construct a unique <see cref="FrameIdentifier"/>.</param>
        public FrameIdentifier(WriterCredit person)
            : this(FrameTypes.Writer, person.Person.Id.ToString("X3"))
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameIdentifier"/> from a <see cref="Rating"/> item.
        /// </summary>
        /// <param name="genres">The item from which to construct a unique <see cref="FrameIdentifier"/>.</param>
        public FrameIdentifier(Rating rating)
            : this(FrameTypes.Rating, rating.Name)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameIdentifier"/> from a <see cref="GenreSet"/> item.
        /// </summary>
        /// <param name="genres">The item from which to construct a unique <see cref="FrameIdentifier"/>.</param>
        public FrameIdentifier(GenreSet genres)
            : this(FrameTypes.Genreset, genres.Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameIdentifier"/>.
        /// </summary>
        /// <param name="frametype">The type of data associated with this <see cref="FrameIdentifier"/></param>
        /// <param name="identifier">A user-defined string for the identifier. Must be 3 chars or more.</param>
        public FrameIdentifier(FrameTypes frametype, string identifier)
        {
            if (identifier.Length < 3)
                throw new ArgumentException("An identifier can be no shorter than 3 characters.");

            foreach (char c in identifier)
            {
                if (!char.IsLetterOrDigit(c))
                    throw new ArgumentException("An identifier cannot contain the char '" + c + "'");
            }

            this.type = frametype;
            char prefix = GetPrefix(frametype);
            if (prefix == '\0')
                throw new ArgumentException("Unknown frametype: " + frametype.ToString());
            else
                this.identifier = prefix + identifier;

            this.buffer = Encoding.ASCII.GetBytes(this.identifier);
            if (this.buffer.Length > byte.MaxValue)
                throw new ArgumentException("An identifier can be no longer than " + byte.MaxValue + " characters.");
        }
        /// <summary>
        /// Internal constructor! Only to be used by <see cref="FrameScanner"/>!
        /// </summary>
        /// <param name="bytes">A buffer from which the identifier is read</param>
        /// <param name="index">The index of the first byte in the identifier</param>
        /// <param name="count">The number of bytes in the identifier</param>
        internal FrameIdentifier(byte[] bytes, int index, int count)
        {
            this.identifier = Encoding.ASCII.GetString(bytes, index, count);
            this.type = GetFrameType(this.identifier[0]);
            this.buffer = new byte[count];
            Buffer.BlockCopy(bytes, index, this.buffer, 0, count);
        }

        public static bool operator ==(FrameIdentifier a, FrameIdentifier b)
        {
            return a.identifier == b.identifier;
        }
        public static bool operator !=(FrameIdentifier a, FrameIdentifier b)
        {
            return a.identifier != b.identifier;
        }

        public bool IsFrameType(FrameTypes type)
        {
            return this.type == type;
        }
        public override bool Equals(object obj)
        {
            if (obj is FrameIdentifier)
                return identifier.Equals((obj as FrameIdentifier).identifier);
            else
                return false;
        }
        public override int GetHashCode()
        {
            return identifier.GetHashCode();
        }

        /// <summary>
        /// Gets the full identifier as it will be represented in a file.
        /// </summary>
        public string Value
        {
            get { return identifier; }
        }
        internal byte ByteLength
        {
            get { return (byte)buffer.Length; }
        }

        internal byte[] GetBytes()
        {
            return buffer;
        }

        public override string ToString()
        {
            return identifier.ToString();
        }
    }
}
