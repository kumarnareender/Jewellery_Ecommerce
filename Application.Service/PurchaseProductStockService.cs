using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface IPurchaseProductStockService
    {
        void CreatePurchaseProductStock(PurchaseProductStock purchaseProductStock);        
        void Commit();
    }

    public class PurchaseProductStockService : IPurchaseProductStockService
    {
        private readonly IPurchaseProductStockRepository purchaseProductStockRepository;
        private readonly IUnitOfWork unitOfWork;

        public PurchaseProductStockService(IPurchaseProductStockRepository purchaseProductStockRepository, IUnitOfWork unitOfWork)
        {
            this.purchaseProductStockRepository = purchaseProductStockRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IPurchaseProductStockService Members

        public void CreatePurchaseProductStock(PurchaseProductStock purchaseProductStock)
        {
            purchaseProductStockRepository.Add(purchaseProductStock);
            Commit();
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
