using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class ProductImageMap : EntityTypeConfiguration<ProductImage>
    {
        public ProductImageMap()
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

            this.Property(t => t.ImageName)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("ProductImages");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ImageName).HasColumnName("ImageName");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
            this.Property(t => t.IsPrimaryImage).HasColumnName("IsPrimaryImage");
            this.Property(t => t.IsApproved).HasColumnName("IsApproved");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");

            // Relationships
            this.HasRequired(t => t.Product)
                .WithMany(t => t.ProductImages)
                .HasForeignKey(d => d.ProductId);

        }
    }
}
