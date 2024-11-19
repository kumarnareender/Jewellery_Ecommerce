using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class ContactUsRepository : RepositoryBase<Feedback>, IContactUsRepository
    {
        public ContactUsRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IContactUsRepository : IRepository<Feedback>
    {
        
    }
}