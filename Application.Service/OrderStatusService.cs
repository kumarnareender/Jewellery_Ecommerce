using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Common;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface IOrderStatusService
    {
        void CreateOrderStatus(OrderStatus OrderStatus);
        void UpdateOrderStatus(OrderStatus OrderStatus);
        void DeleteOrderStatus(OrderStatus OrderStatus);
        IEnumerable<OrderStatus> GetOrderStatusList();
        OrderStatus GetOrderStatus(int id);
        List<OrderStatus> GetOrderStatusByOrder(string orderId);
        void Commit();
    }

    public class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderStatusRepository OrderStatusRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderStatusService(IOrderStatusRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.OrderStatusRepository = classRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IClassService Members

        public void CreateOrderStatus(OrderStatus OrderStatus)
        {
            this.OrderStatusRepository.Add(OrderStatus);
            Commit();
        }
        public void UpdateOrderStatus(OrderStatus OrderStatus)
        {
            this.OrderStatusRepository.Update(OrderStatus);
            Commit();
        }
        public void DeleteOrderStatus(OrderStatus OrderStatus)
        {
            this.OrderStatusRepository.Delete(OrderStatus);
            Commit();
        }

        public IEnumerable<OrderStatus> GetOrderStatusList()
        {
            return this.OrderStatusRepository.GetAll().OrderBy(r => r.LastModifiedDate).ToList();
        }

        public OrderStatus GetOrderStatus(int id)
        {
            var OrderStatus = OrderStatusRepository.Get(r => r.Id == id);
            return OrderStatus;
        }
        public List<OrderStatus> GetOrderStatusByOrder(string orderId)
        {
            var OrderStatus = OrderStatusRepository.GetMany(r => r.OrderId == orderId).OrderByDescending(x => x.LastModifiedDate).ToList();
            return OrderStatus;
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
