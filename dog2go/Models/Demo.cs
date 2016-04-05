using System.Collections.Generic;
using TypeLite;

namespace dog2go.Models
{
    [TsClass]
    public class Person
    {
        public string Name { get; set; }
        public List<Address> Addresses { get; set; }
    }
    public class Employee : Person
    {
        public decimal Salary { get; set; }
    }
    public class Address
    {
        public string Street { get; set; }

    }
}