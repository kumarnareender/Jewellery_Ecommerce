using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{

    public interface IOrderPaymentStatusService
    {
        void CreateOrderPaymentStatus(OrderPayment orderPayment);
        void UpdateOrderPaymentStatus(OrderPayment orderPayment);
        void DeleteOrderPaymentStatus(OrderPayment orderPayment);
        IEnumerable<OrderPayment> GetOrderPaymentStatusList();
        OrderPayment GetOrderPaymentStatus(int id);
        List<OrderPayment> GetOrderPaymentStatusByOrder(string orderId);
        void Commit();
    }

    public class OrderPaymentStatusService : IOrderPaymentStatusService
    {
        private readonly IOrderPaymentStatusRepository OrderStatusRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderPaymentStatusService(IOrderPaymentStatusRepository classRepository, IUnitOfWork unitOfWork)
        {
            OrderStatusRepository = classRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IClassService Members

        public void CreateOrderPaymentStatus(OrderPayment orderPayment)
        {
            OrderStatusRepository.Add(orderPayment);
            Commit();
        }
        public void UpdateOrderPaymentStatus(OrderPayment orderPayment)
        {
            OrderStatusRepository.Update(orderPayment);
            Commit();
        }
        public void DeleteOrderPaymentStatus(OrderPayment orderPayment)
        {
            OrderStatusRepository.Delete(orderPayment);
            Commit();
        }

        public IEnumerable<OrderPayment> GetOrderPaymentStatusList()
        {
            return OrderStatusRepository.GetAll().OrderBy(r => r.LastModifiedDate).ToList();
        }

        public OrderPayment GetOrderPaymentStatus(int id)
        {
            OrderPayment OrderStatus = OrderStatusRepository.Get(r => r.Id == id);
            return OrderStatus;
        }
        public List<OrderPayment> GetOrderPaymentStatusByOrder(string orderId)
        {
            List<OrderPayment> OrderStatus = OrderStatusRepository.GetMany(r => r.OrderId == orderId).OrderByDescending(x => x.LastModifiedDate).ToList();
            return OrderStatus;
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
