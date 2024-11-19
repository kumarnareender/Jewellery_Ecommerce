using Application.Common;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Web.Mvc;


namespace Application.Controllers
{

    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private ISettingService settingService;
        private readonly IProductService productService;
        private readonly IProductImageService productImageService;
        private ICategoryService categoryService;

        public HomeController(IUserService userService, IProductService productService, IProductImageService productImageService, ISettingService settingService, ICategoryService categoryService)
        {
            this.userService = userService;
            this.productService = productService;
            this.productImageService = productImageService;
            this.categoryService = categoryService;
            this.settingService = settingService;

            ReadSettingValues();

        }

        public ActionResult Index()
        {
            return View();
        }

        private void ReadSettingValues()
        {
            List<Setting> settingList = new List<Setting>();
            ObjectCache cache = MemoryCache.Default;
            if (!cache.Contains(ConstKey.ckSettings))
            {
                // Get all settings from DB
                settingList = settingService.GetSettings().ToList();

                // Store data in the cache
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
                {
                    SlidingExpiration = TimeSpan.FromDays(1)
                };
                cache.Add(ConstKey.ckSettings, settingList, cacheItemPolicy);
            }
        }

        public JsonResult GetCategoryWithImage()
        {
            List<HomePageCategoriesModel> itemList = new List<HomePageCategoriesModel>();

            IEnumerable<Category> catList = categoryService.GetHomepageCategoryList();
            foreach (Category item in catList)
            {
                HomePageCategoriesModel model = new HomePageCategoriesModel
                {
                    CategoryId = item.Id.ToString(),
                    Title = item.Name,
                    ImageName = item.ImageName
                };

                itemList.Add(model);
            }

            Random rnd = new Random();
            itemList = itemList.OrderBy(x => rnd.Next()).ToList();

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHomepageCategoryItems(bool isLoadProducts = true)
        {
            List<HomePageCategoriesModel> itemList = new List<HomePageCategoriesModel>();

            IEnumerable<Category> catList = categoryService.GetHomepageCategoryList();
            foreach (Category item in catList)
            {
                HomePageCategoriesModel model = new HomePageCategoriesModel
                {
                    CategoryId = item.Id.ToString(),
                    Title = item.Name,
                    ImageName = item.ImageName
                };

                if (isLoadProducts)
                {
                    model.ProductList = AppUtils.GetHomepageCategoryItems(item.Id);
                }

                itemList.Add(model);
            }

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerList()
        {
            List<CustomerViewModel> itemList = Application.Web.AppUtils.GetHomePage_Customers();

            return new JsonResult()
            {
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json",
                Data = itemList,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }
        public JsonResult GetHomePage_Products(int categoryId)
        {
            try
            {
                var data = productService.GetProductsByCategory(categoryId);

                List<ProductViewModel> products = new List<ProductViewModel>();

                foreach (Product item in data)
                {
                    ProductViewModel product = new ProductViewModel()
                    {
                        Id = item.Id.ToString(),
                        Weight = item.Weight,
                        Title = item.Title,
                        RetailPrice = item.RetailPrice,
                        CostPrice = item.CostPrice,
                        Gst = item.Gst,
                        RetailDiscount = item.RetailDiscount,
                        Description = item.Description,
                        Unit = item.Unit,
                        Category = new CategoryViewModel()
                        {
                            Name = item.Category.Name,
                            Id = item.Category.Id,
                        },
                        IsSoldbyweight = item.IsSoldbyweight
                    };
                    products.Add(product);
                }

                //List<HomePageItem> itemList = new List<HomePageItem>();
                //itemList = Application.Web.AppUtils.GetHomePage_Products(categoryId);
                //Random rnd = new Random();
                //itemList = itemList.OrderBy(x => rnd.Next()).ToList();

                return Json(new { Products = products, isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Products = new List<ProductViewModel>(), isSuccess = false }, JsonRequestBehavior.AllowGet);

            }

        }
        public JsonResult SetBranch(int branchId)
        {
            try
            {
                Application.Common.Utils.SetCookie("branchId", branchId.ToString());
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Products = new List<ProductViewModel>(), isSuccess = false }, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult GetHomePage_FeaturedItems()
        {
            List<HomePageItem> itemList = new List<HomePageItem>();
            itemList = Application.Web.AppUtils.GetHomePage_FeaturedItems();
            Random rnd = new Random();
            itemList = itemList.OrderBy(x => rnd.Next()).ToList();

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHomePage_PopularItems()
        {
            List<HomePageItem> itemList = new List<HomePageItem>();
            itemList = Application.Web.AppUtils.GetHomePage_PopularItems();
            Random rnd = new Random();
            itemList = itemList.OrderBy(x => rnd.Next()).ToList();

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHomePage_NewArrivals()
        {
            List<HomePageItem> itemList = new List<HomePageItem>();
            itemList = Application.Web.AppUtils.GetHomePage_NewArrivals();
            Random rnd = new Random();
            itemList = itemList.OrderBy(x => rnd.Next()).ToList();

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }
    }

    internal class HomePageCategoryItemSetting
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}