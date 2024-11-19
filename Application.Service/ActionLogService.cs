using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface IActionLogService 
    {
        void CreateActionLog(ActionLog actionLog);
        IEnumerable<ActionLog> GetActionLogs(DateTime fromDate, DateTime toDate);
        ActionLog GetActionLog(string id);
        void Commit();
    }

    public class ActionLogService : IActionLogService
    {
        private readonly IActionLogRepository actionLogRepository;
        private readonly IUnitOfWork unitOfWork;

        public ActionLogService(IActionLogRepository actionLogRepository, IUnitOfWork unitOfWork)
        {
            this.actionLogRepository = actionLogRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IActionLogService Members

        public void CreateActionLog(ActionLog actionLog)
        {
            actionLogRepository.Add(actionLog);
            Commit();
        }

        public IEnumerable<ActionLog> GetActionLogs(DateTime fromDate, DateTime toDate)
        {
            var actionLogs = actionLogRepository.GetMany(r=> r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            return actionLogs;
        }

        public ActionLog GetActionLog(string id)
        {
            var actionLog = actionLogRepository.Get(r => r.Id == id);

            return actionLog;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
