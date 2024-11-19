using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class CityRepository : RepositoryBase<City>, ICityRepository
        {
        public CityRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface ICityRepository : IRepository<City>
    {
        
    }
}