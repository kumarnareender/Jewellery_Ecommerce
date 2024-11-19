using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
        {
        public UserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IUserRepository : IRepository<User>
    {
        
    }
}