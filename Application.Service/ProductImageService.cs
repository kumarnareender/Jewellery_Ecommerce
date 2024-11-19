using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface IProductImageService
    {
        void CreateProductImage(ProductImage productImage);
        IEnumerable<ProductImage> GetProductImages(string productId, bool isOnlyApproved = true);
        IEnumerable<ProductImage> GetPendingImages();
        ProductImage GetProductImage(string imageId);
        bool SetProfileImage(string productId, string imageId);
        bool ActivateImage(string imageId);
        bool DeleteImage(string imageId);
        void Commit();
    }

    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository productImageRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProductImageService(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork)
        {
            this.productImageRepository = productImageRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IProductImageService Members

        public void CreateProductImage(ProductImage productImage)
        {
            productImageRepository.Add(productImage);
            Commit();
        }

        public IEnumerable<ProductImage> GetProductImages(string productId, bool isOnlyApproved = true)
        {
            List<ProductImage> productImages = new List<ProductImage>();

            if (isOnlyApproved)
            {
                productImages = productImageRepository.GetMany(r => r.ProductId == productId && r.IsApproved == true).OrderByDescending(r => r.IsPrimaryImage).ThenBy(r=> r.DisplayOrder) .ToList();
            }
            else
            {
                productImages = productImageRepository.GetMany(r => r.ProductId == productId).OrderBy(r => r.DisplayOrder).ToList();
            }

            return productImages;
        }

        public IEnumerable<ProductImage> GetPendingImages()
        {
            var productImages = productImageRepository.GetMany(r => r.IsApproved == false).OrderByDescending(r=> r.ActionDate) .ToList();
            return productImages;
        }

        public ProductImage GetProductImage(string imageId)
        {
            var productImage = productImageRepository.Get(r => r.Id == imageId);
            return productImage;
        }

        public bool SetProfileImage(string productId, string imageId)
        {
            bool isSuccess = true;
            string updateAll = String.Format(@"Update ProductImages Set IsPrimaryImage = 0 Where ProductId = '{0}'", productId);
            string updateToProfile = String.Format(@"Update ProductImages Set IsPrimaryImage = 1 Where ProductId = '{0}' And Id = '{1}'", productId, imageId);

            try
            {
                using (var context = new Data.Models.ApplicationEntities())
                {
                    int updateAllSuccess = context.Database.ExecuteSqlCommand(updateAll);
                    int updateProfileSuccess = context.Database.ExecuteSqlCommand(updateToProfile);
                }
            }
            catch
            {
                isSuccess = false;                
            }

            return isSuccess;
        }

        public bool ActivateImage(string imageId)
        {
            var productImage = productImageRepository.Get(u => u.Id == imageId);
            if (productImage != null)
            {
                productImage.IsApproved = true;
                productImageRepository.Update(productImage);
                Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteImage(string imageId)
        {
            bool isSuccess = true;
            string sql = String.Format(@"Delete From ProductImages Where Id = '{0}'", imageId);            

            try
            {
                using (var context = new Data.Models.ApplicationEntities())
                {
                    int result = context.Database.ExecuteSqlCommand(sql);
                }
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
