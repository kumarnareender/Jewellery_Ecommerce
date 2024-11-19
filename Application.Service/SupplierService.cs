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

    public interface ISupplierService
    {
        void CreateSupplier(Supplier supplier);
        void UpdateSupplier(Supplier supplier);
        void DeleteSupplier(Supplier supplier);
        IEnumerable<Supplier> GetSupplierList();
        Supplier GetSupplier(int id);
        void Commit();
    }

    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository supplierRepository;
        private readonly IUnitOfWork unitOfWork;

        public SupplierService(ISupplierRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.supplierRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreateSupplier(Supplier supplier)
        {
            this.supplierRepository.Add(supplier);
            Commit();
        }
        public void UpdateSupplier(Supplier supplier)
        {
            this.supplierRepository.Update(supplier);
            Commit();
        }
        public void DeleteSupplier(Supplier supplier)
        {
            this.supplierRepository.Delete(supplier);
            Commit();
        }

        public IEnumerable<Supplier> GetSupplierList()
        {
            return this.supplierRepository.GetAll().OrderBy(r => r.Name).ToList();
        }

        public Supplier GetSupplier(int id)
        {
            var supplier = supplierRepository.Get(r => r.Id == id);
            return supplier;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
