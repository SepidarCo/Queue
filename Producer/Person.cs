using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    public class Person
    {
        public Person(int id, string name, string family)
        {
            this.Id = id;
            this.Name = name;
            this.Family = family;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }
    }
}
