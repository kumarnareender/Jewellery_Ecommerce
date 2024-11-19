using Application.Data.Infrastructure;
using Application.Data.Models;
using Application.Model.Models;
using System;
using System.Linq.Expressions;
namespace Application.Data.Repository
{
    public class ActionLogRepository : RepositoryBase<ActionLog>, IActionLogRepository
        {
        public ActionLogRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }        
        }
    public interface IActionLogRepository : IRepository<ActionLog>
    {
        
    }
}