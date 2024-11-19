using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{

    public interface IContactUsService
    {
        void AddFeedback(Feedback ContactUs);
        List<Feedback> GetContactUsList();
        void DeleteFeedback(Feedback feedback);
        Feedback GetFeedback(int id);
        void Commit();
    }

    public class ContactUsService : IContactUsService
    {
        private readonly IContactUsRepository ContactUsRepository;
        private readonly IUnitOfWork unitOfWork;

        public ContactUsService(IContactUsRepository classRepository, IUnitOfWork unitOfWork)
        {
            ContactUsRepository = classRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IClassService Members

        public void AddFeedback(Feedback feedback)
        {
            ContactUsRepository.Add(feedback);
            Commit();
        }
        public List<Feedback> GetContactUsList()
        {
            return ContactUsRepository.GetAll().OrderByDescending(r => r.CreatedOn).ThenBy(f => f.Name).ToList();
        }
        public void DeleteFeedback(Feedback feedback)
        {
            ContactUsRepository.Delete(feedback);
            Commit();
        }
        public Feedback GetFeedback(int id)
        {
            var feedback = ContactUsRepository.Get(r => r.Id == id);
            return feedback;
        }
        public void Commit()
        {
            unitOfWork.Commit();
        }
        #endregion
    }
}
