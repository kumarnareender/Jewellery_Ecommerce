using Application.Common;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class OrderStatusController : Controller
    {
        private IOrderStatusService orderStatusService;
        private readonly IOrderService orderService;
        private readonly IPurchaseOrderService purchaseOrderService;
        public OrderStatusController(IOrderStatusService orderStatusService, IOrderService orderService, IPurchaseOrderService purchaseOrderService)
        {
            this.orderStatusService = orderStatusService;
            this.orderService = orderService;
            this.purchaseOrderService = purchaseOrderService;
        }

        public ActionResult OrderStatus()
        {
            return View();
        }

        public JsonResult GetOrderStatusByOrder(string orderId)
        {
            List<OrderStatus> statusList = orderStatusService.GetOrderStatusByOrder(orderId);
            List<OrderStatus> list = new List<OrderStatus>();
            return Json(new Result { IsSuccess = true, Data = statusList }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateOrderStatusHistory(OrderStatusViewModel status)
        {
            bool isSuccess = true;
            try
            {
                //var order = orderService.GetOrder(status.OrderId);
                //int currentStatusid = order.StatusId;
                OrderStatus orderStatus = new OrderStatus()
                {
                    OrderId = status.OrderId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Utils.GetLoggedInUserName(),
                    LastModifiedBy = Utils.GetLoggedInUserName(),
                    Description = status.Description,
                    NewStatusId = status.NewStatusId,
                    OldStatusId = status.OldStatusId,
                    LastModifiedDate = DateTime.Now,
                };
                if (status.IsPurchaseOrder)
                {
                    purchaseOrderService.UpdateOrderStatus(status);
                }
                else
                {
                    orderService.UpdateOrderStatus(status);
                }

                orderStatusService.CreateOrderStatus(orderStatus);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateOrderStatus(OrderStatus OrderStatus)
        {
            bool isSuccess = true;
            try
            {
                orderStatusService.UpdateOrderStatus(OrderStatus);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteOrderStatus(OrderStatus OrderStatus)
        {
            bool isSuccess = true;
            try
            {
                orderStatusService.DeleteOrderStatus(OrderStatus);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}