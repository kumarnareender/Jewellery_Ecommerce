using Application.Data.Infrastructure;
using Application.Model.Models;
namespace Application.Data.Repository
{
    public class OrderPaymentStatusRepository : RepositoryBase<OrderPayment>, IOrderPaymentStatusRepository
    {
        public OrderPaymentStatusRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IOrderPaymentStatusRepository : IRepository<OrderPayment>
    {

    }
}