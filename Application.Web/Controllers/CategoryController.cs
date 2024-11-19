using Application.Common;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryService categoryService;
        private IProductService productService;

        public CategoryController(ICategoryService categoryService, IProductService productService)
        {
            this.categoryService = categoryService;
            this.productService = productService;
        }

        public ActionResult ManageCategory()     
        {           
            return View();
        }

        public ActionResult CategoryPhoto()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveCategoryPhoto(string catId)
        {
            bool isSuccess = false;
            
            if (Request.Files == null || Request.Files[0] == null)
                return Json(new
                {
                    isSuccess = false,
                    message = "Please choose a category image!"
                }, JsonRequestBehavior.AllowGet);

            foreach (string name in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[name];

                var fileName = catId + ".jpg";
                var imagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Photos/Categories/"), fileName);

                // Save photo
                file.SaveAs(imagePath);


                if (!String.IsNullOrEmpty(catId))
                {
                    Category category = categoryService.GetCategory(int.Parse(catId));
                    if (category != null)
                    {
                        category.ImageName = fileName;
                        categoryService.UpdateCategory(category);
                    }
                }
                
                isSuccess = true;
            }

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryPhoto(string catId)
        {
            string imageName = String.Empty;

            if (!String.IsNullOrEmpty(catId))
            {
                var category = categoryService.GetCategory(int.Parse(catId));
                if (category != null)
                {
                    imageName = category.ImageName;
                }
            }

            return Json(new
            {
                imageName = imageName                
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCategoryPhoto(string catId)
        {
            bool isSuccess = true;

            try
            {
                var category = categoryService.GetCategory(int.Parse(catId));

                if (category != null)
                {
                    // Delete category image
                    string file = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Photos/Categories/"), category.ImageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    // Update category table
                    category.ImageName = null;
                    categoryService.UpdateCategory(category);
                }
            }
            catch
            {
                isSuccess = false;
            }

            return Json(new
            {
                isSuccess = isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParentCategoryList()
        {
            List<CategoryViewModel> list = new List<CategoryViewModel>();

            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(ConstKey.ckCategories))
            {
                list = (List<CategoryViewModel>)cache.Get(ConstKey.ckCategories);
            }
            else
            {
                List<Category> parentLocList = this.categoryService.GetCategoryList(true).ToList();
                foreach (Category cat in parentLocList)
                {
                    CategoryViewModel lvm = new CategoryViewModel();
                    lvm.Id = cat.Id;
                    lvm.Name = cat.Name;
                    lvm.ParentId = cat.ParentId;
                    lvm.DisplayOrder = cat.DisplayOrder;

                    list.Add(lvm);
                }

                // Store data in the cache
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.SlidingExpiration = TimeSpan.FromDays(1);
                cache.Add(ConstKey.ckCategories, list, cacheItemPolicy);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryTree()
        {
            List<CategoryViewModel> finalLocList = new List<CategoryViewModel>();
            List<Category> catList = this.categoryService.GetCategoryList(false).ToList();

            List<Category> parentList = new List<Category>();
            foreach (Category cat in catList)
            {
                if (cat.ParentId == null)
                {
                    parentList.Add(cat);
                }
            }

            foreach (Category cat in parentList)
            {
                GetAllChildLocs(cat.Id, finalLocList, catList);
            }

            return Json(finalLocList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryName(string productId)
        {
            int categoryId = 0;
            string categoryName = String.Empty;

            var product = this.productService.GetProduct(productId);

            if (product != null)
            {
                categoryId = product.CategoryId;
            }

            string query = String.Format(@"WITH RCTE AS
                                (
                                SELECT Id, ParentId, 1 AS Lvl, cat.Name
                                FROM Category cat WHERE cat.[Id] = {0}
    
                                UNION ALL
    
                                SELECT nextDepth.Id  as ItemId, nextDepth.ParentId as ItemParentId, Lvl+1 AS Lvl, nextDepth.[Name]
                                FROM Category nextDepth
                                INNER JOIN RCTE recursive ON nextDepth.Id = recursive.ParentId
                                )
                                    
                            SELECT Id, ParentId, Lvl as [Level], Name
                            FROM RCTE as hierarchie order by lvl desc ", categoryId);


            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (var context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                var recordList = context.Database.SqlQuery<CategoryTree>(query).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (var record in recordList)
                    {
                        categoryName += record.Name + " / ";
                    }
                }

                if (!String.IsNullOrEmpty(categoryName))
                {
                    categoryName = categoryName.TrimEnd(' ').TrimEnd('/').TrimEnd(' ');
                }
            }

            return Json(categoryName, JsonRequestBehavior.AllowGet);
        }

        private void GetAllChildLocs(int id, List<CategoryViewModel> list, List<Category> originalLocList)
        {
            Category cat = FindCategory(id, originalLocList);

            if (cat != null)
            {
                CategoryViewModel lvm = new CategoryViewModel();
                lvm.Id = cat.Id;
                lvm.Name = cat.Name;
                lvm.ParentId = cat.ParentId;
                lvm.DisplayOrder = cat.DisplayOrder;
                
                list.Add(lvm);

                lvm.ChildCategories = new List<CategoryViewModel>();
                lvm.HasChild = (cat.Category1 != null && cat.Category1.Count() > 0) ? true : false;

                // Sort by 'DisplayOrder' and then 'Name'
                if (cat.Category1 != null && cat.Category1.Count() > 0)
                {
                    cat.Category1 = cat.Category1.OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToList();
                }    

                foreach (Category c in cat.Category1)
                {
                    GetAllChildLocs(c.Id, lvm.ChildCategories, originalLocList);
                }
            }
        }

        private Category FindCategory(int id, List<Category> originalLocList)
        {
            Category catReturn = null;

            foreach (Category cat in originalLocList)
            {
                if (cat.Id == id)
                {
                    catReturn = cat;
                    break;
                }
            }

            return catReturn;
        }

        public JsonResult GetCategory(int id)
        {
            CategoryViewModel cvm = new CategoryViewModel();
            var cat = this.categoryService.GetCategory(id);
            if (cat != null)
            {
                cvm.Id = cat.Id;
                cvm.Name = cat.Name;
                cvm.ParentId = cat.ParentId;
                cvm.IsPublished = cat.IsPublished;
                cvm.DisplayOrder = cat.DisplayOrder;
                cvm.Description = cat.Description;
            }

            return Json(cvm, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategoryList()
        {
            var itemList = this.categoryService.GetCategoryList(false, false);

            List<CategoryViewModel> list = new List<CategoryViewModel>();
            foreach (var cat in itemList)
            {
                CategoryViewModel cvm = new CategoryViewModel();
                cvm.Id = cat.Id;
                cvm.Name = cat.Name;
                cvm.ParentId = cat.ParentId;
                cvm.ParentName = cat.Category2 != null ? cat.Category2.Name : String.Empty;
                cvm.IsPublished = cat.IsPublished;
                cvm.DisplayOrder = cat.DisplayOrder;
                cvm.Description = cat.Description;
                cvm.ShowInHomepage = cat.ShowInHomepage;
                cvm.ImageName = cat.ImageName;
                
                list.Add(cvm);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public JsonResult CreateCategory(Category cat)
        {
            bool isSuccess = true;
            try
            {
                this.categoryService.CreateCategory(cat);

                // Remove category cache
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ConstKey.ckCategories);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public JsonResult UpdateCategory(Category cat)
        {
            bool isSuccess = true;
            try
            {
                this.categoryService.UpdateCategory(cat);

                // Remove category cache
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ConstKey.ckCategories);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public JsonResult DeleteCategory(Category cat)
        {
            bool isSuccess = true;
            try
            {
                this.categoryService.DeleteCategory(cat);

                // Remove category cache
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ConstKey.ckCategories);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }               
    }

    class CategoryTree
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }

    }
}