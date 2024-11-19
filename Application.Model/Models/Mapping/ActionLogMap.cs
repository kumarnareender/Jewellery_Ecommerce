using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class ActionLogMap : EntityTypeConfiguration<ActionLog>
    {
        public ActionLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Module)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.Value)
                .HasMaxLength(200);

            this.Property(t => t.ActionType)
                .HasMaxLength(50);

            this.Property(t => t.ActionBy)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ActionLogs");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Module).HasColumnName("Module");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.ActionType).HasColumnName("ActionType");
            this.Property(t => t.ActionBy).HasColumnName("ActionBy");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
        }
    }
}
