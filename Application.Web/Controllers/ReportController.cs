using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web.ReportViewModel;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Application.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private IUserService userService;
        private IOrderService orderService;
        private IActionLogService actionLogService;
        private readonly IProductImageService productImageService;

        public ReportController(IUserService userService, IOrderService orderService, IProductImageService productImageService, IActionLogService actionLogService)
        {
            this.userService = userService;
            this.orderService = orderService;
            this.actionLogService = actionLogService;
            this.productImageService = productImageService;
        }

        public ActionResult SaleSummary()
        {
            return View();
        }

        public ActionResult DailySales()
        {
            return View();
        }

        public ActionResult PaymentReport()
        {
            return View();
        }

        public ActionResult MonthlySalesChart()
        {
            return View();
        }

        public ActionResult DailySalesChart()
        {
            return View();
        }

        public ActionResult ActivityLog()
        {
            return View();
        }

        public JsonResult GetActionLogHistory(string fromDate, string toDate)
        {
            List<ActionLog> actionLogList = GetActionLogHistoryData(fromDate, toDate);
            return Json(actionLogList, JsonRequestBehavior.AllowGet);
        }

        private List<ActionLog> GetActionLogHistoryData(string dateFrom, string dateTo)
        {
            DateTime.TryParseExact(dateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtFrom);
            DateTime.TryParseExact(dateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtTo);

            List<ActionLog> actionLogList = actionLogService.GetActionLogs(dtFrom, dtTo.AddHours(23).AddMinutes(59).AddSeconds(59)).ToList();
            return actionLogList;
        }

        public JsonResult GetPaymentReport(int? branchId, string fromDate, string toDate, string orderMode, string payment)
        {
            List<OrderViewModel> orderList = GetPaymentReportData(branchId, fromDate, toDate, orderMode, payment);
            return Json(orderList, JsonRequestBehavior.AllowGet);
        }

        private List<OrderViewModel> GetPaymentReportData(int? branchId, string fromDate, string toDate, string orderMode, string orderStatus)
        {
            toDate = toDate + " 23:59:59";
            string queryPurchaseOrders =
            string.Format(@"Select o.Id, OrderCode, OrderMode, OrderStatus, 
                                po.PaymentType, PayAmount as TotalAmount,
                                po.AmountDeposited, po.TotalDeposited, po.DueAmount,
                                convert(varchar, po.CreatedDate, 100) as ActionDateString,
                                po.Payment, po.CreatedDate 
                                From PurchaseOrders o inner join orderpayments po on o.id = po.orderId
                                Where createdDate between '{0}' and '{1}'", fromDate, toDate);
            string queryOrders =
            string.Format(@"Select o.Id, OrderCode, OrderMode, OrderStatus, 
                                po.PaymentType, PayAmount as TotalAmount,
                                po.AmountDeposited, po.TotalDeposited, po.DueAmount,
                                convert(varchar, po.CreatedDate, 100) as ActionDateString,
                                po.Payment, po.CreatedDate 
                                From Orders o inner join orderpayments po on o.id = po.orderId
                                Where createdDate between '{0}' and '{1}'", fromDate, toDate);

            string orderByClause = " Order by createdDate desc";
            string andClause = string.Empty;

            if (branchId != null)
            {
                andClause += string.Format(" And BranchId = {0}", branchId);
            }

            if (!string.IsNullOrEmpty(orderMode) && orderMode.ToLower() != "all")
            {
                andClause += string.Format(" And OrderMode = '{0}'", orderMode);
            }

            if (!string.IsNullOrEmpty(orderStatus) && orderStatus.ToLower() != "all")
            {
                andClause += string.Format(" And Payment = '{0}'", orderStatus);
            }

            if (!string.IsNullOrEmpty(andClause))
            {
                queryPurchaseOrders += andClause;
                queryOrders += andClause;
            }


            //queryPurchaseOrders += orderByClause;
            //queryOrders += orderByClause;

            string query = $"{queryPurchaseOrders} UNION {queryOrders} ";
            query += orderByClause;
            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<OrderViewModel> orderList = context.Database.SqlQuery<OrderViewModel>(query).ToList();
                if (orderList != null && orderList.Count > 0)
                {
                    foreach (OrderViewModel item in orderList)
                    {
                        decimal totalCostPrice = 0;

                        // Get total cost price of an order
                        List<OrderItem> orderItemList = GetOrderItems(item.Id);
                        foreach (OrderItem orderItem in orderItemList)
                        {
                            totalCostPrice += orderItem.CostPrice == null ? 0 : ((decimal)orderItem.CostPrice * orderItem.Quantity);
                        }

                        // Calculate profit
                        //decimal profit = (decimal)item.TotalAmount - totalCostPrice - item.Vat != null ? (decimal)item.Vat : 0 - (decimal)item.ShippingAmount;

                        item.TotalCostPrice = totalCostPrice;
                        //item.TotalProfit = profit;
                    }

                    return orderList;
                }
            }

            return new List<OrderViewModel>();
        }


        public JsonResult GetDailySales(int? branchId, string fromDate, string toDate, string orderMode, string orderStatus)
        {
            List<OrderViewModel> orderList = GetDailySalesData(branchId, fromDate, toDate, orderMode, orderStatus);

            return Json(orderList, JsonRequestBehavior.AllowGet);
        }

        private List<OrderViewModel> GetDailySalesData(int? branchId, string fromDate, string toDate, string orderMode, string orderStatus)
        {
            toDate = toDate + " 23:59:59";
            string query = string.Format(@"Select Id, OrderCode, OrderMode, OrderStatus, PaymentType, (PayAmount + Discount - Vat - ShippingAmount) as ItemTotal, DueAmount, Discount, Vat, ShippingAmount, PayAmount, ActionDate, convert(varchar, ActionDate, 100) as ActionDateString
                            From Orders 
                            Where ActionDate between '{0}' and '{1}'", fromDate, toDate);

            string orderByClause = " Order by ActionDate desc";
            string andClause = string.Empty;

            if (branchId != null)
            {
                andClause += string.Format(" And BranchId = {0}", branchId);
            }

            if (!string.IsNullOrEmpty(orderMode) && orderMode.ToLower() != "all")
            {
                andClause += string.Format(" And OrderMode = '{0}'", orderMode);
            }

            if (!string.IsNullOrEmpty(orderStatus) && orderStatus.ToLower() != "all")
            {
                andClause += string.Format(" And orderStatus = '{0}'", orderStatus);
            }

            if (!string.IsNullOrEmpty(andClause))
            {
                query += andClause;
            }

            query += orderByClause;

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<OrderViewModel> orderList = context.Database.SqlQuery<OrderViewModel>(query).ToList();
                if (orderList != null && orderList.Count > 0)
                {
                    foreach (OrderViewModel item in orderList)
                    {
                        decimal totalCostPrice = 0;

                        // Get total cost price of an order
                        List<OrderItem> orderItemList = GetOrderItems(item.Id);
                        foreach (OrderItem orderItem in orderItemList)
                        {
                            totalCostPrice += orderItem.CostPrice == null ? 0 : ((decimal)orderItem.CostPrice * orderItem.Quantity);
                        }

                        // Calculate profit
                        decimal profit = (decimal)item.PayAmount - totalCostPrice - item.Vat != null ? (decimal)item.Vat : 0 - (decimal)item.ShippingAmount;

                        item.TotalCostPrice = totalCostPrice;
                        item.TotalProfit = profit;
                    }

                    return orderList;
                }
            }

            return new List<OrderViewModel>();
        }

        public JsonResult GetMonthlySellsChart(int? branchId, int year)
        {
            string startDate = year.ToString() + "-01-01";
            string endDate = year.ToString() + "-12-31";

            string storeSubQ = "select ISNULL(sum(PayAmount),0) from Orders where OrderMode = 'Store' and convert(varchar(7), ActionDate, 126) = convert(varchar(7), o.ActionDate, 126)";
            string phoneOrderSubQ = "select ISNULL(sum(PayAmount),0) from Orders where OrderMode = 'PhoneOrder' and convert(varchar(7), ActionDate, 126) = convert(varchar(7), o.ActionDate, 126)";
            string onlineSubQ = "select ISNULL(sum(PayAmount),0) from Orders where OrderMode = 'Online' and convert(varchar(7), ActionDate, 126) = convert(varchar(7), o.ActionDate, 126)";
            string totalSubQ = "select ISNULL(sum(PayAmount),0) from Orders where convert(varchar(7), ActionDate, 126) = convert(varchar(7), o.ActionDate, 126)";

            if (branchId != null)
            {
                storeSubQ += string.Format(" and BranchId = {0}", branchId);
                phoneOrderSubQ += string.Format(" and BranchId = {0}", branchId);
                onlineSubQ += string.Format(" and BranchId = {0}", branchId);
                totalSubQ += string.Format(" and BranchId = {0}", branchId);
            }

            string sqlQuery = string.Empty;

            if (branchId != null)
            {
                sqlQuery = string.Format(@" select convert(varchar(7), ActionDate, 126) as Month,
                                ({0}) as TotalStoreSell,
                                ({1}) as TotalPhoneOrderSell,
                                (0.00) as TotalOnlineSell,
                                ({2}) as TotalSell

                                from Orders o
                                where ActionDate Between '{3}' and '{4}'
                                and BranchId = {5}
                                group by convert(varchar(7), ActionDate, 126) 
                                order by convert(varchar(7), ActionDate, 126) ", storeSubQ, phoneOrderSubQ, totalSubQ, startDate, endDate, branchId);
            }
            else
            {
                sqlQuery = string.Format(@" select convert(varchar(7), ActionDate, 126) as Month,
                                ({0}) as TotalStoreSell,
                                ({1}) as TotalPhoneOrderSell,
                                ({2}) as TotalOnlineSell,
                                ({3}) as TotalSell

                                from Orders o
                                where ActionDate Between '{4}' and '{5}'                                
                                group by convert(varchar(7), ActionDate, 126) 
                                order by convert(varchar(7), ActionDate, 126) ", storeSubQ, phoneOrderSubQ, onlineSubQ, totalSubQ, startDate, endDate);
            }

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<MonthlySales> recordList = context.Database.SqlQuery<MonthlySales>(sqlQuery).ToList();
                return Json(recordList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDailySellsChart(int? branchId, int month)
        {
            DateTime dtStartDate = new DateTime(DateTime.Now.Year, month, 1);
            int lastDayOfMonth = DateTime.DaysInMonth(dtStartDate.Year, dtStartDate.Month);


            string startDate = dtStartDate.Year.ToString() + "-" + dtStartDate.Month.ToString() + "-" + dtStartDate.Day.ToString();
            string endDate = dtStartDate.Year.ToString() + "-" + dtStartDate.Month.ToString() + "-" + lastDayOfMonth;

            string sqlQuery = string.Empty;

            if (branchId != null)
            {
                sqlQuery = string.Format(@" select CONVERT(varchar(100), convert(DATE, ActionDate)) as Day,
                                                (select ISNULL(sum(PayAmount),0) from Orders where convert(DATE, ActionDate) = convert(DATE, o.ActionDate) and BranchId = {0} and OrderMode in ('Store','PhoneOrder')) as TotalSell                                
                                                from Orders o
                                                where ActionDate Between '{1}' and '{2}'
                                                and BranchId = {0}
                                                group by convert(DATE, ActionDate) 
                                                order by convert(DATE, ActionDate) ", branchId, startDate, endDate);
            }
            else
            {
                sqlQuery = string.Format(@" select CONVERT(varchar(100), convert(DATE, ActionDate)) as Day,
                                                (select ISNULL(sum(PayAmount),0) from Orders where convert(DATE, ActionDate) = convert(DATE, o.ActionDate)) as TotalSell                                
                                                from Orders o
                                                where ActionDate Between '{0}' and '{1}'
                                                group by convert(DATE, ActionDate) 
                                                order by convert(DATE, ActionDate) ", startDate, endDate);
            }

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<DailySales> recordList = context.Database.SqlQuery<DailySales>(sqlQuery).ToList();
                return Json(recordList, JsonRequestBehavior.AllowGet);
            }
        }

        private List<OrderItem> GetOrderItems(string orderId)
        {
            List<OrderItem> orderItemList = new List<OrderItem>();
            string query = string.Format(@"Select * from OrderItems where OrderId = '{0}'", orderId);

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                orderItemList = context.Database.SqlQuery<OrderItem>(query).ToList();
            }

            return orderItemList;
        }

        [AllowAnonymous]
        public ActionResult PrintOrder(string orderId)
        {
            List<ReportOrderViewModel> list = new List<ReportOrderViewModel>();

            int vatPerc = int.Parse(ConfigurationManager.AppSettings["Vat"].ToString());

            Order order = orderService.GetOrder(orderId);
            if (order != null)
            {
                User user = userService.GetUserById(order.UserId);

                if (user != null)
                {
                    decimal subTotal = 0;
                    decimal vatAmount = 0;
                    decimal shippingAmount = 0; //TODO
                    decimal grandTotal = 0;

                    foreach (OrderItem item in order.OrderItems)
                    {
                        ReportOrderViewModel r = new ReportOrderViewModel();

                        decimal itemTotal = (item.Quantity * item.Price);

                        r.CompanyName = ConfigurationManager.AppSettings["CompanyName"].ToString();
                        r.Name = user.FirstName + " " + user.LastName;
                        r.OrderId = order.Id.ToString();
                        r.Address = user.ShipAddress;
                        r.Mobile = user.Username;
                        r.City = user.ShipCity;
                        r.State = user.ShipState;
                        r.Country = user.ShipCountry;
                        r.Zipcode = user.ShipZipCode;

                        r.ProductName = item.Product.Title;
                        r.Quantity = item.Quantity;
                        r.Price = item.Price;
                        r.ItemTotal = itemTotal;

                        subTotal += itemTotal;
                        vatAmount = Math.Round((subTotal * vatPerc) / 100);
                        grandTotal = subTotal + vatAmount + shippingAmount;

                        r.OrderStatus = order.OrderStatus;
                        r.OrderDate = ((DateTime)order.ActionDate).ToString("dddd, dd MMMM yyyy hh:mm tt");
                        r.ImageName = item.ImageUrl;
                        r.SubTotal = subTotal;
                        r.VatAmount = vatAmount;
                        r.ShippingAmount = shippingAmount;
                        r.GrandTotal = grandTotal;

                        r.PaymentBy = "COD"; // TODO

                        list.Add(r);
                    }
                }

            }

            // Report
            if (list.Count() > 0)
            {
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ProductOrder.rpt"));
                rd.SetDataSource(list);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                try
                {
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

    }

    internal class SaleSummary
    {
        public int TotalOrder { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalShippingAmount { get; set; }
        public decimal TotalSellsAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalCostAmount { get; set; }
        public decimal TotalProfit { get; set; }
    }

    internal class SaleProfitLoss
    {
        public decimal TotalCostPrice { get; set; }
        public decimal TotalSellPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalShippingAmount { get; set; }
    }

    public class MonthlySales
    {
        public string Month { get; set; }
        public decimal TotalStoreSell { get; set; }
        public decimal TotalOnlineSell { get; set; }
        public decimal TotalPhoneOrderSell { get; set; }
        public decimal TotalSell { get; set; }
    }

    public class DailySales
    {
        public string Day { get; set; }
        public decimal TotalSell { get; set; }
    }

}