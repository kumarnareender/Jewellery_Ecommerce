using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class TestimonyRepository : RepositoryBase<Testimony>, ITestimonyRepository
    {
        public TestimonyRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface ITestimonyRepository : IRepository<Testimony>
    {
        
    }
}