using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
        {
        public RoleRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IRoleRepository : IRepository<Role>
    {
        
    }
}