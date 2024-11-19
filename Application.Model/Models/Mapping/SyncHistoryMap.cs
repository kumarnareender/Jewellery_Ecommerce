using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class SyncHistoryMap : EntityTypeConfiguration<SyncHistory>
    {
        public SyncHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Title)
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.Type)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SyncHistory");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
        }
    }
}
