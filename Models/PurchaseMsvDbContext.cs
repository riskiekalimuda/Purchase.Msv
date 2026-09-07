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

        public virtual DbSet<Purchase> Purchases { get; set; }    

        public virtual DbSet<Purchasedetail> Purchasedetails { get; set; }    

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Name=ConnectionStrings:PurchaseMsvDBConnection");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("purchases_pkey");

                entity.ToTable("purchases");

                entity.HasIndex(e => e.Purchasenumber, "purchases_purchasenumber_key").IsUnique();

                entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
                entity.Property(e => e.Purchasedate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("purchasedate");
                entity.Property(e => e.Purchasenumber)
                .HasMaxLength(50)
                .HasColumnName("purchasenumber");
                entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
                entity.Property(e => e.Supplierid).HasColumnName("supplierid");
                entity.Property(e => e.Totalamount)
                .HasPrecision(18, 2)
                .HasColumnName("totalamount");
            });

            modelBuilder.Entity<Purchasedetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("purchasedetails_pkey");

                entity.ToTable("purchasedetails");

                entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
                entity.Property(e => e.Productid).HasColumnName("productid");
                entity.Property(e => e.Purchaseid).HasColumnName("purchaseid");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.Totalprice)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("((quantity)::numeric * unitprice)", true)
                .HasColumnName("totalprice");
                entity.Property(e => e.Unitprice)
                .HasPrecision(18, 2)
                .HasColumnName("unitprice");

                entity.HasOne(d => d.Purchase).WithMany(p => p.Purchasedetails)
                .HasForeignKey(d => d.Purchaseid)
                .HasConstraintName("purchasedetails_purchaseid_fkey");
            });

            this.OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
