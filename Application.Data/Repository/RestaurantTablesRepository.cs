using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class RestaurantTablesRepository : RepositoryBase<RestaurantTable>, IRestaurantTablesRepository
    {
        public RestaurantTablesRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IRestaurantTablesRepository : IRepository<RestaurantTable>
    {

    }
}