using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Common;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface IPurchaseService
    {
        void CreatePurchase(Purchase purchase);
        void UpdatePurchase(Purchase purchase);
        void DeletePurchase(Purchase purchase);
        IEnumerable<Purchase> GetPurchaseList();
        Purchase GetPurchase(string id);
        void Commit();
    }

    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IUnitOfWork unitOfWork;

        public PurchaseService(IPurchaseRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.purchaseRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreatePurchase(Purchase purchase)
        {
            this.purchaseRepository.Add(purchase);
            Commit();
        }
        public void UpdatePurchase(Purchase purchase)
        {
            this.purchaseRepository.Update(purchase);
            Commit();
        }
        public void DeletePurchase(Purchase purchase)
        {
            this.purchaseRepository.Delete(purchase);
            Commit();
        }

        public IEnumerable<Purchase> GetPurchaseList()
        {
            return this.purchaseRepository.GetAll().OrderBy(r => r.PurchaseDate).ToList();
        }

        public Purchase GetPurchase(string id)
        {
            var purchase = purchaseRepository.Get(r => r.Id == id);
            return purchase;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
