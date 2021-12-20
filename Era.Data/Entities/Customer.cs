using System;
using System.Collections.Generic;

#nullable disable

namespace Era.Data.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public bool IsCompany { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
