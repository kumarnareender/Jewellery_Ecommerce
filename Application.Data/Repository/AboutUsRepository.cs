using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class AboutUsRepository : RepositoryBase<AboutUs>, IAboutUsRepository
    {
        public AboutUsRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IAboutUsRepository : IRepository<AboutUs>
    {
        
    }
}