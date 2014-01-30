using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    /// <summary>
    /// Enumerates the various types of media from IMDB.com
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        /// An unknown or undefined media type.
        /// </summary>
        Other,
        /// <summary>
        /// A movie.
        /// </summary>
        Movie,
        /// <summary>
        /// A video.
        /// </summary>
        Video,
        /// <summary>
        /// A video game.
        /// </summary>
        VideoGame,
        /// <summary>
        /// A movie produced for television.
        /// </summary>
        TV,
        /// <summary>
        /// A television series.
        /// </summary>
        TVSeries,
        /// <summary>
        /// An episode from a television series.
        /// </summary>
        TVEpisode
    }
}
