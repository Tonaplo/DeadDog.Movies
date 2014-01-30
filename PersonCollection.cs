using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class PersonCollection
    {
        private Dictionary<int, Person> persons;

        public PersonCollection()
        {
            this.persons = new Dictionary<int, Person>();
        }

        public Person GetPerson(int id, string name)
        {
            if (persons.ContainsKey(id))
                return persons[id];
            else
            {
                Person p = new Person(id, name);
                persons.Add(id, p);
                return p;
            }
        }

        public bool Contains(Person item)
        {
            return persons.ContainsKey(item.Id);
        }

        public Person this[int index]
        {
            get { return persons[index]; }
        }
    }
}
