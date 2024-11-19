using Application.Common;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class OrderPaymentStatusController : Controller
    {
        private readonly IOrderPaymentStatusService orderPaymentStatusService;
        private readonly IOrderService orderService;
        private readonly IPurchaseOrderService purchaseOrderService;
        public OrderPaymentStatusController(IOrderPaymentStatusService orderPaymentStatusService, IOrderService orderService, IPurchaseOrderService purchaseOrderService)
        {
            this.orderPaymentStatusService = orderPaymentStatusService;
            this.orderService = orderService;
            this.purchaseOrderService = purchaseOrderService;
        }

        public ActionResult OrderStatus()
        {
            return View();
        }
        public ActionResult paymentsOrderDetails()
        {
            return View();
        }
        public JsonResult GetOrderStatusByOrder(string orderId)
        {
            List<OrderPayment> statusList = orderPaymentStatusService.GetOrderPaymentStatusByOrder(orderId);
            return Json(new Result { IsSuccess = true, Data = statusList }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateOrderStatusHistory(OrderStatusViewModel status)
        {
            bool isSuccess = true;
            try
            {
                if (status.Amount == status.DueAmount)
                {
                    status.NewStatusId = 1;
                    status.NewStatus = "FullPayment";
                }
                else if (status.Amount < status.DueAmount)
                {
                    status.NewStatusId = 2;
                    status.NewStatus = "PartPayment";
                }



                OrderPayment orderStatus = new OrderPayment()
                {
                    OrderId = status.OrderId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Utils.GetLoggedInUserName(),
                    LastModifiedBy = Utils.GetLoggedInUserName(),
                    Description = status.Description,
                    NewStatusId = status.NewStatusId,
                    OldStatusId = status.OldStatusId,
                    LastModifiedDate = DateTime.Now,
                    AmountDeposited = status.Amount,
                    IsPurchaseOrder = status.IsPurchaseOrder,
                    PaymentType = status.PaymentType
                };

                orderStatus.DueAmount = status.DueAmount - status.Amount;
                orderStatus.TotalDeposited = status.TotalDeposited+ status.Amount;
                status.DueAmount = orderStatus.DueAmount;

                if (status.IsPurchaseOrder)
                {
                    orderStatus.Payment = "Paid";
                    purchaseOrderService.UpdateOrderPaymentStatus(status);
                }
                else
                {
                    orderStatus.Payment = "Received";
                    orderService.UpdateOrderPaymentStatus(status);
                }

                orderPaymentStatusService.CreateOrderPaymentStatus(orderStatus);
            }
            catch (Exception)
            {

                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateOrderStatus(OrderPayment OrderStatus)
        {
            bool isSuccess = true;
            try
            {
                orderPaymentStatusService.UpdateOrderPaymentStatus(OrderStatus);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteOrderStatus(OrderPayment OrderStatus)
        {
            bool isSuccess = true;
            try
            {
                orderPaymentStatusService.DeleteOrderPaymentStatus(OrderStatus);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPaymentStatuses()
        {
            List<StatusViewModel> recordList = new List<StatusViewModel>();

            string sql = string.Format("select Id, Name, IsActive from PaymentStatus");

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
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
}