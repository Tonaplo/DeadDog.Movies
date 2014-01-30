using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public abstract class PersonCredit
    {
        private Person person;
        private CreditTypes type;

        public PersonCredit(Person person, CreditTypes type)
        {
            this.person = person;
            this.type = type;
        }

        public Person Person
        {
            get { return person; }
        }
        public CreditTypes CreditType
        {
            get { return type; }
        }

        public override string ToString()
        {
            return person.ToString();
        }
    }
}
