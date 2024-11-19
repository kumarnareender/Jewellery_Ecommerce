using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Username)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.FirstName)
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .HasMaxLength(50);

            this.Property(t => t.ShipAddress)
                .HasMaxLength(500);

            this.Property(t => t.ShipZipCode)
                .HasMaxLength(50);

            this.Property(t => t.ShipCity)
                .HasMaxLength(50);

            this.Property(t => t.ShipState)
                .HasMaxLength(50);

            this.Property(t => t.ShipCountry)
                .HasMaxLength(50);

            this.Property(t => t.PhotoUrl)
                .HasMaxLength(200);

            this.Property(t => t.Code)
                .HasMaxLength(200);

            this.Property(t => t.Permissions)
                .HasMaxLength(500);

            this.Property(t => t.CustomerCode)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("Users");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Username).HasColumnName("Username");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.ShipAddress).HasColumnName("ShipAddress");
            this.Property(t => t.ShipZipCode).HasColumnName("ShipZipCode");
            this.Property(t => t.ShipCity).HasColumnName("ShipCity");
            this.Property(t => t.ShipState).HasColumnName("ShipState");
            this.Property(t => t.ShipCountry).HasColumnName("ShipCountry");
            this.Property(t => t.PhotoUrl).HasColumnName("PhotoUrl");
            this.Property(t => t.LastLoginTime).HasColumnName("LastLoginTime");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.IsManual).HasColumnName("IsManual");
            this.Property(t => t.IsVerified).HasColumnName("IsVerified");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
            this.Property(t => t.IsSync).HasColumnName("IsSync");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Permissions).HasColumnName("Permissions");
            this.Property(t => t.CustomerCode).HasColumnName("CustomerCode");
        }
    }
}
