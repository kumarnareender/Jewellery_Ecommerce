using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class ItemTypeRepository : RepositoryBase<ItemType>, IItemTypeRepository
        {
        public ItemTypeRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IItemTypeRepository : IRepository<ItemType>
    {
        
    }
}