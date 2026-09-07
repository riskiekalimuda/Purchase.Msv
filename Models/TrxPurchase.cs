using System;
using System.Collections.Generic;


namespace Purchase.Msv.Models
{

    public partial class TrxPurchase
    {
        public Guid Id { get; set; }

        public string PurchaseNumber { get; set; } = null!;

        public Guid SupplierId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<TrxPurchaseDetail> TrxPurchaseDetails { get; set; } = new List<TrxPurchaseDetail>();
    }
}