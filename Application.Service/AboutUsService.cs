using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Common;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface IAboutUsService
    {
        void CreateAboutUs(AboutUs aboutus);
        void UpdateAboutUs(AboutUs aboutus);
        void DeleteAboutUs(AboutUs aboutus);
        List<AboutUs> GetAboutUsList();
        AboutUs GetAboutUs(int id);
        void Commit();
    }

    public class AboutUsService : IAboutUsService
    {
        private readonly IAboutUsRepository aboutusRepository;
        private readonly IUnitOfWork unitOfWork;

        public AboutUsService(IAboutUsRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.aboutusRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreateAboutUs(AboutUs aboutus)
        {
            this.aboutusRepository.Add(aboutus);
            Commit();
        }
        public void UpdateAboutUs(AboutUs aboutus)
        {
            this.aboutusRepository.Update(aboutus);
            Commit();
        }
        public void DeleteAboutUs(AboutUs aboutus)
        {
            this.aboutusRepository.Delete(aboutus);
            Commit();
        }

        public List<AboutUs> GetAboutUsList()
        {
            return this.aboutusRepository.GetAll().OrderBy(r => r.Description).ToList();
        }

        public AboutUs GetAboutUs(int id)
        {
            var aboutus = aboutusRepository.Get(r => r.Id == id);
            return aboutus;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
