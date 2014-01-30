using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class ProducerCredit : PersonCredit
    {
        public ProducerCredit(Person person)
            : base(person, CreditTypes.Producer)
        {
        }
    }
}
