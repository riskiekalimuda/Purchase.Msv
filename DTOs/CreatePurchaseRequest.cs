namespace Purchase.Msv.DTOs
{

        // DTO Utama Pembelian
        public class CreatePurchaseRequest
        {
            public string PurchaseNumber { get; set; } = string.Empty;
            public Guid SupplierId { get; set; }
            public DateTime PurchaseDate { get; set; }
            public decimal TotalAmount { get; set; }
            public List<CreatePurchaseDetail> Details { get; set; } = new List<CreatePurchaseDetail>();
        }   

        public class CreatePurchaseDetail
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal? TotalPrice { get; set; }
        }
}
