using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
        {
        public ProductRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IProductRepository : IRepository<Product>
    {
        
    }
}