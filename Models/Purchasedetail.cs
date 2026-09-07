using System;
using System.Collections.Generic;


namespace Purchase.Msv.Models
{

    public partial class Purchasedetail
    {
        public Guid Id { get; set; }

        public Guid Purchaseid { get; set; }

        public Guid Productid { get; set; }

        public int Quantity { get; set; }

        public decimal Unitprice { get; set; }

        public decimal? Totalprice { get; set; }

        public virtual Purchase Purchase { get; set; } = null!;
    }
}