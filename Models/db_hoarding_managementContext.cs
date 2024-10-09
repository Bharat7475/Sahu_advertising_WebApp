using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Hoarding_managment.Models
{
    public partial class db_hoarding_managementContext : DbContext
    {
      
        public db_hoarding_managementContext(DbContextOptions<db_hoarding_managementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblCampaign> TblCampaigns { get; set; } = null!;
        public virtual DbSet<TblCampaignnew> TblCampaignnews { get; set; } = null!;
        public virtual DbSet<TblCampaingitem> TblCampaingitems { get; set; } = null!;
        public virtual DbSet<TblCustomer> TblCustomers { get; set; } = null!;
        public virtual DbSet<TblInventory> TblInventories { get; set; } = null!;
        public virtual DbSet<TblInventoryitem> TblInventoryitems { get; set; } = null!;
        public virtual DbSet<TblQuotation> TblQuotations { get; set; } = null!;
        public virtual DbSet<TblQuotationitem> TblQuotationitems { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;
        public virtual DbSet<TblVendor> TblVendors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<TblCampaign>(entity =>
            {
                entity.ToTable("tbl_campaign");

                entity.HasIndex(e => e.FkCustomerId, "Fk_customerId");

                entity.HasIndex(e => e.FkInventoryId, "Fk_inventoryId");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BookingAmt).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.FkCustomerId).HasColumnName("Fk_customerId");

                entity.Property(e => e.FkInventoryId).HasColumnName("Fk_inventoryId");

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TblCampaigns)
                    .HasForeignKey(d => d.FkCustomerId)
                    .HasConstraintName("tbl_campaign_ibfk_2");
            });

            modelBuilder.Entity<TblCampaignnew>(entity =>
            {
                entity.ToTable("tbl_campaignnew");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampaignNumber).HasMaxLength(45);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.FkCustomerId).HasColumnName("Fk_customerId");

                entity.Property(e => e.IsDelete)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            });

            modelBuilder.Entity<TblCampaingitem>(entity =>
            {
                entity.ToTable("tbl_campaingitem");

                entity.Property(e => e.BookingAmt).HasMaxLength(45);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.FkCampaignId).HasColumnName("Fk_CampaignId");

                entity.Property(e => e.FkInventoryId).HasColumnName("Fk_InventoryId");

                entity.Property(e => e.IsDelete)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");
            });

            modelBuilder.Entity<TblCustomer>(entity =>
            {
                entity.ToTable("tbl_customer");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.AlternateNumber).HasMaxLength(255);

                entity.Property(e => e.BusinessName).HasMaxLength(255);

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.ContactNo).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CustomerName).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.GstNo).HasMaxLength(255);

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.State).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            });

            modelBuilder.Entity<TblInventory>(entity =>
            {
                entity.ToTable("tbl_inventory");

                entity.HasIndex(e => e.FkVendorId, "Fk_vendorId");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Area).HasMaxLength(255);

                entity.Property(e => e.BookingStatus).HasColumnType("bit(1)");

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.FkVendorId).HasColumnName("Fk_vendorId");

                entity.Property(e => e.Height).HasMaxLength(255);

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.Rate).HasMaxLength(255);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);

                entity.Property(e => e.VendorAmt).HasMaxLength(255);

                entity.Property(e => e.Width).HasMaxLength(255);

                entity.HasOne(d => d.FkVendor)
                    .WithMany(p => p.TblInventories)
                    .HasForeignKey(d => d.FkVendorId)
                    .HasConstraintName("tbl_inventory_ibfk_1");
            });

            modelBuilder.Entity<TblInventoryitem>(entity =>
            {
                entity.ToTable("tbl_inventoryitems");

                entity.Property(e => e.Area).HasMaxLength(255);

                entity.Property(e => e.BookingStatus).HasColumnType("bit(1)");

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Fkcustomer).HasColumnName(" FKCustomer");

                entity.Property(e => e.Height).HasMaxLength(45);

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.Rate).HasMaxLength(45);

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Width).HasMaxLength(45);
            });

            modelBuilder.Entity<TblQuotation>(entity =>
            {
                entity.ToTable("tbl_quotation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.FkCustomerId).HasColumnName("Fk_CustomerId");

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.QuotationNumber).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            });

            modelBuilder.Entity<TblQuotationitem>(entity =>
            {
                entity.ToTable("tbl_quotationitems");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Area).HasMaxLength(255);

                entity.Property(e => e.BookingStatus).HasColumnType("bit(1)");

                entity.Property(e => e.City).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.FkCustomerId).HasColumnName("Fk_CustomerId");

                entity.Property(e => e.FkInventory).HasColumnName("Fk_Inventory");

                entity.Property(e => e.FkQuotationId).HasColumnName("Fk_QuotationId");

                entity.Property(e => e.FkVendorId).HasColumnName("Fk_VendorId");

                entity.Property(e => e.Height).HasMaxLength(255);

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.LocationDescription).HasMaxLength(255);

                entity.Property(e => e.MarginAmt).HasMaxLength(255);

                entity.Property(e => e.Rate).HasMaxLength(255);

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);

                entity.Property(e => e.VendorAmt).HasMaxLength(255);

                entity.Property(e => e.Width).HasMaxLength(255);
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.ToTable("tbl_user");

                entity.HasIndex(e => e.UserName, "UserName")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);
            });

            modelBuilder.Entity<TblVendor>(entity =>
            {
                entity.ToTable("tbl_vendor");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.AlternateNumber).HasMaxLength(255);

                entity.Property(e => e.BusinessName).HasMaxLength(255);

                entity.Property(e => e.ContactNo).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.GstNo).HasMaxLength(255);

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.State).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);

                entity.Property(e => e.VendorName).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
