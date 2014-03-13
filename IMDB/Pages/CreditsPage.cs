using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Movies.IMDB
{
    public class CreditsPage : ExtraInfoPage, IEnumerable<PersonCredit>
    {
        public CreditsPage(string html, URL request, URL response, MovieId id, PersonCollection personCollection)
            : base(html, request, response, id)
        {
            this.Cast = new ParsedCollection<Role>(parseCast(html, personCollection));
            this.Directors = new ParsedCollection<DirectorCredit>(parseDirectors(html, personCollection));
            this.Producers = new ParsedCollection<ProducerCredit>(parseProducers(html, personCollection));
            this.Writers = new ParsedCollection<WriterCredit>(parseWriters(html, personCollection));
        }

        private Role[] parseCast(string input, PersonCollection collection)
        {
            if (!input.Contains("<table class=\"cast\">"))
                return new Role[0];

            input = input.CutToFirst("<table class=\"cast\">", CutDirection.Left, false);
            input = input.CutToFirst("</table>", CutDirection.Right, false);

            List<Role> persons = new List<Role>();

            while (input.Contains("<a href=\"/name/nm"))
            {
                input = input.CutToFirst("<a href=\"/name", CutDirection.Left, true);
                string idString = input.CutToFirst(">", CutDirection.Right, true);
                idString = idString.CutToSection("/nm", "/", true);
                int id = int.Parse(idString);

                string name = input.CutToFirst("</span>", CutDirection.Right, true);
                name = name.CutToFirst("<span class=\"itemprop\" itemprop=\"name\">", CutDirection.Left, true);

                input = input.CutToFirst("<a href=\"/character", CutDirection.Left, true);
                string role = input.CutToFirst("</", CutDirection.Right, true);
                if (role.Contains('>'))
                    role = role.CutToLast('>', CutDirection.Left, true);

                if (role.Contains("(as "))
                    role = role.CutToFirst("(as ", CutDirection.Right, true, 1);

                Person personParser = collection.GetPerson(id, decodeHTML(name));
                persons.Add(new Role(personParser, decodeHTML(role), ActorTypes.ActorOther));
            }

            return persons.ToArray();
        }
        private DirectorCredit[] parseDirectors(string input, PersonCollection collection)
        {
            if (!input.Contains("Directed by"))
                return new DirectorCredit[0];

            input = input.CutToFirst("Directed by", CutDirection.Left, false);
            input = input.CutToFirst("</table>", CutDirection.Right, false);

            List<DirectorCredit> persons = new List<DirectorCredit>();

            while (input.Contains("<a href=\"/name/nm"))
            {
                input = input.CutToFirst("<a href=\"/name/nm", CutDirection.Left, true);
                string idString = input.CutToFirst("/", CutDirection.Right, true);
                string name = input.CutToSection("\" >", "</a>", true);
                int id = int.Parse(idString);
                Person personParser = collection.GetPerson(id, decodeHTML(name));
                persons.Add(new DirectorCredit(personParser));
            }

            return persons.ToArray();
        }
        private ProducerCredit[] parseProducers(string input, PersonCollection collection)
        {
            if (!input.Contains("Produced by"))
                return new ProducerCredit[0];

            input = input.CutToFirst("Produced by", CutDirection.Left, false);
            input = input.CutToFirst("</table>", CutDirection.Right, false);

            List<ProducerCredit> persons = new List<ProducerCredit>();

            while (input.Contains("<a href=\"/name/nm"))
            {
                input = input.CutToFirst("<a href=\"/name/nm", CutDirection.Left, true);
                string idString = input.CutToFirst("/", CutDirection.Right, true);
                string name = input.CutToSection("/\" >", "</a>", true);
                int id = int.Parse(idString);
                Person personParser = collection.GetPerson(id, decodeHTML(name));
                persons.Add(new ProducerCredit(personParser));
            }

            return persons.ToArray();
        }
        private WriterCredit[] parseWriters(string input, PersonCollection collection)
        {
            if (!input.Contains("Writing credits"))
                return new WriterCredit[0];

            input = input.CutToFirst("Writing credits", CutDirection.Left, false);
            input = input.CutToFirst("</table>", CutDirection.Right, false);

            List<WriterCredit> persons = new List<WriterCredit>();

            while (input.Contains("<a href=\"/name/nm"))
            {
                input = input.CutToFirst("<a href=\"/name/nm", CutDirection.Left, true);
                string idString = input.CutToFirst("/", CutDirection.Right, true);
                string name = input.CutToSection("/\" >", "</a>", true);
                int id = int.Parse(idString);
                Person personParser = collection.GetPerson(id, decodeHTML(name));
                persons.Add(new WriterCredit(personParser));
            }

            return persons.ToArray();
        }

        public readonly ParsedCollection<Role> Cast;
        public readonly ParsedCollection<DirectorCredit> Directors;
        public readonly ParsedCollection<ProducerCredit> Producers;
        public readonly ParsedCollection<WriterCredit> Writers;

        IEnumerator<PersonCredit> IEnumerable<PersonCredit>.GetEnumerator()
        {
            foreach (PersonCredit p in Cast)
                yield return p;
            foreach (PersonCredit p in Directors)
                yield return p;
            foreach (PersonCredit p in Producers)
                yield return p;
            foreach (PersonCredit p in Writers)
                yield return p;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (PersonCredit p in Cast)
                yield return p;
            foreach (PersonCredit p in Directors)
                yield return p;
            foreach (PersonCredit p in Producers)
                yield return p;
            foreach (PersonCredit p in Writers)
                yield return p;
        }

        public override string ToString()
        {
            return string.Format("[{0} actor{1}, {2} director{3}, {4} producer{5}, {6} writer{7}]",
                Cast.Count, Cast.Count == 1 ? string.Empty : "s",
                Directors.Count, Directors.Count == 1 ? string.Empty : "s",
                Producers.Count, Producers.Count == 1 ? string.Empty : "s",
                Writers.Count, Writers.Count == 1 ? string.Empty : "s");
        }
    }
}
