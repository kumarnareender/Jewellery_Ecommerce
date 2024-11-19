using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class BranchRepository : RepositoryBase<Branch>, IBranchRepository
        {
        public BranchRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IBranchRepository : IRepository<Branch>
    {
        
    }
}