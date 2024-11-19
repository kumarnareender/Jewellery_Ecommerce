using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Code)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ShortCode)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Barcode)
                .HasMaxLength(100);

            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .HasMaxLength(2000);

            this.Property(t => t.Unit)
                .HasMaxLength(50);

            this.Property(t => t.DiscountType)
                .HasMaxLength(50);

            this.Property(t => t.Status)
                .HasMaxLength(50);

            this.Property(t => t.Condition)
                .HasMaxLength(100);

            this.Property(t => t.Color)
                .HasMaxLength(100);

            this.Property(t => t.Capacity)
                .HasMaxLength(100);

            this.Property(t => t.Manufacturer)
                .HasMaxLength(100);

            this.Property(t => t.ModelNumber)
                .HasMaxLength(200);

            this.Property(t => t.WarrantyPeriod)
                .HasMaxLength(100);

            this.Property(t => t.IMEI)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Products");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.BranchId).HasColumnName("BranchId");
            this.Property(t => t.SupplierId).HasColumnName("SupplierId");
            this.Property(t => t.ItemTypeId).HasColumnName("ItemTypeId");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.ShortCode).HasColumnName("ShortCode");
            this.Property(t => t.Barcode).HasColumnName("Barcode");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsFeatured).HasColumnName("IsFeatured");
            this.Property(t => t.CostPrice).HasColumnName("CostPrice");
            this.Property(t => t.RetailPrice).HasColumnName("RetailPrice");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.Unit).HasColumnName("Unit");
            this.Property(t => t.IsDiscount).HasColumnName("IsDiscount");
            this.Property(t => t.DiscountType).HasColumnName("DiscountType");
            this.Property(t => t.LowStockAlert).HasColumnName("LowStockAlert");
            this.Property(t => t.ViewCount).HasColumnName("ViewCount");
            this.Property(t => t.SoldCount).HasColumnName("SoldCount");
            this.Property(t => t.ExpireDate).HasColumnName("ExpireDate");
            this.Property(t => t.IsInternal).HasColumnName("IsInternal");
            this.Property(t => t.IsFastMoving).HasColumnName("IsFastMoving");
            this.Property(t => t.IsMainItem).HasColumnName("IsMainItem");
            this.Property(t => t.IsApproved).HasColumnName("IsApproved");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
            this.Property(t => t.IsSync).HasColumnName("IsSync");
            this.Property(t => t.Condition).HasColumnName("Condition");
            this.Property(t => t.Color).HasColumnName("Color");
            this.Property(t => t.Capacity).HasColumnName("Capacity");
            this.Property(t => t.Manufacturer).HasColumnName("Manufacturer");
            this.Property(t => t.ModelNumber).HasColumnName("ModelNumber");
            this.Property(t => t.WarrantyPeriod).HasColumnName("WarrantyPeriod");
            this.Property(t => t.IsFrozen).HasColumnName("IsFrozen");
            this.Property(t => t.IMEI).HasColumnName("IMEI");
            this.Property(t => t.WholesalePrice).HasColumnName("WholesalePrice");
            this.Property(t => t.OnlinePrice).HasColumnName("OnlinePrice");
            this.Property(t => t.RetailDiscount).HasColumnName("RetailDiscount");
            this.Property(t => t.WholesaleDiscount).HasColumnName("WholesaleDiscount");
            this.Property(t => t.OnlineDiscount).HasColumnName("OnlineDiscount");

            // Relationships
            this.HasRequired(t => t.Branch)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.BranchId);
            this.HasRequired(t => t.Category)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.CategoryId);
            this.HasOptional(t => t.ItemType)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.ItemTypeId);
            this.HasRequired(t => t.User)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.UserId);
            this.HasOptional(t => t.Supplier)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.SupplierId);

        }
    }
}
