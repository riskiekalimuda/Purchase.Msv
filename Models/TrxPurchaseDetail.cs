using System;
using System.Collections.Generic;


namespace Purchase.Msv.Models
{

    public partial class TrxPurchaseDetail
    {
        public Guid Id { get; set; }

        public Guid PurchaseId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public virtual TrxPurchase Purchase { get; set; } = null!;
    }
}