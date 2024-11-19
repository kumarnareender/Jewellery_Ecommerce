using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class LookupMap : EntityTypeConfiguration<Lookup>
    {
        public LookupMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Lookups");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Value).HasColumnName("Value");
        }
    }
}
