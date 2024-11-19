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

    public interface ICategoryService
    {
        void CreateCategory(Category cat);
        void UpdateCategory(Category cat);
        void DeleteCategory(Category cat);
        IEnumerable<Category> GetCategoryList(bool isParentsOnly = false, bool isPublishedOnly = true);
        IEnumerable<Category> GetHomepageCategoryList();
        Category GetCategory(int id);
        Category GetCategory(string name);
        void Commit();
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        public CategoryService(ICategoryRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.categoryRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreateCategory(Category cat)
        {
            this.categoryRepository.Add(cat);
            Commit();
        }
        public void UpdateCategory(Category cat)
        {
            this.categoryRepository.Update(cat);
            Commit();
        }
        public void DeleteCategory(Category cat)
        {
            this.categoryRepository.Delete(cat);
            Commit();
        }

        public IEnumerable<Category> GetCategoryList(bool isParentsOnly = false, bool isPublishedOnly = true)
        {
            if (isParentsOnly)
            {
                if (isPublishedOnly)
                {
                    return this.categoryRepository.GetMany(r => r.ParentId == null && r.IsPublished == true && r.Name != "Anonymous").OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToList();
                }
                else
                {
                    return this.categoryRepository.GetMany(r => r.ParentId == null && r.Name != "Anonymous").OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToList();
                }                
            }
            else
            {
                if (isPublishedOnly)
                {
                    return this.categoryRepository.GetMany(r => r.IsPublished == true && r.Name != "Anonymous").OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToList();
                }
                else
                {
                    return this.categoryRepository.GetMany(r=> r.Name != "Anonymous").OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToList();
                }
            }
        }

        public IEnumerable<Category> GetHomepageCategoryList()
        {
            return this.categoryRepository.GetMany(r => r.ShowInHomepage == true && r.ImageName != null).OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToList();
        }

        public Category GetCategory(int id)
        {
            var cat = categoryRepository.Get(r => r.Id == id);
            return cat;
        }

        public Category GetCategory(string name)
        {
            var cat = categoryRepository.Get(r => r.Name == name);
            return cat;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
