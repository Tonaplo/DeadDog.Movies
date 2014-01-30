using System;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Movies
{
    public class Person
    {
        private int id;
        private string name;

        internal Person(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int Id
        {
            get { return id; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override string ToString()
        {
            return name;
        }

        public static void Idtostring(int id)
        {
            int decValue = 182;
            // Convert integer 182 as a hex in a string variable
            string hexValue = decValue.ToString("X3");
            // Convert the hex string back to the number
            int decAgain = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
