using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{

    public interface IProductService
    {
        void CreateProduct(Product product);
        IEnumerable<Product> GetProducts();
        IEnumerable<Product> GetUnApprovedProducts();
        IEnumerable<Product> GetHomePageProducts(int count);
        IEnumerable<Product> GetRelatedProducts(int numberOfProducts, int categoryId);
        IEnumerable<ProductViewModel> GetProductsByCategory(int numberOfProducts, string categoryIds);
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        IEnumerable<ProductViewModel> GetMyProducts(string userId, int branchId = 0, string categoryIds = "", int itemTypeId = 0, int supplierId = 0, string attribute = "", string lowStock = "");
        IEnumerable<Product> GetUnSyncProducts(int branchId);
        IEnumerable<Product> GetAllProducts(int branchId);
        IEnumerable<Product> GetStoreNewProducts(string userId, int count);
        Product GetProduct(string id);
        Product GetProductByBarcode(string barcode);
        bool IsBarcodeExists(string barcode, string excludeProductId = "");
        bool IsBarcodeExists(int branchId, string barcode, string excludeProductId = "");
        bool ApproveProduct(string productId, bool isApprove);
        bool DeleteProduct(string productId);
        bool UpdateProduct(Product product);
        void UpdateViewCount(string productId);
        void UpdateSoldCount(string productId);
        void UpdateStockQty(string productId, int quantity);
        void MinusStockQty(string productId, int soldQuantity);
        void AddStockQty(string productId, int soldQuantity, bool lastRecv = false);
        void DeleteIMEIProduct(string productId);
        void MarkIsSync(string productIds);
        decimal GetCostPrice(string productId);
        int GetLowStockItemCount();
        void Commit();
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            this.productRepository = productRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IProductService Members

        public void CreateProduct(Product product)
        {
            productRepository.Add(product);
            Commit();
        }

        public IEnumerable<Product> GetProducts()
        {
            List<Product> products = productRepository.GetMany(r => r.IsApproved == true && r.IsDeleted == false).OrderByDescending(r => r.ActionDate).ToList();
            return products;
        }

        public IEnumerable<Product> GetUnApprovedProducts()
        {
            List<Product> products = productRepository.GetMany(r => r.IsApproved == false && r.IsDeleted == false).OrderByDescending(r => r.ActionDate).ToList();
            return products;
        }

        public IEnumerable<Product> GetHomePageProducts(int count)
        {
            IEnumerable<Product> products = productRepository.GetMany(r => r.ProductImages.Count() > 0 && r.IsApproved == true && r.IsDeleted == false).OrderBy(r => Guid.NewGuid()).ToList().Take(count);
            return products;
        }

        public IEnumerable<Product> GetRelatedProducts(int numberOfProducts, int categoryId)
        {
            List<Product> products = productRepository.GetMany(r => r.CategoryId == categoryId && r.IsApproved == true && r.IsDeleted == false).OrderByDescending(r => r.ActionDate).Take(numberOfProducts).ToList();
            return products;
        }

        public IEnumerable<ProductViewModel> GetProductsByCategory(int numberOfProducts, string categoryIds)
        {
            List<ProductViewModel> list = new List<ProductViewModel>();

            string sql = string.Format("Select top {0} * From Products p Where p.IsDeleted = 0 And CategoryId In ({1}) order by ViewCount desc", numberOfProducts, categoryIds);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                list = context.Database.SqlQuery<ProductViewModel>(sql).ToList();
            }

            return list;
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            if (categoryId != 0)
                return productRepository.GetAll().Where(x => x.CategoryId == categoryId).ToList();
            else
                return productRepository.GetAll().ToList();

        }


        public IEnumerable<ProductViewModel> GetMyProducts(string userId, int branchId = 0, string categoryIds = "", int itemTypeId = 0, int supplierId = 0, string attribute = "", string lowStock = "")
        {
            List<ProductViewModel> list = new List<ProductViewModel>();

            string sql = "select p.* from Products p where p.IsDeleted = 0";

            if (branchId > 0)
            {
                sql += string.Format(" and BranchId = {0} ", branchId);
            }

            if (!string.IsNullOrEmpty(categoryIds))
            {
                sql += " and CategoryId IN (" + categoryIds + ")";
            }

            if (itemTypeId > 0)
            {
                sql += string.Format(" and ItemTypeId = {0} ", itemTypeId);
            }

            if (supplierId > 0)
            {
                sql += string.Format(" and SupplierId = {0} ", supplierId);
            }

            if (!string.IsNullOrEmpty(attribute))
            {
                if (attribute == "Main")
                {
                    sql += " and IsMainItem = 1 ";
                }
                else if (attribute == "FastMoving")
                {
                    sql += " and IsFastMoving = 1 ";
                }
                else if (attribute == "Internal")
                {
                    sql += " and IsInternal = 1 ";
                }
            }

            if (!string.IsNullOrEmpty(lowStock))
            {
                sql += string.Format(" and Quantity <= LowStockAlert ");
            }

            // Exclude anonymous product
            sql += string.Format(" and Id != '00000000-0000-0000-0000-000000000000' ");

            // Order by clause
            sql += " order by ActionDate desc ";

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                list = context.Database.SqlQuery<ProductViewModel>(sql).ToList();
            }

            return list;
        }

        public decimal GetCostPrice(string productId)
        {
            decimal costPrice = 0;
            string sql = string.Format("select ISNULL(CostPrice,0) as CostPrice from Products where Id = '{0}'", productId);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                costPrice = context.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            }
            return costPrice;
        }

        public int GetLowStockItemCount()
        {
            int lowStockCount = 0;
            string sql = string.Format("select count(*) as LowStockCount from products where Quantity <= LowStockAlert and IsDeleted = 0 and Id != '00000000-0000-0000-0000-000000000000'");

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                lowStockCount = context.Database.SqlQuery<int>(sql).FirstOrDefault();
            }
            return lowStockCount;
        }

        public IEnumerable<Product> GetStoreNewProducts(string userId, int count)
        {
            List<Product> products = productRepository.GetMany(r => r.IsApproved == true && r.IsDeleted == false && r.UserId == userId).OrderByDescending(r => r.ActionDate).Take(count).ToList();
            return products;
        }

        public Product GetProduct(string id)
        {
            Product product = productRepository.Get(r => r.Id == id);

            return product;
        }

        public Product GetProductByBarcode(string barcode)
        {
            Product product = productRepository.Get(r => r.Barcode == barcode);

            return product;
        }

        public IEnumerable<Product> GetUnSyncProducts(int branchId)
        {
            IEnumerable<Product> products = productRepository.GetMany(r => r.BranchId == branchId && r.IsSync != true && r.IsDeleted == false);
            return products;
        }

        public IEnumerable<Product> GetAllProducts(int branchId)
        {
            IEnumerable<Product> products = productRepository.GetMany(r => r.BranchId == branchId && r.IsDeleted == false);
            return products;
        }

        public bool IsBarcodeExists(string barcode, string excludeProductId = "")
        {
            Product product;

            if (!string.IsNullOrEmpty(excludeProductId))
            {
                product = productRepository.Get(r => r.Barcode == barcode && r.Id != excludeProductId);
            }
            else
            {
                product = productRepository.Get(r => r.Barcode == barcode);
            }

            bool isExists = product != null ? true : false;
            return isExists;
        }

        public bool IsBarcodeExists(int branchId, string barcode, string excludeProductId = "")
        {
            Product product;

            if (!string.IsNullOrEmpty(excludeProductId))
            {
                product = productRepository.Get(r => r.BranchId == branchId && r.Barcode == barcode && r.Id != excludeProductId);
            }
            else
            {
                product = productRepository.Get(r => r.BranchId == branchId && r.Barcode == barcode);
            }

            bool isExists = product != null ? true : false;
            return isExists;
        }

        public bool ApproveProduct(string productId, bool isApprove)
        {
            Product product = productRepository.Get(r => r.Id == productId);
            if (product != null)
            {
                product.IsApproved = isApprove;
                productRepository.Update(product);
                Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteProduct(string productId)
        {
            Product product = productRepository.Get(r => r.Id == productId);
            if (product != null)
            {
                product.IsDeleted = true;
                product.Barcode = product.Barcode + "_deleted_" + DateTime.Now.Ticks.ToString();
                product.IMEI = product.IMEI + "_deleted_" + DateTime.Now.Ticks.ToString();
                productRepository.Update(product);
                Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateProduct(Product product)
        {
            if (product != null)
            {
                productRepository.Update(product);
                Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateViewCount(string productId)
        {
            string sql = string.Format("Update Products Set ViewCount = ISNULL(ViewCount, 0 ) + 1 where Id = '{0}'", productId);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public void UpdateSoldCount(string productId)
        {
            string sql = string.Format("Update Products Set SoldCount = ISNULL(SoldCount, 0 ) + 1 where Id = '{0}'", productId);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public void UpdateStockQty(string productId, int quantity)
        {
            string sql = string.Format("Update Products Set Quantity = {1} where Id = '{0}'", productId, quantity);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public void MinusStockQty(string productId, int soldQuantity)
        {
            string sql = string.Format("Update Products Set Quantity = ISNULL(Quantity, 0 ) - {1} where Id = '{0}'", productId, soldQuantity);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public void AddStockQty(string productId, int soldQuantity, bool lastRecv = false)
        {
            string sql = string.Format("Update Products Set Quantity = ISNULL(Quantity, 0 ) + {1} where Id = '{0}'", productId, soldQuantity);

            if (lastRecv)
            {
                string lastRecvSql = string.Format("Update Products Set LastReceivedQuantity = {1} where Id = '{0}'", productId, soldQuantity);
                sql = $"{sql}     {lastRecvSql}";
            }


            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public void DeleteIMEIProduct(string productId)
        {
            string sql = string.Format("Update Products Set IsDeleted = 1 where Id = '{0}' and IMEI is not null", productId);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public void MarkIsSync(string productIds)
        {
            string sql = string.Format("Update Products Set IsSync = 1 where Id IN ({0})", productIds);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
