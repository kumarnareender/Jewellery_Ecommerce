using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class LookupRepository : RepositoryBase<Lookup>, ILookupRepository
        {
        public LookupRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface ILookupRepository : IRepository<Lookup>
    {
        
    }
}