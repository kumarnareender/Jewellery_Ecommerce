using Application.Common;
using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{

    public interface IPurchaseOrderService
    {
        void CreateOrder(PurchaseOrder order);
        void UpdateOrder(PurchaseOrder order);
        bool UpdateOrderStatus(OrderStatusViewModel order);
        bool UpdateOrderPaymentStatus(OrderStatusViewModel order);
        void DeleteOrder(PurchaseOrder order);
        bool CompleteOrder(string orderId);
        void OrderPaymentDone(string orderId);
        IEnumerable<PurchaseOrder> GetOrders(string userId);
        IEnumerable<PurchaseOrder> GetOrders(EOrderStatus orderStatus);
        //IEnumerable<Order> GetOrders(int? branchId, DateTime fromDate, DateTime toDate, EOrderStatus orderStatus, EOrderMode orderMode);
        IEnumerable<PurchaseOrder> GetOrders(int? branchId, DateTime fromDate, DateTime toDate, string orderStatus, EOrderMode orderMode);

        IEnumerable<PurchaseOrder> GetOnlineOrders(int count);
        PurchaseOrder GetOrder(string id);
        void Commit();
    }

    public class PuchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository orderRepository;
        private readonly IUnitOfWork unitOfWork;

        public PuchaseOrderService(IPurchaseOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            this.orderRepository = orderRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IOrderService Members

        public void CreateOrder(PurchaseOrder order)
        {
            orderRepository.Add(order);
            Commit();
        }

        public void UpdateOrder(PurchaseOrder order)
        {
            orderRepository.Update(order);
            Commit();
        }

        public void OrderPaymentDone(string orderId)
        {
            PurchaseOrder order = orderRepository.Get(r => r.Id == orderId);
            if (order != null)
            {
                order.PaymentStatus = EPaymentStatus.Done.ToString();
                orderRepository.Update(order);
                Commit();
            }
        }

        public void DeleteOrder(PurchaseOrder order)
        {
            string sqlDeleteOrderItems = string.Format("Delete From PurchaseOrderItems Where PurchaseOrderId = '{0}'", order.Id);
            string sqlDeleteOrder = string.Format("Delete From PurchaseOrders Where Id = '{0}'", order.Id);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                int result = context.Database.ExecuteSqlCommand(sqlDeleteOrderItems + ";" + sqlDeleteOrder + ";");
            }
        }

        public bool CompleteOrder(string orderId)
        {
            PurchaseOrder order = orderRepository.Get(r => r.Id == orderId);
            if (order != null)
            {
                order.OrderStatus = EOrderStatus.Completed.ToString();
                order.StatusId = 7;
                orderRepository.Update(order);
                Commit();
                return true;
            }

            return false;
        }

        public bool UpdateOrderStatus(OrderStatusViewModel orderStatus)
        {
            PurchaseOrder order = orderRepository.Get(r => r.Id == orderStatus.OrderId);
            if (order != null)
            {
                order.OrderStatus = orderStatus.NewStatus;
                order.StatusId = orderStatus.NewStatusId;
                orderRepository.Update(order);
                Commit();
                return true;
            }

            return false;
        }
        public bool UpdateOrderPaymentStatus(OrderStatusViewModel orderStatus)
        {
            PurchaseOrder order = orderRepository.Get(r => r.Id == orderStatus.OrderId);
            if (order != null)
            {
                order.PaymentStatus = orderStatus.NewStatus;
                order.PaymentStatusId = orderStatus.NewStatusId;
                order.DueAmount = orderStatus.DueAmount;
                orderRepository.Update(order);
                Commit();
                return true;
            }

            return false;
        }
        public IEnumerable<PurchaseOrder> GetOrders(string userId)
        {
            List<PurchaseOrder> orders = orderRepository.GetMany(r => r.UserId == userId).OrderByDescending(r => r.ActionDate).ToList();
            return orders;
        }

        public IEnumerable<PurchaseOrder> GetOrders(EOrderStatus orderStatus)
        {
            IEnumerable<PurchaseOrder> orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToString().ToLower()).OrderByDescending(r => r.ActionDate).ToList().Take(500);
            return orders;
        }

        public IEnumerable<PurchaseOrder> GetOrders(int? branchId, DateTime fromDate, DateTime toDate, string orderStatus, EOrderMode orderMode)
        {
            List<PurchaseOrder> orders = new List<PurchaseOrder>();

            if (branchId != null)
            {
                if (orderMode == EOrderMode.All)
                {
                    orders = orderRepository.GetMany(r => r.BranchId == (int)branchId && r.OrderStatus.ToLower() == orderStatus.ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
                }
                else
                {
                    orders = orderRepository.GetMany(r => r.BranchId == (int)branchId && r.OrderStatus.ToLower() == orderStatus.ToLower() && r.OrderMode.ToLower() == orderMode.ToString().ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
                }
            }
            else
            {
                if (orderMode == EOrderMode.All)
                {
                    orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
                }
                else
                {
                    orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToLower() && r.OrderMode.ToLower() == orderMode.ToString().ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
                }
            }

            return orders;
        }

        public IEnumerable<PurchaseOrder> GetOnlineOrders(int count)
        {
            List<PurchaseOrder> orders = new List<PurchaseOrder>();
            orders = orderRepository.GetMany(r => r.OrderMode.ToLower() == EOrderMode.Online.ToString().ToLower() || r.OrderMode.ToLower() == EOrderMode.PhoneOrder.ToString().ToLower()).OrderByDescending(r => r.ActionDate).Take(count).ToList();
            return orders;
        }

        public PurchaseOrder GetOrder(string id)
        {
            PurchaseOrder order = orderRepository.Get(r => r.Id == id);

            return order;
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
