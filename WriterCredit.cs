using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class WriterCredit : PersonCredit
    {
        public WriterCredit(Person person)
            : base(person, CreditTypes.Writer)
        {
        }
    }
}
