using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IO
{
    public static class FrameIdentifierExtension
    {
        public static IEnumerable<FrameIdentifier> IsFrameType(this IEnumerable<FrameIdentifier> collection, FrameTypes type)
        {
            foreach (FrameIdentifier fi in collection)
                if (fi.IsFrameType(type))
                    yield return fi;
        }
    }
}
