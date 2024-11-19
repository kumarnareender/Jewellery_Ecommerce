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

    public interface ILookupService
    {
        void CreateLookup(Lookup lookup);
        void UpdateLookup(Lookup lookup);
        void DeleteLookup(Lookup lookup);
        IEnumerable<Lookup> GetLookupList(string name);
        Lookup GetLookup(int id);
        void Commit();
    }

    public class LookupService : ILookupService
    {
        private readonly ILookupRepository lookupRepository;
        private readonly IUnitOfWork unitOfWork;

        public LookupService(ILookupRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.lookupRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreateLookup(Lookup lookup)
        {
            this.lookupRepository.Add(lookup);
            Commit();
        }
        public void UpdateLookup(Lookup lookup)
        {
            this.lookupRepository.Update(lookup);
            Commit();
        }
        public void DeleteLookup(Lookup lookup)
        {
            this.lookupRepository.Delete(lookup);
            Commit();
        }

        public IEnumerable<Lookup> GetLookupList(string name)
        {
            return this.lookupRepository.GetMany(r=> r.Name.ToLower() == name.ToLower()).OrderBy(r => r.Name).ToList();
        }

        public Lookup GetLookup(int id)
        {
            var lookup = lookupRepository.Get(r => r.Id == id);
            return lookup;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
