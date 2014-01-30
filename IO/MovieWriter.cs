using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Movies.IO
{
    public class MovieWriter : IDisposable
    {
        private FileInfo file;
        private FileStream output;
        private FrameScanner fs;

        public MovieWriter(string filepath, MovieId id, bool UseExistingFile)
        {
            file = new FileInfo(filepath);

            if (file.Exists && UseExistingFile)
            {
                using (FileStream input = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    fs = new FrameScanner(input);

                if (fs.HeaderOkay)
                    output = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite);
            }
            else
            {
                output = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                output.Write(Encoding.ASCII.GetBytes("MMDb"), 0, 4);
                output.WriteByte((byte)FileVersions.First.Major);
                output.WriteByte((byte)FileVersions.First.Minor);
                output.Write(BitConverter.GetBytes(id.Id), 0, 4);
                output.Close();

                using (FileStream input = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    fs = new FrameScanner(input);

                output = new FileStream(filepath, FileMode.Open, FileAccess.Write);
            }
        }

        public void Write(Rating rating)
        {
            Write(new FrameIdentifier(rating), rating);
        }
        public void Write(PersonCredit person)
        {
            if (person is Role)
                Write(person as Role);
            else if (person is DirectorCredit)
                Write(person as DirectorCredit);
            else if (person is ProducerCredit)
                Write(person as ProducerCredit);
            else if (person is WriterCredit)
                Write(person as WriterCredit);
            else
                throw new Exception("Unknown credit type.");
        }
        public void Write(Role person)
        {
            FrameIdentifier identifier = new FrameIdentifier(person);
            Write(identifier, person);
        }
        public void Write(DirectorCredit person)
        {
            FrameIdentifier identifier = new FrameIdentifier(person);
            Write(identifier, person);
        }
        public void Write(ProducerCredit person)
        {
            FrameIdentifier identifier = new FrameIdentifier(person);
            Write(identifier, person);
        }
        public void Write(WriterCredit person)
        {
            FrameIdentifier identifier = new FrameIdentifier(person);
            Write(identifier, person);
        }
        public void Write(GenreSet genres)
        {
            Write(new FrameIdentifier(genres), genres);
        }

        public void WriteRuntime(TimeSpan runtime)
        {
            Write(FrameIdentifier.RuntimeIdentifier, runtime.Ticks);
        }

        public void WriteYear(int year)
        {
            Write(FrameIdentifier.YearIdentifier, (long)year);
        }
        public void WriteTitle(string title)
        {
            Write(FrameIdentifier.TitleIdentifier, title);
        }
        public void WritePoster(System.Drawing.Image image)
        {
            Write(FrameIdentifier.PosterIdentifier, image);
        }

        #region MainMethods

        public void Write(FrameIdentifier identifier, Rating rating)
        {
            MemoryStream ms = new MemoryStream(4);
            ms.WriteByte(BitConverter.GetBytes(rating.Seen)[0]);
            ms.WriteByte(rating.Min);
            ms.WriteByte(rating.Max);
            ms.WriteByte(rating.Value);
            WriteFrameFromStream(identifier, ms, true);
        }
        public void Write(FrameIdentifier identifier, Role person)
        {
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)person.CreditType);
            AddString(ms, person.Person.Name, Encoding.UTF8);

            ms.WriteByte((byte)person.ActorType);
            AddString(ms, person.Rolename, Encoding.UTF8);

            WriteFrameFromStream(identifier, ms, false);
            ms.Dispose();
        }
        public void Write(FrameIdentifier identifier, DirectorCredit person)
        {
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)person.CreditType);
            AddString(ms, person.Person.Name, Encoding.UTF8);

            WriteFrameFromStream(identifier, ms, false);
            ms.Dispose();
        }
        public void Write(FrameIdentifier identifier, ProducerCredit person)
        {
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)person.CreditType);
            AddString(ms, person.Person.Name, Encoding.UTF8);

            WriteFrameFromStream(identifier, ms, false);
            ms.Dispose();
        }
        public void Write(FrameIdentifier identifier, WriterCredit person)
        {
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)person.CreditType);
            AddString(ms, person.Person.Name, Encoding.UTF8);

            WriteFrameFromStream(identifier, ms, false);
            ms.Dispose();
        }

        public void Write(FrameIdentifier identifier, System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes((int)image.Width), 0, 4);
            ms.Write(BitConverter.GetBytes((int)image.Height), 0, 4);
            ms.Write(BitConverter.GetBytes((int)0), 0, 4);
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Seek(8, SeekOrigin.Begin);
            ms.Write(BitConverter.GetBytes((int)ms.Length - 4), 0, 4);
            WriteFrameFromStream(identifier, ms, false);
            ms.Dispose();
        }
        public void Write(FrameIdentifier identifier, string value)
        {
            Write(identifier, value, Encoding.UTF8);
        }
        public void Write(FrameIdentifier identifier, string value, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream();
            AddString(ms, value, encoding);
            WriteFrameFromStream(identifier, ms, false);
            ms.Dispose();
        }
        public void Write(FrameIdentifier identifier, long value)
        {
            using (MemoryStream ms = new MemoryStream(BitConverter.GetBytes(value)))
                WriteFrameFromStream(identifier, ms, true);
        }
        public void Write(FrameIdentifier identifier, GenreSet genres)
        {
            MemoryStream ms = new MemoryStream();
            foreach(Genre g in genres)
            {
                byte b = (byte)g.Id;
                ms.WriteByte(b);
            }
            WriteFrameFromStream(identifier, ms, false);
            ms.Dispose();
        }

        #endregion

        private void WriteFrameFromStream(FrameIdentifier identifier, MemoryStream stream, bool fixedSize)
        {
            if (!fs.HeaderOkay)
                return;
            if (fs.Contains(identifier))
            {
                if (fixedSize || stream.Length <= fs[identifier].DataLength)
                {
                    output.Seek(fs[identifier].DataPosition, SeekOrigin.Begin);
                    stream.WriteTo(output);
                }
                else
                {
                    Delete(identifier);
                    WriteFrameFromStream(identifier, stream, fixedSize);
                }
            }
            else
            {
                output.Seek(0, SeekOrigin.End);
                output.WriteByte(identifier.ByteLength);
                output.Write(identifier.GetBytes(), 0, identifier.ByteLength);

                output.Write(BitConverter.GetBytes((int)stream.Length), 0, 4);
                fs.Add(identifier, (int)output.Position, (int)stream.Length);
                stream.WriteTo(output);
            }
        }

        private void AddString(Stream stream, string value, Encoding encoding)
        {
            byte enc = EncodingBytes.GetByte(encoding);
            byte[] buffer;

            stream.WriteByte(enc);
            buffer = encoding.GetBytes(value);
            stream.Write(BitConverter.GetBytes((short)buffer.Length), 0, 2);
            stream.Write(buffer, 0, buffer.Length);
        }

        public bool Contains(FrameIdentifier identifier)
        {
            return fs.Contains(identifier);
        }

        public void Delete(FrameIdentifier identifier)
        {
            if (!fs.HeaderOkay)
                return;
            if (!fs.Contains(identifier))
                throw new Exception("identifier " + identifier.Value + " not found.");

            Frame frame = fs[identifier];

            if (frame.FramePosition + frame.FrameLength == output.Length)
            {
                output.Seek(frame.FramePosition - 1, SeekOrigin.Begin);
                output.SetLength(frame.FramePosition);
            }
            else
            {
                output.Seek(frame.FramePosition + frame.FrameLength, SeekOrigin.Begin);
                byte[] buffer = new byte[512];
                int read = 0;
                do
                {
                    read = output.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        output.Seek(-(frame.FrameLength + read), SeekOrigin.Current);
                        output.Write(buffer, 0, read);
                        output.Seek(frame.FrameLength, SeekOrigin.Current);
                    }
                } while (read > 0);
                output.SetLength(output.Length - frame.FrameLength);
            }

            fs.Delete(identifier);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (output != null)
            {
                output.Close();
                output.Dispose();
                output = null;
            }
        }

        #endregion
    }
}
