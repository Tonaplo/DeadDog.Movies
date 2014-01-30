using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    /// <summary>
    /// Enumerates different types of credits for a person.
    /// </summary>
    public enum CreditTypes
    {
        /// <summary>
        /// None or unknown type of credit.
        /// </summary>
        None = 0,
        /// <summary>
        /// The person is credited as an actor.
        /// </summary>
        Actor = 1,
        /// <summary>
        /// The person is credited as a director.
        /// </summary>
        Director = 2,
        /// <summary>
        /// The person is credited as a writer.
        /// </summary>
        Writer = 4,
        /// <summary>
        /// The person is credited as a producer
        /// </summary>
        Producer = 8
    }
}
