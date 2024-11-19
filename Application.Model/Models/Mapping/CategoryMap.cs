using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Category");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ParentId).HasColumnName("ParentId");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
            this.Property(t => t.IsPublished).HasColumnName("IsPublished");

            // Relationships
            this.HasOptional(t => t.Category2)
                .WithMany(t => t.Category1)
                .HasForeignKey(d => d.ParentId);

        }
    }
}
