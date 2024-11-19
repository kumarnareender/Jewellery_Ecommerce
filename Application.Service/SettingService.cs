using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Service.Properties;
using System;

namespace Application.Service
{

    public interface ISettingService
    {
        void CreateSetting(Setting setting);
        void UpdateSetting(Setting setting);
        void DeleteSetting(Setting setting);
        IEnumerable<Setting> GetSettings();
        Setting GetSetting(string name);
        Setting GetSetting(int id);
        void Commit();
    }

    public class SettingService : ISettingService
    {
        private readonly ISettingRepository settingRepository;
        private readonly IUnitOfWork unitOfWork;

        public SettingService(ISettingRepository settingRepository, IUnitOfWork unitOfWork)
        {
            this.settingRepository = settingRepository;
            this.unitOfWork = unitOfWork;           
        }
        
        #region ISettingService Members

        public void CreateSetting(Setting setting)
        {
            settingRepository.Add(setting);
            Commit();
        }

        public IEnumerable<Setting> GetSettings()
        {
            var settings = settingRepository.GetAll().OrderBy(r => r.Name).ToList();
            return settings;
        }

        public Setting GetSetting(string name)
        {
            var setting = settingRepository.Get(r => r.Name == name);

            return setting;
        }

        public Setting GetSetting(int id)
        {
            var setting = settingRepository.Get(r => r.Id == id);

            return setting;
        }

        public void UpdateSetting(Setting setting)
        {
            this.settingRepository.Update(setting);
            Commit();
        }
        public void DeleteSetting(Setting setting)
        {
            this.settingRepository.Delete(setting);
            Commit();
        }
                
        public void Commit()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
