using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class PurchaseMap : EntityTypeConfiguration<Purchase>
    {
        public PurchaseMap()
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

            this.Property(t => t.InvoiceNo)
                .HasMaxLength(100);

            this.Property(t => t.Unit)
                .HasMaxLength(50);

            this.Property(t => t.Remarks)
                .HasMaxLength(500);

            this.Property(t => t.ActionBy)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Purchase");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.SupplierId).HasColumnName("SupplierId");
            this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Weight).HasColumnName("Weight");
            this.Property(t => t.Unit).HasColumnName("Unit");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            this.Property(t => t.TaxPerc).HasColumnName("TaxPerc");
            this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
            this.Property(t => t.ActionBy).HasColumnName("ActionBy");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.Purchases)
                .HasForeignKey(d => d.ProductId);
            this.HasRequired(t => t.Supplier)
                .WithMany(t => t.Purchases)
                .HasForeignKey(d => d.SupplierId);

        }
    }
}
