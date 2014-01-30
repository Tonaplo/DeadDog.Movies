using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class DirectorCredit : PersonCredit
    {
        public DirectorCredit(Person person)
            : base(person, CreditTypes.Director)
        {
        }
    }
}
