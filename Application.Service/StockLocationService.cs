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

    public interface IStockLocationService
    {
        void CreateStockLocation(StockLocation stockLocation);
        void UpdateStockLocation(StockLocation stockLocation);
        void DeleteStockLocation(StockLocation stockLocation);
        IEnumerable<StockLocation> GetStockLocationList();
        StockLocation GetStockLocation(int id);
        void Commit();
    }

    public class StockLocationService : IStockLocationService
    {
        private readonly IStockLocationRepository stockLocationRepository;
        private readonly IUnitOfWork unitOfWork;

        public StockLocationService(IStockLocationRepository classRepository, IUnitOfWork unitOfWork)
        {
            this.stockLocationRepository = classRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region IClassService Members

        public void CreateStockLocation(StockLocation stockLocation)
        {
            this.stockLocationRepository.Add(stockLocation);
            Commit();
        }
        public void UpdateStockLocation(StockLocation stockLocation)
        {
            this.stockLocationRepository.Update(stockLocation);
            Commit();
        }
        public void DeleteStockLocation(StockLocation stockLocation)
        {
            this.stockLocationRepository.Delete(stockLocation);
            Commit();
        }

        public IEnumerable<StockLocation> GetStockLocationList()
        {
            return this.stockLocationRepository.GetAll().OrderBy(r => r.Name).ToList();
        }

        public StockLocation GetStockLocation(int id)
        {
            var stockLocation = stockLocationRepository.Get(r => r.Id == id);
            return stockLocation;
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
