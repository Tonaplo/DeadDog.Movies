using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies.IO
{
    internal class Frame
    {
        private int position;
        private int length;
        private int headerlength;

        public Frame(int position, int length, int headerlength)
        {
            this.position = position;
            this.length = length;
            this.headerlength = headerlength;
        }

        public int DataPosition
        {
            get { return position; }
        }
        public int DataLength
        {
            get { return length; }
        }
        public int FramePosition
        {
            get { return position - headerlength; }
        }
        public int FrameLength
        {
            get { return length + headerlength; }
        }

        /// <summary>
        /// Reassigns the position of this <see cref="Frame"/> instance. Use with care!
        /// </summary>
        /// <param name="length">The distance the frame is "moved".</param>
        public void Move(int length)
        {
            this.position += length;
        }

        public override string ToString()
        {
            return "Position: " + position + ", Length: " + length;
        }
    }
}
