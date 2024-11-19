using Application.Data.Infrastructure;
using Application.Data.Repository;
using Application.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{

    public interface ICityService
    {
        void CreateCity(City city);
        void UpdateCity(City city);
        void DeleteCity(City city);
        IEnumerable<City> GetCityList();
        City GetCity(int id);
        City GetCityByName(string name);
        void Commit();
    }

    public class CityService : ICityService
    {
        private readonly ICityRepository cityRepository;
        private readonly IUnitOfWork unitOfWork;

        public CityService(ICityRepository classRepository, IUnitOfWork unitOfWork)
        {
            cityRepository = classRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IClassService Members

        public void CreateCity(City city)
        {
            cityRepository.Add(city);
            Commit();
        }
        public void UpdateCity(City city)
        {
            cityRepository.Update(city);
            Commit();
        }
        public void DeleteCity(City city)
        {
            cityRepository.Delete(city);
            Commit();
        }

        public IEnumerable<City> GetCityList()
        {
            return cityRepository.GetAll().OrderBy(r => r.Name).ToList();
        }

        public City GetCity(int id)
        {
            City city = cityRepository.Get(r => r.Id == id);
            return city;
        }

        public City GetCityByName(string name)
        {
            City city = cityRepository.Get(r => r.Name == name);
            return city;
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
