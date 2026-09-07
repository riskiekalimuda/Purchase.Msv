using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Purchase.Msv.Models
{
    partial class PurchaseMsvDbContext: DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.AddTransactionalOutboxEntities();
        }
    }
}
