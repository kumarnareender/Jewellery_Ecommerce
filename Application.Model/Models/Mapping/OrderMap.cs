using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Barcode)
                .HasMaxLength(50);

            this.Property(t => t.OrderMode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OrderStatus)
                .HasMaxLength(50);

            this.Property(t => t.PaymentStatus)
                .HasMaxLength(50);

            this.Property(t => t.PaymentType)
                .HasMaxLength(50);

            this.Property(t => t.ActionBy)
                .HasMaxLength(128);

            this.Property(t => t.DeliveryDate)
                .HasMaxLength(100);

            this.Property(t => t.DeliveryTime)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Orders");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.OrderCode).HasColumnName("OrderCode");
            this.Property(t => t.Barcode).HasColumnName("Barcode");
            this.Property(t => t.PayAmount).HasColumnName("PayAmount");
            this.Property(t => t.DueAmount).HasColumnName("DueAmount");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.Vat).HasColumnName("Vat");
            this.Property(t => t.ShippingAmount).HasColumnName("ShippingAmount");
            this.Property(t => t.ReceiveAmount).HasColumnName("ReceiveAmount");
            this.Property(t => t.ChangeAmount).HasColumnName("ChangeAmount");
            this.Property(t => t.OrderMode).HasColumnName("OrderMode");
            this.Property(t => t.OrderStatus).HasColumnName("OrderStatus");
            this.Property(t => t.PaymentStatus).HasColumnName("PaymentStatus");
            this.Property(t => t.PaymentType).HasColumnName("PaymentType");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
            this.Property(t => t.ActionBy).HasColumnName("ActionBy");
            this.Property(t => t.BranchId).HasColumnName("BranchId");
            this.Property(t => t.DeliveryDate).HasColumnName("DeliveryDate");
            this.Property(t => t.DeliveryTime).HasColumnName("DeliveryTime");
            this.Property(t => t.TotalWeight).HasColumnName("TotalWeight");
            this.Property(t => t.IsFrozen).HasColumnName("IsFrozen");

            // Relationships
            this.HasOptional(t => t.Branch)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.BranchId);
            this.HasRequired(t => t.User)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.UserId);

        }
    }
}
