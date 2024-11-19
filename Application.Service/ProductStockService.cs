using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface IProductStockService
    {
        void SaveProductStock(ProductStock productStock);
        void UpdateProductStock(int stockLocationId, string productId, int quantity);
        ProductStock GetProductStock(int stockLocationId, string productId);
        List<ProductStock> GetProductStocks(int stockLocationId);
        void Commit();
    }

    public class ProductStockService : IProductStockService
    {
        private readonly IProductStockRepository productStockRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProductStockService(IProductStockRepository productStockRepository, IUnitOfWork unitOfWork)
        {
            this.productStockRepository = productStockRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IProductStockService Members

        public void SaveProductStock(ProductStock productStock)
        {
            var data = this.GetProductStock(productStock.StockLocationId, productStock.ProductId);
            if (data == null)
            {
                productStockRepository.Add(productStock);
                Commit();
            }
            else
            {
                data.Quantity = data.Quantity + productStock.Quantity;
                productStockRepository.Update(data);
                Commit();
            }            
        }

        public void UpdateProductStock(int stockLocationId, string productId, int quantity)
        {
            var data = this.GetProductStock(stockLocationId, productId);
            if (data != null)
            {
                data.Quantity = quantity;
                productStockRepository.Update(data);
                Commit();
            }            
        }

        public ProductStock GetProductStock(int stockLocationId, string productId)
        {
            var productStock = productStockRepository.GetMany(r => r.StockLocationId == stockLocationId && r.ProductId == productId).FirstOrDefault();
            return productStock;
        }

        public List<ProductStock> GetProductStocks(int stockLocationId)
        {
            var list = productStockRepository.GetMany(r => r.StockLocationId == stockLocationId).ToList();
            return list;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
