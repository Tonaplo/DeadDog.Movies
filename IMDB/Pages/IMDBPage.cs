using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    /// <summary>
    /// Represents an IMDb.com webpage.
    /// </summary>
    public class IMDBPage : HTMLPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IMDBPage"/> class.
        /// </summary>
        /// <param name="html">The html-code associated with the <see cref="IMDBPage"/>.</param>
        /// <param name="request">The requested url that is associated with the <see cref="IMDBPage"/>.</param>
        /// <param name="response">The response url that is associated with the <see cref="IMDBPage"/>.</param>
        public IMDBPage(string html, URL request, URL response)
            : base(html, request, response)
        {
        }
    }
}
