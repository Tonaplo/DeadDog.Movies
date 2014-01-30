using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Movies.IO
{
    public class MovieReader : IDisposable
    {
        private FileInfo file;
        private FileStream input;
        private FrameScanner fs;
        private MovieId id;

        public MovieReader(string filepath)
        {
            file = new FileInfo(filepath);

            if (file.Exists)
            {
                input = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                fs = new FrameScanner(input);
                this.id = new MovieId(fs.Id);
            }
            else
                this.id = new MovieId();
        }
        private bool SeekToFrame(FrameIdentifier identifier)
        {
            if (!fs.Contains(identifier))
                return false;

            input.Seek(fs[identifier].DataPosition, SeekOrigin.Begin);
            return true;
        }
        private bool SeekToFrame(FrameIdentifier identifier, out int length)
        {
            length = 0;
            if (!fs.Contains(identifier))
                return false;

            input.Seek(fs[identifier].DataPosition, SeekOrigin.Begin);
            length = fs[identifier].DataLength;
            return true;
        }

        public MovieId MovieId
        {
            get { return id; }
        }

        public bool Contains(FrameIdentifier identifier)
        {
            return fs.Contains(identifier);
        }

        public int ReadYear()
        {
            return (int)ReadInt64(FrameIdentifier.YearIdentifier);
        }
        public string ReadTitle()
        {
            return ReadString(FrameIdentifier.TitleIdentifier);
        }
        public System.Drawing.Size ReadPosterSize()
        {
            return ReadImageSize(FrameIdentifier.PosterIdentifier);
        }
        public System.Drawing.Image ReadPoster()
        {
            return ReadImage(FrameIdentifier.PosterIdentifier);
        }
        public void ReadPosterXNA(out long offset, out int count)
        {
            ReadImageXNA(FrameIdentifier.PosterIdentifier, out offset, out count);
        }

        public Rating ReadIMDbRating()
        {
            return ReadRating(FrameIdentifier.IMDbRatingIdentifier);
        }
        public Rating ReadMetacriticRating()
        {
            return ReadRating(FrameIdentifier.MetacriticRatingIdentifier);
        }
        public GenreSet ReadIMDbGenres(GenreCollection collection)
        {
            return ReadGenreSet(FrameIdentifier.IMDbGenresIdentifier, collection);
        }

        public TimeSpan ReadRuntime()
        {
            return new TimeSpan(ReadInt64(FrameIdentifier.RuntimeIdentifier));
        }

        #region Reading Persons

        public PersonCredit ReadPersonCredit(FrameIdentifier identifier, PersonCollection collection)
        {
            Person p = readPerson(identifier, collection);

            if (identifier.Value[1] == 'A')
                return readRole(p);
            else if (identifier.Value[1] == 'D')
                return new DirectorCredit(p);
            else if (identifier.Value[1] == 'W')
                return new WriterCredit(p);
            else if (identifier.Value[1] == 'P')
                return new ProducerCredit(p);
            else
                throw new Exception("Unknown Credit type.");
        }
        private Person readPerson(FrameIdentifier identifier, PersonCollection collection)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            int id = int.Parse(identifier.Value.Substring(1), System.Globalization.NumberStyles.HexNumber);
            CreditTypes type = (CreditTypes)input.ReadByte();
            string name = ReadString();

            return collection.GetPerson(id, name);
        }
        private Role readRole(Person person)
        {
            ActorTypes act = (ActorTypes)input.ReadByte();
            string role = ReadString();
            return new Role(person, role, act);
        }
        public Role ReadRole(FrameIdentifier identifier, PersonCollection collection)
        {
            return readRole(readPerson(identifier, collection));
        }
        public DirectorCredit ReadDirector(FrameIdentifier identifier, PersonCollection collection)
        {
            return new DirectorCredit(readPerson(identifier, collection));
        }
        public ProducerCredit ReadProducer(FrameIdentifier identifier, PersonCollection collection)
        {
            return new ProducerCredit(readPerson(identifier, collection));
        }
        public WriterCredit ReadWriter(FrameIdentifier identifier, PersonCollection collection)
        {
            return new WriterCredit(readPerson(identifier, collection));
        }

        #endregion

        public Rating ReadRating(FrameIdentifier identifier)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            byte[] buffer = new byte[4];
            input.Read(buffer, 0, 4);
            return new Rating(identifier.Value.Substring(1), BitConverter.ToBoolean(buffer, 0), buffer[1], buffer[2], buffer[3]);
        }
        public System.Drawing.Size ReadImageSize(FrameIdentifier identifier)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");
            byte[] buffer = new byte[8];
            input.Read(buffer, 0, 8);

            return new System.Drawing.Size(BitConverter.ToInt32(buffer, 0), BitConverter.ToInt32(buffer, 4));
        }
        public System.Drawing.Image ReadImage(FrameIdentifier identifier)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            input.Seek(8, SeekOrigin.Current);
            byte[] buffer = new byte[4];
            input.Read(buffer, 0, 4);
            int length = BitConverter.ToInt32(buffer, 0);
            MemoryStream ms = new MemoryStream();

            int read = 0;
            buffer = new byte[8192];
            do
            {
                read = input.Read(buffer, 0, 8192);
                ms.Write(buffer, 0, read);
            } while (read > 0);

            return System.Drawing.Image.FromStream(ms);
        }
        public MemoryStream ReadData(FrameIdentifier identifier)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            input.Seek(8, SeekOrigin.Current);
            byte[] buffer = new byte[4];
            input.Read(buffer, 0, 4);
            int length = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[length];
            input.Read(buffer, 0, length);

            MemoryStream ms = new MemoryStream(buffer);
            return ms;
        }
        public void ReadImageXNA(FrameIdentifier identifier, out long offset, out int count)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            input.Seek(8, SeekOrigin.Current);
            byte[] buffer = new byte[4];
            input.Read(buffer, 0, 4);
            int length = BitConverter.ToInt32(buffer, 0);

            offset = input.Position;
            count = length;
        }
        public string ReadString(FrameIdentifier identifier)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            return ReadString();
        }
        public long ReadInt64(FrameIdentifier identifier)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            byte[] buffer = new byte[8];
            input.Read(buffer, 0, 8);
            long length = BitConverter.ToInt64(buffer, 0);
            return length;
        }
        public GenreSet ReadGenreSet(FrameIdentifier identifier, GenreCollection collection)
        {
            if (!SeekToFrame(identifier))
                throw new ArgumentException(identifier.Value + " Frame not found.");

            GenreSet g = new GenreSet(identifier.Value.Substring(1), collection);
            int c = fs.Frames[identifier].DataLength;
            for (int i = 0; i < c; i++)
                g.Add(collection.GetGenre(input.ReadByte()));

            return g;
        }

        [Obsolete("Use the IsFrameType extension method instead.")]
        public IEnumerable<FrameIdentifier> GetIdentifiers(FrameTypes type)
        {
            return fs.Frames.Keys.IsFrameType(type);
        }
        public IEnumerable<FrameIdentifier> GetIdentifiers()
        {
            foreach (FrameIdentifier fi in fs.Frames.Keys)
                yield return fi;
        }

        private string ReadString()
        {
            Encoding encoding = EncodingBytes.GetEncoding((byte)input.ReadByte());
            byte[] buffer = new byte[2];
            input.Read(buffer, 0, 2);
            short length = BitConverter.ToInt16(buffer, 0);
            buffer = new byte[length];
            input.Read(buffer, 0, length);
            return encoding.GetString(buffer);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (input != null)
            {
                input.Close();
                input.Dispose();
                input = null;
            }
        }

        #endregion
    }
}
