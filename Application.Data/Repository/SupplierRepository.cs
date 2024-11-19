using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class SupplierRepository : RepositoryBase<Supplier>, ISupplierRepository
        {
        public SupplierRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface ISupplierRepository : IRepository<Supplier>
    {
        
    }
}