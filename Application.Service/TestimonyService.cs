using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{

    public interface ITestimonyService
    {
        void CreateTestimony(Testimony testimony);
        void UpdateTestimony(Testimony testimony);
        void DeleteTestimony(Testimony testimony);
        List<Testimony> GetTestimonyList();
        IEnumerable<Testimony> GetActiveTestimonies();
        Testimony GetTestimony(int id);
        void Commit();
    }

    public class TestimonyService : ITestimonyService
    {
        private readonly ITestimonyRepository testimonyRepository;
        private readonly IUnitOfWork unitOfWork;

        public TestimonyService(ITestimonyRepository classRepository, IUnitOfWork unitOfWork)
        {
            testimonyRepository = classRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IClassService Members

        public void CreateTestimony(Testimony testimony)
        {
            testimonyRepository.Add(testimony);
            Commit();
        }
        public void UpdateTestimony(Testimony testimony)
        {
            testimonyRepository.Update(testimony);
            Commit();
        }
        public void DeleteTestimony(Testimony testimony)
        {
            testimonyRepository.Delete(testimony);
            Commit();
        }

        public List<Testimony> GetTestimonyList()
        {
            return testimonyRepository.GetAll().OrderBy(r => r.Description).ToList();
        }

        public IEnumerable<Testimony> GetActiveTestimonies()
        {
            return testimonyRepository.GetMany(t => t.IsActive).OrderBy(r => r.Description).ToList();
        }


        public Testimony GetTestimony(int id)
        {
            Testimony testimony = testimonyRepository.Get(r => r.Id == id);
            return testimony;
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
