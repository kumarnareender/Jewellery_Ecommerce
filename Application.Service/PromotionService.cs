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

    public interface IPromotionService
    {
        void CreatePromotion(Promotions promotions);
        void UpdatePromotion(Promotions promotions);
        void DeletePromotion(Promotions promotions);
        IEnumerable<Promotions> GetPromotionList();
        Promotions GetPromotion(int id);
        void Commit();
    }

    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository promotionRepository;
        private readonly IUnitOfWork unitOfWork;

        public PromotionService(IPromotionRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.promotionRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreatePromotion(Promotions promotions)
        {
            this.promotionRepository.Add(promotions);
            Commit();
        }
        public void UpdatePromotion(Promotions promotions)
        {
            this.promotionRepository.Update(promotions);
            Commit();
        }
        public void DeletePromotion(Promotions promotion)
        {
            this.promotionRepository.Delete(promotion);
            Commit();
        }

        public IEnumerable<Promotions> GetPromotionList()
        {
            return this.promotionRepository.GetAll().OrderBy(r => r.Coupon).ToList();
        }

        public Promotions GetPromotion(int id)
        {
            var promotion = promotionRepository.Get(r => r.Id == id);
            return promotion;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
