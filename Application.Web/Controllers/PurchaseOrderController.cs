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


namespace Application.Web.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService purchaseOrderService;
        private IUserService userService;
        private IProductService productService;
        private readonly IActionLogService actionLogService;
        private readonly IBranchService branchService;
        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService,
            IProductService productService,
            IActionLogService actionLogService,
            IUserService userService,
            IBranchService branchService)
        {
            this.purchaseOrderService = purchaseOrderService;
            this.productService = productService;
            this.actionLogService = actionLogService;
            this.userService = userService;
            this.branchService = branchService;
        }

        // GET: WholeSale
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PurchaseOrderList()
        {
            return View();
        }

        public ActionResult PurchaseOrderDetails()
        {
            return View();
        }
        public ActionResult OrderConfirm()
        {
            return View();
        }
        public ActionResult SalesReturn()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PlaceOrder(Application.Model.Models.PurchaseOrder order)
        {
            string orderId = Guid.NewGuid().ToString();
            long orderCode = DateTime.Now.Ticks;

            string branchName = Utils.GetSetting("CompanyName");
            Branch branch = branchService.GetBranchByName(branchName);
            order.BranchId = branch.Id;

            bool isSuccess = false;
            try
            {
                // nullify the user object
                order.User = null;

                order.Id = orderId;
                order.OrderCode = orderCode;
                order.UserId = Utils.GetLoggedInUserId();
                order.StatusId = 1;
                order.PaymentStatusId = 3;
                order.DueAmount = order.PayAmount; // Always 0 in online version
                //order.Discount = 0; // Always 0 in online version because discount is already reduced from price
                order.ActionDate = DateTime.Now; //DateTime.UtcNow;
                order.ActionBy = Utils.GetLoggedInUserName();

                foreach (Model.Models.PurchaseOrderItem item in order.PurchaseOrderItems)
                {
                    item.PurchaseOrderId = orderId;
                    //item.Id = Guid.NewGuid().ToString();
                    item.ActionDate = DateTime.Now;
                    item.CostPrice = item.Price;
                    item.ReceivedQuantity = 0;
                    //item.CostPrice = purchaseOrderService.price(item.ProductId);
                }

                purchaseOrderService.CreateOrder(order);
                isSuccess = true;

                //// Update inventory stock
                //foreach (Model.Models.OrderItem item in order.OrderItems)
                //{
                //    productService.MinusStockQty(item.ProductId, item.Quantity);
                //}

                //// Update sold count
                //foreach (Model.Models.OrderItem item in order.OrderItems)
                //{
                //    productService.UpdateSoldCount(item.ProductId);
                //}

                // Generate order barcode
                AppCommon.GenerateOrderBarcode(order.OrderCode.ToString());

            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new
            {
                isSuccess = isSuccess,
                orderId = orderId,
                orderCode = orderCode
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrderDetails(string orderId)
        {
            OrderViewModel orderVM = new OrderViewModel();

            Model.Models.PurchaseOrder order = purchaseOrderService.GetOrder(orderId);
            if (order != null)
            {
                orderVM.Id = order.Id;
                orderVM.UserId = order.UserId;
                orderVM.BranchId = order.BranchId;
                orderVM.OrderCode = order.OrderCode;
                orderVM.OrderStatus = order.OrderStatus;
                orderVM.OrderMode = order.OrderMode;
                orderVM.PayAmount = order.PayAmount;
                orderVM.Discount = order.Discount;
                orderVM.Vat = order.Vat;
                orderVM.ShippingAmount = order.ShippingAmount;
                orderVM.DueAmount = order.DueAmount;
                orderVM.ReceiveAmount = order.ReceiveAmount;
                orderVM.ChangeAmount = order.ChangeAmount;
                orderVM.TotalWeight = order.TotalWeight;
                orderVM.DeliveryDate = order.DeliveryDate;
                orderVM.DeliveryTime = order.DeliveryTime;
                orderVM.IsFrozen = order.IsFrozen == null ? false : (bool)order.IsFrozen;
                orderVM.ActionDate = order.ActionDate;
                orderVM.OrderType = order.OrderType;
                orderVM.StatusId = (int)order.StatusId;
                orderVM.PaymentStatus = order.PaymentStatus;
                orderVM.PaymentStatusId = order.PaymentStatusId;
                orderVM.OrderItems = new List<ViewModel.OrderItemViewModel>();
                foreach (Model.Models.PurchaseOrderItem oi in order.PurchaseOrderItems)
                {
                    string title = oi.ProductId == Guid.Empty.ToString() ? oi.Title : oi.Product.Title;

                    OrderItemViewModel o = new ViewModel.OrderItemViewModel
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = title,
                        Price = oi.Price,
                        Discount = oi.Discount,
                        Quantity = oi.Quantity,
                        CostPrice = oi.CostPrice == null ? oi.Price : (decimal)oi.CostPrice,
                        ImageUrl = string.IsNullOrEmpty(oi.ImageUrl) ? "/Images/no-image.png" : oi.ImageUrl,
                        ActionDate = oi.ActionDate,
                        ReceivedQuantity = oi.ReceivedQuantity
                    };
                    orderVM.OrderItems.Add(o);
                }
            }

            return Json(orderVM, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintOrder(string orderId)
        {
            List<ReportOrderViewModel> list = new List<ReportOrderViewModel>();

            string sVat = Utils.GetSetting(ESetting.Vat.ToString());
            int vatPerc = string.IsNullOrEmpty(sVat) ? 0 : int.Parse(sVat);

            PurchaseOrder order = purchaseOrderService.GetOrder(orderId);
            if (order != null)
            {
                User user = userService.GetUserById(order.UserId);

                if (user != null)
                {
                    decimal vatAmount = order.Vat != null ? (decimal)order.Vat : 0;
                    decimal shippingAmount = order.ShippingAmount != null ? (decimal)order.ShippingAmount : 0;
                    decimal subTotal = (decimal)order.PayAmount + (decimal)order.Discount - vatAmount - shippingAmount;
                    decimal discount = (decimal)order.Discount;
                    decimal grandTotal = (decimal)order.PayAmount;

                    foreach (PurchaseOrderItem item in order.PurchaseOrderItems)
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
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "PurchaseOrder.rpt"));
                rd.SetDataSource(list);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "PurchaseOrder.pdf");
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
        public JsonResult CompleteOrder(string orderId)
        {
            bool isSuccess = purchaseOrderService.CompleteOrder(orderId);

            return Json(new
            {
                isSuccess = isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderList(int? branchId, string fromDate, string toDate, string orderStatus, string orderMode)
        {
            DateTime dtFrom;
            DateTime dtTo;

            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
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
                EOrderMode orderModeEnum = string.IsNullOrEmpty(orderMode) ? EOrderMode.All : (EOrderMode)Enum.Parse(typeof(EOrderMode), orderMode);

                //var orderList = this.orderService.GetOrders(branchId, dtFrom, dtTo, orderStatusEnum, orderModeEnum);
                IEnumerable<Model.Models.PurchaseOrder> orderList = purchaseOrderService.GetOrders(branchId, dtFrom, dtTo, orderStatus, orderModeEnum);


                foreach (Model.Models.PurchaseOrder item in orderList)
                {
                    OrderViewModel order = new OrderViewModel
                    {
                        Id = item.Id,
                        BranchId = item.BranchId,
                        OrderCode = item.OrderCode,
                        Discount = item.Discount,
                        DueAmount = item.DueAmount,
                        PayAmount = item.PayAmount,
                        OrderMode = item.OrderMode,
                        OrderStatus = item.OrderStatus,
                        PaymentStatus = item.PaymentStatus,
                        PaymentType = item.PaymentType,
                        Vat = item.Vat,
                        ActionDate = item.ActionDate,
                        ActionDateString = Utils.GetFormattedDate(item.ActionDate)
                    };

                    orderVMList.Add(order);
                }
            }

            return Json(orderVMList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateOrder(PurchaseOrder order)
        {
            bool isSuccess = false;
            string sql = string.Empty;
            string actionBy = (User.Identity != null) ? User.Identity.Name : string.Empty;

            using (TransactionScope tran = new TransactionScope())
            {
                try
                {
                    // First delete the current order and then create a new order
                    PurchaseOrder orderExist = purchaseOrderService.GetOrder(order.Id);
                    if (orderExist != null)
                    {
                        // Restore inventory stock
                        //foreach (PurchaseOrderItem item in orderExist.PurchaseOrderItems)
                        //{
                        //    productService.AddStockQty(item.ProductId, item.Quantity);
                        //}

                        // Delete order                            
                        purchaseOrderService.DeleteOrder(orderExist);
                        order.Id = Guid.NewGuid().ToString();
                    }

                    // Assign order id to all order items
                    foreach (PurchaseOrderItem item in order.PurchaseOrderItems)
                    {
                        item.PurchaseOrderId = order.Id;
                        //item.Id = Guid.NewGuid().ToString();
                    }
                    //order.ChangeAmount = 0;
                    //order.ReceiveAmount = 0;
                    // Creating order (Save data to Orders and OrderItems table)
                    Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
                    using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
                    {
                        if (order.OrderMode == "Online")
                        {
                            sql = string.Format(@"Insert Into PurchaseOrders (Id,UserId,OrderCode,Barcode,PayAmount,DueAmount,Discount,Vat,ShippingAmount,OrderMode,OrderStatus,PaymentType,ActionDate,ActionBy,DeliveryDate,DeliveryTime,TotalWeight,IsFrozen) 
                                                  Values ('{0}','{1}',{2},'{3}',{4},{5},{6},{7},{8},'{9}','{10}','{11}','{12}','{13}','{14}','{15}',{16},{17})",
                                                order.Id, order.UserId, order.OrderCode, order.Barcode, order.PayAmount, order.DueAmount, order.Discount, order.Vat, order.ShippingAmount, order.OrderMode, order.OrderStatus, order.PaymentType, order.ActionDate, actionBy, order.DeliveryDate, order.DeliveryTime, order.TotalWeight, (bool)order.IsFrozen ? 1 : 0);
                        }
                        else
                        {
                            if (order.BranchId == null)
                            {
                                sql = string.Format(@"Insert Into PurchaseOrders (Id,UserId,OrderCode,Barcode,PayAmount,DueAmount,Discount,Vat,ShippingAmount,OrderMode,OrderStatus,PaymentStatus,PaymentType,ActionDate,ActionBy,DeliveryDate,DeliveryTime,TotalWeight,IsFrozen) 
                                                  Values ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},'{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')", order.Id, order.UserId, order.OrderCode, order.Barcode, order.PayAmount, order.DueAmount, order.Discount, order.Vat, order.ShippingAmount, order.OrderMode, order.OrderStatus, order.PaymentStatus, order.PaymentType, order.ActionDate, actionBy, order.DeliveryDate, order.DeliveryTime, order.TotalWeight, (bool)order.IsFrozen ? 1 : 0);
                            }
                            else
                            {
                                sql = string.Format(@"Insert Into PurchaseOrders (Id,UserId,BranchId,OrderCode,Barcode,PayAmount,DueAmount,Discount,Vat,ShippingAmount,OrderMode,OrderStatus,PaymentStatus,PaymentType,ActionDate,ActionBy,DeliveryDate,DeliveryTime,TotalWeight,IsFrozen) 
                                                  Values ('{0}','{1}',{2},'{3}','{4}',{5},{6},{7},{8},{9},{10},{11},'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')", order.Id, order.UserId, order.BranchId, order.OrderCode, order.Barcode, order.PayAmount, order.DueAmount, order.Discount, order.Vat, order.ShippingAmount, order.OrderMode, order.OrderStatus, order.PaymentStatus, order.PaymentType, order.ActionDate, actionBy, order.DeliveryDate, order.DeliveryTime, order.TotalWeight, (bool)order.IsFrozen ? 1 : 0);
                            }
                        }

                        int result = context.Database.ExecuteSqlCommand(sql);

                        if (result > 0)
                        {
                            foreach (PurchaseOrderItem item in order.PurchaseOrderItems)
                            {
                                string title = (item.ProductId == "00000000-0000-0000-0000-000000000000") ? item.Title : string.Empty;

                                sql = string.Format(@"Insert Into PurchaseOrderItems (Id,OrderId,ProductId,Quantity,Discount,Price,TotalPrice,ImageUrl,ActionDate,Title,CostPrice, ReceivedQuantity)
                                Values('{0}','{1}','{2}',{3},{4},{5},{6},'{7}','{8}','{9}', {10}, {11})", item.Id, item.PurchaseOrderId, item.ProductId, item.Quantity, item.Discount, item.Price, item.TotalPrice, item.ImageUrl, item.ActionDate, title, item.CostPrice, item.ReceivedQuantity);
                                result = context.Database.ExecuteSqlCommand(sql);
                            }
                        }
                    }

                    // Update inventory stock
                    foreach (PurchaseOrderItem item in order.PurchaseOrderItems)
                    {
                        productService.AddStockQty(item.ProductId, item.ReceivedQuantity, true);
                    }

                    // Write action log
                    AppCommon.WriteActionLog(actionLogService, "Order", "Update " + order.OrderMode, "Order Amount: " + order.PayAmount.ToString(), "Update", User.Identity.Name);

                    tran.Complete();
                    tran.Dispose();
                    isSuccess = true;
                }
                catch (Exception)
                {
                    tran.Dispose();
                    isSuccess = false;
                }
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
    }
}