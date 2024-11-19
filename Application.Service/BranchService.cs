using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{

    public interface IBranchService
    {
        void CreateBranch(Branch branch);
        void UpdateBranch(Branch branch);
        void DeleteBranch(Branch branch);
        IEnumerable<Branch> GetBranchList();
        List<Branch> GetBranchList(string userId);
        Branch GetBranch(int id);
        Branch GetBranchByName(string name);
        void Commit();
    }

    public class BranchService : IBranchService
    {
        private readonly IBranchRepository branchRepository;
        private readonly IUnitOfWork unitOfWork;

        public BranchService(IBranchRepository classRepository, IUnitOfWork unitOfWork)
        {
            branchRepository = classRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IClassService Members

        public void CreateBranch(Branch branch)
        {
            branchRepository.Add(branch);
            Commit();
        }
        public void UpdateBranch(Branch branch)
        {
            branchRepository.Update(branch);
            Commit();
        }
        public void DeleteBranch(Branch branch)
        {
            branchRepository.Delete(branch);
            Commit();
        }

        public IEnumerable<Branch> GetBranchList()
        {
            return branchRepository.GetAll().OrderBy(r => r.Name).ToList();
        }

        public List<Branch> GetBranchList(string userId)
        {
            List<Branch> list = new List<Branch>();
            string sql = string.Format(@"Select * from Branch b, UserBranches ub where b.Id = ub.BranchId and ub.UserId = '{0}'", userId);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                list = context.Database.SqlQuery<Branch>(sql).ToList();
            }

            return list;
        }

        public Branch GetBranch(int id)
        {
            Branch branch = branchRepository.Get(r => r.Id == id);
            return branch;
        }
        public Branch GetBranchByName(string name)
        {
            Branch branch = branchRepository.Get(r => r.Name == name);
            return branch;
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
