using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class BranchMap : EntityTypeConfiguration<Branch>
    {
        public BranchMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Branch");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.IsAllowOnline).HasColumnName("IsAllowOnline");

            // Relationships
            this.HasMany(t => t.Users)
                .WithMany(t => t.Branches)
                .Map(m =>
                    {
                        m.ToTable("UserBranches");
                        m.MapLeftKey("BranchId");
                        m.MapRightKey("UserId");
                    });


        }
    }
}
