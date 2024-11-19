using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class PromotionRepository : RepositoryBase<Promotions>, IPromotionRepository
        {
        public PromotionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IPromotionRepository : IRepository<Promotions>
    {
        
    }
}