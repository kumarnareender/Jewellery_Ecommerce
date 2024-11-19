using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
        {
        public SettingRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface ISettingRepository : IRepository<Setting>
    {
        
    }
}