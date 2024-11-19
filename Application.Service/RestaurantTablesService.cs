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

    public interface IRestaurantTablesService
    {
        void CreateRestTable(RestaurantTable restaurantTable);
        void UpdateRestTable(RestaurantTable restaurantTable);
        void DeleteRestTable(RestaurantTable restaurantTables);
        List<RestaurantTable> GetRestTablesList();
        RestaurantTable GetRestTable(int id);
        RestaurantTable GetRestTableByTableNumber(int tableNumber);
        RestaurantTable GetRestTableByOrderId(string orderId);
        void Commit();
    }

    public class RestaurantTablesService : IRestaurantTablesService
    {
        private readonly IRestaurantTablesRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public RestaurantTablesService(IRestaurantTablesRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.repository = classRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IClassService Members

        public void CreateRestTable(RestaurantTable restTable)
        {
            this.repository.Add(restTable);
            Commit();
        }
        public RestaurantTable GetRestTableByOrderId(string orderId)
        {
            var RestTable = repository.Get(r => r.OrderId == orderId);
            return RestTable;

        }
        public void UpdateRestTable(RestaurantTable restTable)
        {
            this.repository.Update(restTable);
            Commit();
        }
        public void DeleteRestTable(RestaurantTable restTable)
        {
            this.repository.Delete(restTable);
            Commit();
        }

        public List<RestaurantTable> GetRestTablesList()
        {
            return this.repository.GetAll().OrderBy(r => r.TableNumber).ToList();
        }

        public RestaurantTable GetRestTable(int id)
        {
            var RestTable = repository.Get(r => r.Id == id);
            return RestTable;
        }

        public RestaurantTable GetRestTableByTableNumber(int tableNumber)
        {
            var RestTable = repository.Get(r => r.TableNumber == tableNumber);
            return RestTable;
        }
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
