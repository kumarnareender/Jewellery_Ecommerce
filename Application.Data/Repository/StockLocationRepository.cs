using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class StockLocationRepository : RepositoryBase<StockLocation>, IStockLocationRepository
        {
        public StockLocationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IStockLocationRepository : IRepository<StockLocation>
    {
        
    }
}