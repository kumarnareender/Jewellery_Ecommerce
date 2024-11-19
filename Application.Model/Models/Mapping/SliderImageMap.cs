using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class SliderImageMap : EntityTypeConfiguration<SliderImage>
    {
        public SliderImageMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ImageName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Url)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("SliderImages");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ImageName).HasColumnName("ImageName");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
            this.Property(t => t.Url).HasColumnName("Url");
        }
    }
}
