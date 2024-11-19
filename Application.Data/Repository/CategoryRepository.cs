using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
        {
        public CategoryRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface ICategoryRepository : IRepository<Category>
    {
        
    }
}