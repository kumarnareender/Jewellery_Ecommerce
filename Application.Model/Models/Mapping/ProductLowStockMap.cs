using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class ProductLowStockMap : EntityTypeConfiguration<ProductLowStock>
    {
        public ProductLowStockMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ProductId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.AddedBy)
                .HasMaxLength(100);

            this.Property(t => t.UpdateBy)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ProductLowStocks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.LowStockQuantity).HasColumnName("LowStockQuantity");
            this.Property(t => t.LowStockEntryDate).HasColumnName("LowStockEntryDate");
            this.Property(t => t.IsOrder).HasColumnName("IsOrder");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdateBy).HasColumnName("UpdateBy");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ProductLowStocks)
                .HasForeignKey(d => d.ProductId);

        }
    }
}
