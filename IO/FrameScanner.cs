using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeadDog.Movies.IO
{
    internal class FrameScanner
    {
        private bool headerOkay;
        private Version version;
        private int id;

        private Dictionary<FrameIdentifier, Frame> frames = new Dictionary<FrameIdentifier, Frame>();

        public FrameScanner(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);
            byte[] frameheadbuffer = new byte[byte.MaxValue + 4];
            byte[] buffer = new byte[6];
            input.Read(buffer, 0, 6);
            headerOkay = Encoding.ASCII.GetString(buffer, 0, 4) == "MMDb";
            if (!headerOkay)
                return;
            version = new Version(buffer[4], buffer[5]);

            if (version == FileVersions.First)
            {
                input.Read(buffer, 0, 4);
                id = BitConverter.ToInt32(buffer, 0);

                while (input.Position < input.Length)
                {
                    byte b = (byte)input.ReadByte();
                    input.Read(frameheadbuffer, 0, 4 + b);
                    FrameIdentifier identifier = new FrameIdentifier(frameheadbuffer, 0, b);
                    int length = BitConverter.ToInt32(frameheadbuffer, b);
                    int position = (int)input.Position;
                    Frame frame = new Frame(position, length, 5 + b);
                    frames.Add(identifier, frame);
                    input.Seek(length, SeekOrigin.Current);
                }
            }
            else
            {
                headerOkay = false;
                return;
            }
        }

        public bool HeaderOkay
        {
            get { return headerOkay; }
        }
        public Version Version
        {
            get { return version; }
        }
        public int Id
        {
            get { return id; }
        }

        public bool Contains(FrameIdentifier frame)
        {
            return frames.ContainsKey(frame);
        }
        public Frame this[FrameIdentifier identifier]
        {
            get { return frames[identifier]; }
        }
        public Dictionary<FrameIdentifier, Frame> Frames
        {
            get { return frames; }
        }

        public bool Delete(FrameIdentifier frame)
        {
            bool removed = frames.ContainsKey(frame);
            Frame thisF = frames[frame];
            frames.Remove(frame);

            foreach (KeyValuePair<FrameIdentifier, Frame> f in frames)
                if (f.Value.FramePosition > thisF.FramePosition)
                    f.Value.Move(thisF.FrameLength);

            return removed;
        }
        public void Add(FrameIdentifier frame, int dataposition, int datalength)
        {
            frames.Add(frame, new Frame(dataposition, datalength, frame.ByteLength));
        }
    }
}
