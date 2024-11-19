using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class ProductStockRepository : RepositoryBase<ProductStock>, IProductStockRepository
        {
        public ProductStockRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IProductStockRepository : IRepository<ProductStock>
    {
        
    }
}