using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class PurchaseProductStockMap : EntityTypeConfiguration<PurchaseProductStock>
    {
        public PurchaseProductStockMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.PurchaseId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ProductId)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("PurchaseProductStocks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PurchaseId).HasColumnName("PurchaseId");
            this.Property(t => t.SupplierId).HasColumnName("SupplierId");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.StockLocationId).HasColumnName("StockLocationId");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.PurchaseProductStocks)
                .HasForeignKey(d => d.ProductId);
            this.HasRequired(t => t.Purchase)
                .WithMany(t => t.PurchaseProductStocks)
                .HasForeignKey(d => d.PurchaseId);
            this.HasRequired(t => t.StockLocation)
                .WithMany(t => t.PurchaseProductStocks)
                .HasForeignKey(d => d.StockLocationId);
            this.HasRequired(t => t.Supplier)
                .WithMany(t => t.PurchaseProductStocks)
                .HasForeignKey(d => d.SupplierId);

        }
    }
}
