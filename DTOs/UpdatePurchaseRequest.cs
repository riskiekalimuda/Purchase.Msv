using Purchase.Msv.Models;

namespace Purchase.Msv.DTOs
{
    public class UpdatePurchaseRequest
    {
        public Guid Id { get; set; }

        public string PurchaseNumber { get; set; } = null!;

        public Guid SupplierId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public List<UpdatePurchaseRequestDetail> UpdatePurchaseDetailRequest { get; set; } = new List<UpdatePurchaseRequestDetail>();
    }

    public class UpdatePurchaseRequestDetail
    {
        public Guid Id { get; set; }

        public Guid PurchaseId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
