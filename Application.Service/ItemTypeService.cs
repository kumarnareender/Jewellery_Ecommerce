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

    public interface IItemTypeService
    {
        void CreateItemType(ItemType itemType);
        void UpdateItemType(ItemType itemType);
        void DeleteItemType(ItemType itemType);
        IEnumerable<ItemType> GetItemTypeList();
        ItemType GetItemType(int id);
        void Commit();
    }

    public class ItemTypeService : IItemTypeService
    {
        private readonly IItemTypeRepository itemTypeRepository;
        private readonly IUnitOfWork unitOfWork;

        public ItemTypeService(IItemTypeRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.itemTypeRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreateItemType(ItemType itemType)
        {
            this.itemTypeRepository.Add(itemType);
            Commit();
        }
        public void UpdateItemType(ItemType itemType)
        {
            this.itemTypeRepository.Update(itemType);
            Commit();
        }
        public void DeleteItemType(ItemType itemType)
        {
            this.itemTypeRepository.Delete(itemType);
            Commit();
        }

        public IEnumerable<ItemType> GetItemTypeList()
        {
            return this.itemTypeRepository.GetAll().OrderBy(r => r.Name).ToList();
        }

        public ItemType GetItemType(int id)
        {
            var itemType = itemTypeRepository.Get(r => r.Id == id);
            return itemType;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
