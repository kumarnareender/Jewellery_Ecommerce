using Application.Common;
using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Application.Service
{

    public interface IOrderService
    {
        void CreateOrder(Order order);
        void UpdateOrder(Order order);
        bool UpdateOrderStatus(OrderStatusViewModel order);
        bool UpdateOrderPaymentStatus(OrderStatusViewModel order);
        void DeleteOrder(Order order);
        bool CompleteOrder(string orderId);
        void OrderPaymentDone(string orderId);
        IEnumerable<Order> GetOrders(string userId);
        IEnumerable<Order> GetOrders(EOrderStatus orderStatus);
        //IEnumerable<Order> GetOrders(int? branchId, DateTime fromDate, DateTime toDate, EOrderStatus orderStatus, EOrderMode orderMode);
        IEnumerable<Order> GetOrders(DateTime fromDate, DateTime toDate);
        IEnumerable<Order> GetOrdersByUser(int branchId, DateTime fromDate, DateTime toDate, string userId);

        IEnumerable<Order> GetOnlineOrders(int count);
        IEnumerable<Order> GetOnlineOrdersByUsers(string userId, int branchId, int count);
        Order GetOrder(string id);
        void Commit();
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            this.orderRepository = orderRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IOrderService Members

        public void CreateOrder(Order order)
        {
            orderRepository.Add(order);
            Commit();
        }

        public void UpdateOrder(Order order)
        {
            orderRepository.Update(order);
            Commit();
        }

        public void OrderPaymentDone(string orderId)
        {
            Order order = orderRepository.Get(r => r.Id == orderId);
            if (order != null)
            {
                order.PaymentStatus = EPaymentStatus.Done.ToString();
                orderRepository.Update(order);
                Commit();
            }
        }

        public void DeleteOrder(Order order)
        {
            string sqlDeleteOrderItems = string.Format("Delete From OrderItems Where OrderId = '{0}'", order.Id);
            string sqlDeleteOrder = string.Format("Delete From Orders Where Id = '{0}'", order.Id);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                int result = context.Database.ExecuteSqlCommand(sqlDeleteOrderItems + ";" + sqlDeleteOrder + ";");
            }
        }

        public bool CompleteOrder(string orderId)
        {
            Order order = orderRepository.Get(r => r.Id == orderId);
            if (order != null)
            {
                order.OrderStatus = EOrderStatus.Completed.ToString();
                order.StatusId = 7;
                order.PaymentStatus = EPaymentStatus.Paid.ToString();
                order.PaymentStatusId = 1;
                orderRepository.Update(order);
                Commit();
                return true;
            }

            return false;
        }

        public bool UpdateOrderStatus(OrderStatusViewModel orderStatus)
        {
            Order order = orderRepository.Get(r => r.Id == orderStatus.OrderId);
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
            Order order = orderRepository.Get(r => r.Id == orderStatus.OrderId);
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

        public IEnumerable<Order> GetOrders(string userId)
        {
            List<Order> orders = orderRepository.GetMany(r => r.UserId == userId).OrderByDescending(r => r.ActionDate).ToList();
            return orders;
        }

        public IEnumerable<Order> GetOrders(EOrderStatus orderStatus)
        {
            IEnumerable<Order> orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToString().ToLower()).OrderByDescending(r => r.ActionDate).ToList().Take(500);
            return orders;
        }

        public IEnumerable<Order> GetOrders(DateTime fromDate, DateTime toDate)
        {

            List<Order> orders = new List<Order>();

            //if (branchId != null)
            //{
            orders = orderRepository.GetMany(x => x.ActionDate >= fromDate && x.ActionDate <= toDate).ToList();
            //}

            //orders = orderRepository.GetAll().ToList();
            //return orders;
            //if (branchId != null)
            //{
            //    if (orderMode == EOrderMode.All)
            //    {
            //        orders = orderRepository.GetMany(r => r.BranchId == (int)branchId && r.OrderStatus.ToLower() == orderStatus.ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //    else
            //    {
            //        orders = orderRepository.GetMany(r => r.BranchId == (int)branchId && r.OrderStatus.ToLower() == orderStatus.ToLower() && r.OrderMode.ToLower() == orderMode.ToString().ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //}
            //else
            //{
            //    if (orderMode == EOrderMode.All)
            //    {
            //        orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //    else
            //    {
            //        orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToLower() && r.OrderMode.ToLower() == orderMode.ToString().ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //}
            //orders = orders.Where(x => x.OrderMode == "RestaurantOrder").ToList();
            return orders;
        }
        public IEnumerable<Order> GetOrdersByUser(int branchId, DateTime fromDate, DateTime toDate, string userId)
        {

            List<Order> orders = new List<Order>();

            orders = orderRepository.GetMany(x => x.BranchId == branchId && x.UserId == userId && x.ActionDate >= fromDate && x.ActionDate <= toDate).ToList();

            //orders = orderRepository.GetAll().ToList();
            //return orders;
            //if (branchId != null)
            //{
            //    if (orderMode == EOrderMode.All)
            //    {
            //        orders = orderRepository.GetMany(r => r.BranchId == (int)branchId && r.OrderStatus.ToLower() == orderStatus.ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //    else
            //    {
            //        orders = orderRepository.GetMany(r => r.BranchId == (int)branchId && r.OrderStatus.ToLower() == orderStatus.ToLower() && r.OrderMode.ToLower() == orderMode.ToString().ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //}
            //else
            //{
            //    if (orderMode == EOrderMode.All)
            //    {
            //        orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //    else
            //    {
            //        orders = orderRepository.GetMany(r => r.OrderStatus.ToLower() == orderStatus.ToLower() && r.OrderMode.ToLower() == orderMode.ToString().ToLower() && r.ActionDate >= fromDate && r.ActionDate <= toDate).OrderByDescending(r => r.ActionDate).ToList();
            //    }
            //}
            //orders = orders.Where(x => x.OrderMode == "RestaurantOrder").ToList();
            return orders;
        }

        public IEnumerable<Order> GetOnlineOrders(int count)
        {
            List<Order> orders = new List<Order>();
            orders = orderRepository.GetAll().OrderByDescending(r => r.ActionDate).Take(count).ToList();
            return orders;
        }

        public IEnumerable<Order> GetOnlineOrdersByUsers(string userId, int branchId, int count)
        {
            List<Order> orders = new List<Order>();
            orders = orderRepository.GetMany(r =>
            r.UserId == userId && r.BranchId == branchId
            ).OrderByDescending(r => r.ActionDate).Take(count).ToList();
            return orders;
        }

        public Order GetOrder(string id)
        {
            Order order = orderRepository.Get(r => r.Id == id);

            return order;
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
