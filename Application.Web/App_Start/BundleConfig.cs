using System.Configuration;
using System.Web.Optimization;

namespace Application.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            string theme = ConfigurationManager.AppSettings["SiteTheme"];

            // Css
            bundles.Add(new StyleBundle("~/css").Include(
                    "~/Content/Vendor/bootstrap.min.css",
                    "~/Assets/css/responsive.css",
                    "~/Assets/css/colors/blue.css",
                    "~/Assets/css/fontello.css",
                    "~/Content/style.css",
                    "~/Content/top-menu.css",
                    "~/Content/Vendor/dataTables.bootstrap.min.css",
                    "~/Content/Plugin/slick-theme.css",
                    "~/Content/Plugin/slick.css",
                    "~/Assets/css/style.css",
                    "~/Assets/css/datepicker.css",
                    "~/Content/app-view.css",
                    "~/Content/Themes/" + theme + ".css"
                ));

            // Js vendors
            bundles.Add(new ScriptBundle("~/vendor").Include(
                   "~/Scripts/Vendor/jquery-2.1.1.min.js",
                   "~/Scripts/Vendor/jquery-ui.min.js",
                   "~/Scripts/Plugin/jquery.twbsPagination.js",
                   "~/Scripts/Plugin/moment.js",
                   "~/Assets/js/bootstrap-datepicker.min.js",
                   "~/Assets/js/jquery.migrate.js",
                   "~/Assets/js/modernizrr.js",
                   "~/Assets/js/bootstrap.js",
                   "~/Scripts/Plugin/jquery.dataTables.min.js",
                   "~/Assets/js/jquery.dataTables.bootstrap.js",
                   "~/Scripts/Plugin/jquery-dateFormat.min.js",
                   "~/Scripts/Plugin/bootbox.min.js",
                   "~/Scripts/Plugin/slick.min.js",
                   "~/Scripts/Plugin/masonry.pkgd.min.js",
                   "~/Scripts/Plugin/imagesloaded.pkgd.min.js",
                   "~/Scripts/Vendor/jquery.plugin.min.js",
                   "~/Scripts/Plugin/jquery.timeentry.min.js",
                   "~/Scripts/Plugin/ace.js",
                   "~/Scripts/Plugin/jquery.slimscroll.min.js",
                   "~/Scripts/Plugin/dataTables.fixedColumns.min.js",
                   "~/Scripts/Plugin/dataTables.fixedHeader.min.js",
                   "~/Scripts/Vendor/highcharts.js",
                   "~/Scripts/Vendor/highcharts-exporting.js",
                   "~/Scripts/Plugin/html5-qrcode.min.js",
                   "~/Assets/js/jquery.validate.min.js"

                   ));

            // Js Apps
            bundles.Add(new ScriptBundle("~/app").Include(
                   "~/Scripts/Config.js",
                   "~/Scripts/App/enum.js",
                   "~/Scripts/App/App.js",
                   "~/Scripts/App/common.js",
                   "~/Scripts/Controller/Shared/HeaderSearchBar.js",
                   "~/Scripts/Controller/Account/ChangePassword.js",
                   "~/Scripts/Controller/Account/AccountInfo.js",
                   "~/Scripts/Controller/Account/ChangeUsername.js",
                   "~/Scripts/Controller/Notification/Notification.js",
                   "~/Scripts/Controller/Photo/ManagePhoto.js",
                   "~/Scripts/Controller/Photo/SiteLogo.js",
                   "~/Scripts/Controller/Product/Search.js",
                   "~/Scripts/Controller/Security/ForgotPassword.js",
                   "~/Scripts/Controller/Security/Login.js",
                   "~/Scripts/Controller/Static/ContactUs.js",
                   "~/Scripts/Controller/ProductEntry/Post.js",
                   "~/Scripts/Controller/ProductEntry/EditPost.js",
                   "~/Scripts/Controller/ProductEntry/StockUpdate.js",
                   "~/Scripts/Controller/ProductEntry/EditCategory.js",
                   "~/Scripts/Controller/Home/Index.js",
                   "~/Scripts/Controller/Product/Details.js",
                   "~/Scripts/Controller/Cart/ShoppingCart.js",
                   "~/Scripts/Controller/Customer/OrderList.js",
                   "~/Scripts/Controller/Customer/OrderDetails.js",
                   "~/Scripts/Controller/Order/OrderList.js",
                   "~/Scripts/Controller/Order/OrderDetails.js",
                   "~/Scripts/Controller/Order/SalesReturn.js",
                   "~/Scripts/Controller/PurchaseOrder/SalesReturn.js",
                   "~/Scripts/Controller/Branch/Branch.js",
                   "~/Scripts/Controller/City/City.js",
                   "~/Scripts/Controller/Promotion/Promotion.js",
                   "~/Scripts/Controller/Reports/DailySales.js",
                   "~/Scripts/Controller/Reports/SalesSummary.js",
                   "~/Scripts/Controller/Account/RegisterAccount.js",
                   "~/Scripts/Controller/Supplier/Supplier.js",
                   "~/Scripts/Controller/ItemType/ItemType.js",
                   "~/Scripts/Controller/UserManagement/UserList.js",
                   "~/Scripts/Controller/UserManagement/CreateUser.js",
                   "~/Scripts/Controller/Admin/ProductList.js",
                   "~/Scripts/Controller/Category/Category.js",
                   "~/Scripts/Controller/Category/CategoryPhoto.js",
                   "~/Scripts/Controller/Lookup/Lookup.js",
                   "~/Scripts/Controller/Admin/Dashboard.js",
                   "~/Scripts/Controller/Settings/Settings.js",
                   "~/Scripts/Controller/StockLocation/StockLocation.js",
                   "~/Scripts/Controller/Purchase/ProductStock.js",
                   "~/Scripts/Controller/Purchase/LowStockEntry.js",
                   "~/Scripts/Controller/Purchase/LowStockList.js",
                   "~/Scripts/Controller/Reports/MonthlySalesChart.js",
                   "~/Scripts/Controller/Reports/DailySalesChart.js",
                   "~/Scripts/Controller/Reports/ActivityLog.js",
                   "~/Scripts/Controller/Admin/CustomerAdd.js",
                   "~/Scripts/Controller/Admin/CustomerList.js",
                   "~/Scripts/Controller/HomeSlider/HomeSlider.js",
                   "~/Scripts/Controller/Testimony/Testimony.js",
                   "~/Scripts/Controller/Testimony/UpdateTestimonies.js",
                   "~/Scripts/Controller/AboutUs/AboutUs.js",
                   "~/Scripts/Controller/AboutUs/CreateAboutUs.js",
                   "~/Scripts/Controller/ContactUs/ContactUs.js",
                   "~/Scripts/Controller/WholeSale/AddWholesaleOrder.js",
                   "~/Scripts/Controller/ProductUpload/UploadExcel.js",
                    "~/Scripts/Controller/PurchaseOrder/AddPurchaseOrder.js",
                    "~/Scripts/Controller/PurchaseOrder/PurchaseOrderList.js",
                    "~/Scripts/Controller/PurchaseOrder/PurchaseOrderDetails.js",
                    "~/Scripts/Controller/OrderPaymentStatus/PaymentsOrderDetails.js",
                    "~/Scripts/Controller/Reports/PaymentReport.js",
                    "~/Scripts/Controller/RestaurantTable/RestTable.js"
                   ));

            // Enable bundling 
            BundleTable.EnableOptimizations = false;
        }
    }
}
