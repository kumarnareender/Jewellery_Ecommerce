using Application.Model.Models;
using Application.Model.Models.Mapping;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Application.Data.Models
{
    public class ApplicationEntities : DbContext
    {

        public ApplicationEntities() : base("DBConnection") { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ELMAH_Error> ELMAH_Error { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<SupplierPayment> SupplierPayments { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<SyncHistory> SyncHistories { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<StockLocation> StockLocations { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<PurchaseProductStock> PurchaseProductStocks { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<Promotions> Promotion { get; set; }
        public DbSet<AboutUs> Aboutus { get; set; }
        public DbSet<Testimony> Testimonies { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<RestaurantTable> RestaurantTables { get; set; }

        public virtual void Commit()
        {
            try
            {
                base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                foreach (DbEntityValidationResult failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (DbValidationError error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                // Add the original exception as the innerException
                throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new BranchMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new ELMAH_ErrorMap());
            modelBuilder.Configurations.Add(new ProductImageMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderItemMap());
            modelBuilder.Configurations.Add(new SettingMap());
            modelBuilder.Configurations.Add(new SupplierMap());
            modelBuilder.Configurations.Add(new ItemTypeMap());
            modelBuilder.Configurations.Add(new SyncHistoryMap());
            modelBuilder.Configurations.Add(new LookupMap());
            modelBuilder.Configurations.Add(new PurchaseMap());
            modelBuilder.Configurations.Add(new StockLocationMap());
            modelBuilder.Configurations.Add(new ProductStockMap());
            modelBuilder.Configurations.Add(new PurchaseProductStockMap());
            modelBuilder.Configurations.Add(new ActionLogMap());
            modelBuilder.Configurations.Add(new SliderImageMap());
        }
    }
}