using System;
using System.Collections.Generic;

#nullable disable

namespace Era.Data.Entities
{
    public partial class Product
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
