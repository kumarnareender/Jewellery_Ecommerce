using Application.Common;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web.App_Code;
using Application.Web.ReportViewModel;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Application.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IUserService userService;
        private IOrderService orderService;
        private IProductService productService;
        private IProductImageService productImageService;
        private IActionLogService actionLogService;
        private IRestaurantTablesService restaurantTablesService;

        public OrderController(IUserService userService, IOrderService orderService, IProductService productService, IProductImageService productImageService,
            IActionLogService actionLogService, IRestaurantTablesService restaurantTablesService)
        {
            this.userService = userService;
            this.orderService = orderService;
            this.productService = productService;
            this.productImageService = productImageService;
            this.actionLogService = actionLogService;
            this.restaurantTablesService = restaurantTablesService;
        }

        public ActionResult OrderList()
        {
            return View();
        }

        public ActionResult OrderDetails()
        {
            return View();
        }

        public ActionResult SalesReturn()
        {
            return View();
        }

        public JsonResult CompleteOrder(string orderId)
        {
            bool isSuccess = orderService.CompleteOrder(orderId);
            var table = restaurantTablesService.GetRestTableByOrderId(orderId);
            if (isSuccess && table != null)
            {
                table.OrderId = null;
                table.IsOccupied = false;
                restaurantTablesService.UpdateRestTable(table);
            }
            return Json(new
            {
                isSuccess = isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderList(int? branchId, string fromDate, string toDate, string orderStatus, string orderMode)
        {
            DateTime dtFrom;
            DateTime dtTo;

            if (String.IsNullOrEmpty(fromDate) || String.IsNullOrEmpty(toDate))
            {
                dtFrom = DateTime.Now.AddDays(-30);
                dtTo = DateTime.Now.AddDays(1);
            }
            else
            {
                dtFrom = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dtTo = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddTicks(-1).AddDays(1);
            }

            List<OrderViewModel> orderVMList = new List<OrderViewModel>();

            if (Utils.GetLoggedInUser() != null)
            {
                //EOrderStatus orderStatusEnum = (EOrderStatus)Enum.Parse(typeof(EOrderStatus), orderStatus);
                EOrderMode orderModeEnum = String.IsNullOrEmpty(orderMode) ? EOrderMode.All : (EOrderMode)Enum.Parse(typeof(EOrderMode), orderMode);

                //var orderList = this.orderService.GetOrders(branchId, dtFrom, dtTo, orderStatusEnum, orderModeEnum);
                IEnumerable<Order> orderList = null;
                string userId = Utils.GetLoggedInUserId();

                var user = userService.GetUserById(userId);

                if (user.Roles.Where(x => x.Name == ERoleName.admin.ToString()).Any())
                {
                    //branchId = 0;
                    orderList = this.orderService.GetOrders(dtFrom, dtTo);
                }
                else if (user.Roles.Where(x => x.Name == ERoleName.salesperson.ToString()).Any()
                     || user.Roles.Where(x => x.Name == ERoleName.manager.ToString()).Any())
                {
                    branchId = Utils.GetBranchId();
                    orderList = this.orderService.GetOrdersByUser(branchId.Value, dtFrom, dtTo, userId);

                }


                //branchId = branchId == null ? 0 : branchId;






                foreach (var item in orderList)
                {
                    OrderViewModel order = new OrderViewModel();
                    order.Id = item.Id;
                    order.BranchId = item.BranchId;
                    order.OrderCode = item.OrderCode;
                    order.Discount = item.Discount == null ? 0 : item.Discount;
                    order.DueAmount = item.DueAmount == null ? 0 : item.DueAmount;
                    order.PayAmount = item.PayAmount == null ? 0 : item.PayAmount;
                    order.OrderMode = item.OrderMode;
                    order.OrderStatus = item.OrderStatus;
                    order.PaymentStatus = item.PaymentStatus;
                    order.PaymentType = item.PaymentType;
                    order.Vat = item.Vat == null ? 0 : item.Vat;
                    order.ActionDate = item.ActionDate;
                    order.ActionDateString = Utils.GetFormattedDate(item.ActionDate);
                    order.TableNumber = item.TableNumber != null ? item.TableNumber.Value : 0;
                    orderVMList.Add(order);
                }
            }

            return Json(orderVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineOrdersForDashboard()
        {
            List<OrderViewModel> orderVMList = new List<OrderViewModel>();
            IEnumerable<Order> orderList = null;
            var user = userService.GetUserById(Utils.GetLoggedInUserId());

            if (user != null)
            {
                if (user.Roles.Where(x => x.Name == ERoleName.admin.ToString()).Any())
                {
                    orderList = this.orderService.GetOnlineOrders(10);
                }
                else
                {
                    orderList = this.orderService.GetOnlineOrdersByUsers(Utils.GetLoggedInUserId(), Utils.GetBranchId(), 10);
                }

                foreach (var item in orderList)
                {
                    OrderViewModel order = new OrderViewModel();
                    order.Id = item.Id;
                    order.BranchId = item.BranchId;
                    order.OrderCode = item.OrderCode;
                    order.Discount = item.Discount;
                    order.DueAmount = item.DueAmount;
                    order.PayAmount = item.PayAmount;
                    order.OrderMode = item.OrderMode;
                    order.OrderStatus = item.OrderStatus;
                    order.PaymentStatus = item.PaymentStatus;
                    order.PaymentType = item.PaymentType;
                    order.Vat = item.Vat;
                    order.ActionDate = item.ActionDate;
                    order.ActionDateString = Utils.GetFormattedDate(order.ActionDate);

                    orderVMList.Add(order);
                }
            }

            return Json(orderVMList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult PrintOrder(string orderId)
        {
            List<ReportOrderViewModel> list = new List<ReportOrderViewModel>();

            string sVat = Utils.GetSetting(ESetting.Vat.ToString());
            int vatPerc = String.IsNullOrEmpty(sVat) ? 0 : int.Parse(sVat);

            var order = this.orderService.GetOrder(orderId);
            if (order != null)
            {
                var user = this.userService.GetUserById(order.UserId);

                if (user != null)
                {
                    decimal vatAmount = order.Vat != null ? (decimal)order.Vat : 0;
                    decimal shippingAmount = order.ShippingAmount != null ? (decimal)order.ShippingAmount : 0;
                    decimal subTotal = (decimal)order.PayAmount + (decimal)order.Discount - vatAmount - shippingAmount;
                    decimal discount = (decimal)order.Discount;
                    decimal grandTotal = (decimal)order.PayAmount;

                    foreach (var item in order.OrderItems)
                    {
                        ReportOrderViewModel r = new ReportOrderViewModel();

                        decimal itemTotal = (item.Quantity * item.Price);

                        r.CompanyName = r.CompanyName = Utils.GetSetting(ESetting.CompanyName.ToString());
                        r.Name = user.FirstName + " " + user.LastName;
                        //r.OrderId = order.OrderCode;
                        r.Address = user.ShipAddress;
                        r.Mobile = user.Mobile;
                        r.City = user.ShipCity;
                        r.State = user.ShipState;
                        r.Country = user.ShipCountry;
                        r.Zipcode = user.ShipZipCode;
                        r.RegistrationNo = Utils.GetSetting(ESetting.RegistrationNo.ToString());

                        r.ProductName = item.ProductId == Guid.Empty.ToString() ? item.Title : item.Product.Title;
                        r.ProductBarcode = item.Product.Barcode;
                        r.Quantity = item.Quantity;
                        r.Price = item.Price;
                        r.ItemTotal = itemTotal;

                        r.OrderStatus = order.OrderStatus;
                        r.OrderDate = ((DateTime)order.ActionDate).ToString("dddd, dd MMMM yyyy hh:mm tt");
                        r.ImageName = item.ImageUrl;
                        r.SubTotal = subTotal;
                        r.Discount = discount;
                        r.VatAmount = vatAmount;
                        r.ShippingAmount = shippingAmount;
                        r.GrandTotal = grandTotal;
                        r.PaymentBy = order.PaymentType;

                        r.BarcodeImageName = System.Web.HttpContext.Current.Server.MapPath("~") + "/Photos/Barcode/Orders/" + order.OrderCode + ".jpeg";

                        r.OrderMode = order.OrderMode;
                        r.TotalWeight = order.TotalWeight != null ? (decimal)order.TotalWeight + " Kg" : "";
                        r.DeliveryDateTime = order.DeliveryDate + " " + order.DeliveryTime;
                        r.FrozenItem = order.IsFrozen != null ? ((bool)order.IsFrozen ? "Yes" : "No") : "";

                        list.Add(r);
                    }
                }
            }

            // Report
            if (list.Count() > 0)
            {
                ReportDocument rd = null;
                try
                {
                    rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ProductOrder.rpt"));
                    rd.SetDataSource(list);

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();


                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "ProductOrder.pdf");
                }
                catch (Exception exp)
                {
                    throw exp;
                }
                finally
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return null;
        }

        [AllowAnonymous]
        public ActionResult PrintInvoice(string orderId)
        {
            List<ReportOrderViewModel> list = new List<ReportOrderViewModel>();

            string sVat = Utils.GetSetting(ESetting.Vat.ToString());
            int vatPerc = String.IsNullOrEmpty(sVat) ? 0 : int.Parse(sVat);

            var order = this.orderService.GetOrder(orderId);
            if (order != null)
            {
                var user = this.userService.GetUserById(order.UserId);

                if (user != null)
                {
                    decimal vatAmount = order.Vat != null ? (decimal)order.Vat : 0;
                    decimal shippingAmount = order.ShippingAmount != null ? (decimal)order.ShippingAmount : 0;
                    decimal subTotal = (decimal)order.PayAmount + (decimal)order.Discount - vatAmount - shippingAmount;
                    decimal discount = (decimal)order.Discount;
                    decimal grandTotal = (decimal)order.PayAmount;

                    // Sort the order based on title
                    order.OrderItems = order.OrderItems.OrderBy(r => r.Product.Title).ToList();

                    foreach (var item in order.OrderItems)
                    {
                        ReportOrderViewModel r = new ReportOrderViewModel();

                        decimal itemTotal = (item.Quantity * item.Price);

                        r.CompanyName = Utils.GetSetting(ESetting.CompanyName.ToString());
                        r.CompanyAddress = Utils.GetSetting(ESetting.CompanyAddress.ToString());
                        r.CompanyAddress1 = Utils.GetSetting(ESetting.CompanyAddress1.ToString());
                        r.CompanyPhone = Utils.GetSetting(ESetting.CompanyPhone.ToString());
                        r.FooterLine1 = Utils.GetSetting(ESetting.FooterLine1.ToString());
                        r.FooterLine2 = Utils.GetSetting(ESetting.FooterLine2.ToString());
                        r.RegistrationNo = Utils.GetSetting(ESetting.RegistrationNo.ToString());
                        //r.OrderId = order.OrderCode;

                        r.ProductName = item.ProductId == Guid.Empty.ToString() ? item.Title : item.Product.Title;
                        r.ProductBarcode = item.Product.Barcode;
                        r.Quantity = item.Quantity;
                        r.Price = item.Price;
                        r.ItemTotal = itemTotal;

                        r.OrderStatus = order.OrderStatus;
                        r.OrderDate = ((DateTime)order.ActionDate).ToString("dddd, dd MMMM yyyy hh:mm tt");
                        r.ImageName = item.ImageUrl;
                        r.SubTotal = subTotal;
                        r.Discount = discount;
                        r.VatAmount = vatAmount;
                        r.ShippingAmount = shippingAmount;
                        r.GrandTotal = grandTotal;
                        r.PaymentBy = order.PaymentType;

                        r.BarcodeImageName = System.Web.HttpContext.Current.Server.MapPath("~") + "/Photos/Barcode/Orders/" + order.OrderCode + ".jpeg";
                        r.CompanyLogo = System.Web.HttpContext.Current.Server.MapPath("~") + "/Images/Logo/Logo.png";

                        r.OrderMode = order.OrderMode;
                        r.TotalWeight = order.TotalWeight != null ? (decimal)order.TotalWeight + " Kg" : "";
                        r.DeliveryDateTime = order.DeliveryDate + " " + order.DeliveryTime;
                        r.FrozenItem = order.IsFrozen != null ? ((bool)order.IsFrozen ? "Yes" : "No") : "";

                        list.Add(r);
                    }
                }
            }

            // Report
            if (list.Count() > 0)
            {
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "OrderInvoice.rpt"));
                rd.SetDataSource(list);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "OrderInvoice.pdf");
                }
                catch (Exception exp)
                {
                    throw exp;
                }
                finally
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return null;
        }

        public JsonResult GetPreviousOrderItems(string userId) // Not using now. If needed later then fix the sql query with price and discount column name
        {
            var recordList = new List<SoldItemViewModel>();
            var imageSrcPrefix = Utils.GetProductImageSrcPrefix() + "/Grid/";
            string sql = String.Format("select distinct top 200 oi.ProductId, p.Title, p.Price, p.Discount from Orders o, OrderItems oi, Products p where o.Id = oi.OrderId and oi.ProductId = p.Id and o.UserId = '{0}'", userId);

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (var context = new Data.Models.ApplicationEntities())
            {
                recordList = context.Database.SqlQuery<SoldItemViewModel>(sql).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (SoldItemViewModel p in recordList)
                    {
                        // Price & Discount
                        string priceText = String.Empty;
                        string oldPriceText = String.Empty;
                        decimal newPrice = 0;
                        Utils.GetPrice(p.Price, p.Discount, out newPrice, out priceText, out oldPriceText);
                        p.Price = newPrice;
                        p.PriceText = priceText;

                        // Get user images
                        List<ProductImageViewModel> imageVMList = new List<ProductImageViewModel>();
                        var imageList = this.productImageService.GetProductImages(p.ProductId);
                        foreach (ProductImage image in imageList)
                        {
                            p.PrimaryImageName = imageSrcPrefix + image.ImageName;
                            break;
                        }

                        if (String.IsNullOrEmpty(p.PrimaryImageName))
                        {
                            p.PrimaryImageName = imageSrcPrefix + "no-image.jpg";
                        }
                    }
                }
            }

            return Json(recordList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrder(Order order)
        {
            bool isSuccess = false;
            string sql = String.Empty;
            string actionBy = (User.Identity != null) ? User.Identity.Name : String.Empty;

            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    if (order.ReceiveAmount == null)
                    {
                        order.ReceiveAmount = 0;
                    }
                    order.ChangeAmount = order.ChangeAmount == null ? 0 : order.ChangeAmount;
                    // First delete the current order and then create a new order
                    Order orderExist = this.orderService.GetOrder(order.Id);
                    if (orderExist != null)
                    {
                        // Restore inventory stock
                        foreach (var item in orderExist.OrderItems)
                        {
                            this.productService.AddStockQty(item.ProductId, item.Quantity);
                        }

                        // Delete order                            
                        this.orderService.DeleteOrder(orderExist);
                        //order.Id = Guid.NewGuid().ToString();
                    }

                    // Assign order id to all order items
                    foreach (var item in order.OrderItems)
                    {
                        item.OrderId = order.Id;
                        //item.Id = Guid.NewGuid().ToString();
                    }

                    // Creating order (Save data to Orders and OrderItems table)
                    Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
                    using (var context = new Data.Models.ApplicationEntities())
                    {
                        if (order.OrderMode == "Online")
                        {
                            sql = String.Format(@"Insert Into Orders (Id,UserId,OrderCode,Barcode,PayAmount,DueAmount,Discount,Vat,ShippingAmount,OrderMode,OrderStatus,PaymentType,ActionDate,ActionBy,DeliveryDate,DeliveryTime,TotalWeight,IsFrozen) 
                                                  Values ('{0}','{1}',{2},'{3}',{4},{5},{6},{7},{8},'{9}','{10}','{11}','{12}','{13}','{14}','{15}',{16},{17})",
                                                order.Id, order.UserId, order.OrderCode, order.Barcode, order.PayAmount, order.DueAmount, order.Discount, order.Vat, order.ShippingAmount, order.OrderMode, order.OrderStatus, order.PaymentType, order.ActionDate, actionBy, order.DeliveryDate, order.DeliveryTime, order.TotalWeight, (bool)order.IsFrozen ? 1 : 0);
                        }
                        else
                        {
                            if (order.BranchId == null)
                            {
                                sql = String.Format(@"Insert Into Orders (Id,UserId,OrderCode,Barcode,PayAmount,DueAmount,Discount,Vat,ShippingAmount,ReceiveAmount,ChangeAmount,OrderMode,OrderStatus,PaymentStatus,PaymentType,ActionDate,ActionBy,DeliveryDate,DeliveryTime,TotalWeight,IsFrozen) 
                                                  Values ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},'{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}',{19},{20})", order.Id, order.UserId, order.OrderCode, order.Barcode, order.PayAmount, order.DueAmount, order.Discount, order.Vat, order.ShippingAmount, order.ReceiveAmount, order.ChangeAmount, order.OrderMode, order.OrderStatus, order.PaymentStatus, order.PaymentType, order.ActionDate, actionBy, order.DeliveryDate, order.DeliveryTime, order.TotalWeight, (bool)order.IsFrozen ? 1 : 0);
                            }
                            else
                            {
                                sql = String.Format(@"Insert Into Orders (Id,UserId,BranchId,OrderCode,Barcode,PayAmount,DueAmount,Discount,Vat,ShippingAmount,ReceiveAmount,ChangeAmount,OrderMode,OrderStatus,PaymentStatus,PaymentType,ActionDate,ActionBy,DeliveryDate,DeliveryTime,TotalWeight,CustomerId, OrderType) 
                                                  Values ('{0}','{1}',{2},'{3}','{4}',{5},{6},{7},{8},{9},{10},{11},'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',{20},{21},'{22}')", order.Id, order.UserId, order.BranchId, order.OrderCode, order.Barcode, order.PayAmount, order.DueAmount, order.Discount, order.Vat, order.ShippingAmount, order.ReceiveAmount, order.ChangeAmount, order.OrderMode, order.OrderStatus, order.PaymentStatus, order.PaymentType, order.ActionDate, actionBy, order.DeliveryDate, order.DeliveryTime, order.TotalWeight, order.CustomerId, order.OrderType);
                            }
                        }

                        int result = context.Database.ExecuteSqlCommand(sql);

                        if (result > 0)
                        {
                            foreach (var item in order.OrderItems)
                            {
                                string title = item.Title;

                                sql = String.Format(@"Insert Into OrderItems (OrderId,ProductId,Quantity,Discount,Price,TotalPrice,ActionDate,Title,CostPrice, Weight, Unit)
                                Values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}','{10}')", item.OrderId, item.ProductId, item.Quantity, item.Discount, item.Price, item.TotalPrice, item.ActionDate, title, item.CostPrice, item.Weight, item.Unit);
                                result = context.Database.ExecuteSqlCommand(sql);
                            }
                        }
                    }

                    // Update inventory stock
                    foreach (var item in order.OrderItems)
                    {
                        this.productService.MinusStockQty(item.ProductId, item.Quantity);
                    }

                    // Write action log
                    AppCommon.WriteActionLog(actionLogService, "Order", "Update " + order.OrderMode, "Order Amount: " + order.PayAmount.ToString(), "Update", User.Identity.Name);

                    tran.Complete();
                    tran.Dispose();
                    isSuccess = true;
                }
                catch (Exception exp)
                {
                    tran.Dispose();
                    isSuccess = false;
                }
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteOrder(string orderIds)
        {
            string sql = String.Empty;
            string actionBy = (User.Identity != null) ? User.Identity.Name : String.Empty;
            string message = String.Empty;

            string[] ids = orderIds.Split(',');
            string deletedOrderCodes = String.Empty;

            if (!User.IsInRole(ERoleName.admin.ToString()))
            {
                message = "Only administrator are allowed to delete orders!";

                return Json(new
                {
                    message = message,
                    deletedOrderCodes = deletedOrderCodes

                }, JsonRequestBehavior.AllowGet);
            }

            foreach (string id in ids)
            {
                if (String.IsNullOrEmpty(id)) continue;

                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        // First delete the current order and then create a new order
                        Order orderExist = this.orderService.GetOrder(id);
                        if (orderExist != null)
                        {
                            // Delete order                            
                            this.orderService.DeleteOrder(orderExist);
                        }

                        deletedOrderCodes += orderExist.OrderCode + ",";

                        tran.Complete();
                        tran.Dispose();
                    }
                    catch (Exception exp)
                    {
                        tran.Dispose();
                    }
                }
            }

            deletedOrderCodes = !String.IsNullOrEmpty(deletedOrderCodes) ? deletedOrderCodes.TrimEnd(',') : deletedOrderCodes;
            AppCommon.WriteActionLog(actionLogService, "Order", "Delete Orders (" + deletedOrderCodes + ")", "Order Codes: " + deletedOrderCodes, "Delete", User.Identity.Name);

            return Json(new
            {
                message = message,
                deletedOrderCodes = deletedOrderCodes

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatuses()
        {
            List<StatusViewModel> recordList = new List<StatusViewModel>();

            string sql = String.Format("select Id, Name, IsActive from Status");

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (var context = new Data.Models.ApplicationEntities())
            {
                recordList = context.Database.SqlQuery<StatusViewModel>(sql).ToList();
            }
            return Json(new
            {
                IsSuccess = true,
                data = recordList
            }, JsonRequestBehavior.AllowGet);
        }
    }
    class NextOrder
    {
        public decimal Id { get; set; }
    }

    class SoldItemViewModel
    {
        public string ProductId { get; set; }
        public string Title { get; set; }
        public string PriceText { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string PrimaryImageName { get; set; }
    }
}