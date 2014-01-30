using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class Role : PersonCredit
    {
        private string rolename;
        private ActorTypes actortype;

        public Role(Person person, string rolename, ActorTypes actortype)
            : base(person, CreditTypes.Actor)
        {
            this.rolename = rolename;
            this.actortype = actortype;
        }

        public bool HasRolename
        {
            get { return rolename == null || rolename.Length == 0; }
        }
        public string Rolename
        {
            get { return rolename; }
            set { rolename = value; }
        }
        public ActorTypes ActorType
        {
            get { return actortype; }
            set { actortype = value; }
        }

        public override string ToString()
        {
            return base.ToString() + " (" + rolename + ")";
        }
    }
}
