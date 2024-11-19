using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
        {
        public OrderRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IOrderRepository : IRepository<Order>
    {
        
    }
}