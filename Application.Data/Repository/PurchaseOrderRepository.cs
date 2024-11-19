using Application.Data.Infrastructure;
using Application.Model.Models;
namespace Application.Data.Repository
{
    public class PurchaseOrderRepository : RepositoryBase<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {

    }
}