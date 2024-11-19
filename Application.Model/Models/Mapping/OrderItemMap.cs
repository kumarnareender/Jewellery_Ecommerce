using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Application.Model.Models.Mapping
{
    public class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            //this.Property(t => t.Id)
            //    .IsRequired()
            //    .HasMaxLength(128);

            this.Property(t => t.OrderId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ProductId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ImageUrl)
                .HasMaxLength(200);

            this.Property(t => t.Title)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("OrderItems");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OrderId).HasColumnName("OrderId");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Discount).HasColumnName("Discount");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.TotalPrice).HasColumnName("TotalPrice");
            this.Property(t => t.ImageUrl).HasColumnName("ImageUrl");
            this.Property(t => t.ActionDate).HasColumnName("ActionDate");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.CostPrice).HasColumnName("CostPrice");

            // Relationships
            this.HasRequired(t => t.Order)
                .WithMany(t => t.OrderItems)
                .HasForeignKey(d => d.OrderId);
            this.HasRequired(t => t.Product)
                .WithMany(t => t.OrderItems)
                .HasForeignKey(d => d.ProductId);

        }
    }
}
