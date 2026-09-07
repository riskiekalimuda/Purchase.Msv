using System;
using System.Collections.Generic;


namespace Purchase.Msv.Models
{

    public partial class Purchase
    {
        public Guid Id { get; set; }

        public string Purchasenumber { get; set; } = null!;

        public Guid Supplierid { get; set; }

        public DateTime? Purchasedate { get; set; }

        public decimal Totalamount { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<Purchasedetail> Purchasedetails { get; set; } = new List<Purchasedetail>();
    }
}