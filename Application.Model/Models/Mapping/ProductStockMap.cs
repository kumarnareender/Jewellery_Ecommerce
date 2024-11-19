using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class ProductStockMap : EntityTypeConfiguration<ProductStock>
    {
        public ProductStockMap()
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

            this.Property(t => t.Unit)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ProductStocks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.StockLocationId).HasColumnName("StockLocationId");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.Unit).HasColumnName("Unit");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ProductStocks)
                .HasForeignKey(d => d.ProductId);
            this.HasRequired(t => t.StockLocation)
                .WithMany(t => t.ProductStocks)
                .HasForeignKey(d => d.StockLocationId);

        }
    }
}
