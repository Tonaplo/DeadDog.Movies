using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies.IO
{
    /// <summary>
    /// Species which type of data is associated with a <see cref="FrameIdentifier"/>.
    /// </summary>
    public enum FrameTypes
    {
        /// <summary>
        /// No data type specified. Not to be used for reading/writing.
        /// </summary>
        None,
        /// <summary>
        /// The frame contains a number in form of a <see cref="Int64"/>.
        /// </summary>
        Number,
        /// <summary>
        /// The frame contains text in form of a <see cref="String"/>.
        /// </summary>
        Text,
        /// <summary>
        /// The frame contains an <see cref="Image"/>.
        /// </summary>
        Image,
        /// <summary>
        /// The frame contains rating info.
        /// </summary>
        Rating,
        /// <summary>
        /// The frame contains info about an actor.
        /// </summary>
        Actor,
        /// <summary>
        /// The frame contains info about a director.
        /// </summary>
        Direcor,
        /// <summary>
        /// The frame contains info about a producer.
        /// </summary>
        Producer,
        /// <summary>
        /// The frame contains info about a director.
        /// </summary>
        Writer,
        /// <summary>
        /// The frame contains info about a set of genres.
        /// </summary>
        Genreset
    }
}
