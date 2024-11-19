using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class ProductStockManualUpdateHistoryMap : EntityTypeConfiguration<ProductStockManualUpdateHistory>
    {
        public ProductStockManualUpdateHistoryMap()
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

            this.Property(t => t.Remarks)
                .HasMaxLength(200);

            this.Property(t => t.ActionBy)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ProductStockManualUpdateHistory");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.StockLocationId).HasColumnName("StockLocationId");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
            this.Property(t => t.ActionBy).HasColumnName("ActionBy");
        }
    }
}
