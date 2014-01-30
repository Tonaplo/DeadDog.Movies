using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    /// <summary>
    /// Identifies the type of actor, sorted by the level of significance in the movie.
    /// </summary>
    public enum ActorTypes
    {
        /// <summary>
        /// The actor has no significance (or it is unknown).
        /// </summary>
        None = 0,
        /// <summary>
        /// The lead actor, highest significance.
        /// </summary>
        LeadActor = 1,
        /// <summary>
        /// Supporting actor, second highest significance.
        /// </summary>
        SupportingActor = 2,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor3 = 3,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor4 = 4,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor5 = 5,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor6 = 6,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor7 = 7,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor8 = 8,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor9 = 9,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor10 = 10,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor11 = 11,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor12 = 12,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor13 = 13,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Actor14 = 14,
        /// <summary>
        /// The lowest type of significance.
        /// </summary>
        ActorOther = 15
    }
}
