using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace Purchase.Msv.Models
{

    public partial class PurchaseMsvDbContext : DbContext
    {
        public PurchaseMsvDbContext()
        {
        }

        public PurchaseMsvDbContext(DbContextOptions<PurchaseMsvDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TrxPurchase> TrxPurchases { get; set; }    

        public virtual DbSet<TrxPurchaseDetail> TrxPurchaseDetails { get; set; }    

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Name=ConnectionStrings:PurchaseMsvDBConnection");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrxPurchase>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("trx_purchase_pkey");

                entity.ToTable("trx_purchase");

                entity.HasIndex(e => e.PurchaseNumber, "trx_purchase_purchase_number_key").IsUnique();

                entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
                entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("purchase_date");
                entity.Property(e => e.PurchaseNumber)
                .HasMaxLength(50)
                .HasColumnName("purchase_number");
                entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
                entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_amount");
            });

            modelBuilder.Entity<TrxPurchaseDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("trx_purchase_detail_pkey");

                entity.ToTable("trx_purchase_detail");

                entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.TotalPrice)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("((quantity)::numeric * unit_price)", true)
                .HasColumnName("total_price");
                entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .HasColumnName("unit_price");

                entity.HasOne(d => d.Purchase).WithMany(p => p.TrxPurchaseDetails)
                .HasForeignKey(d => d.PurchaseId)
                .HasConstraintName("trx_purchase_detail_purchase_id_fkey");
            });

            this.OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
