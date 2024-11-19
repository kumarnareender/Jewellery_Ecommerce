using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class OrderStatusRepository : RepositoryBase<OrderStatus>, IOrderStatusRepository
    {
        public OrderStatusRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IOrderStatusRepository : IRepository<OrderStatus>
    {

    }
}