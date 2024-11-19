using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class SettingController : Controller
    {
        private ISettingService settingService;

        public SettingController(ISettingService settingService)
        {
            this.settingService = settingService;
        }

        public ActionResult Setting()
        {
            return View();
        }
        public JsonResult GetSettingList()
        {
            var itemList = this.settingService.GetSettings();

            List<Setting> list = new List<Setting>();
            foreach (var item in itemList)
            {
                list.Add(new Setting { Id = item.Id, Name = item.Name, Value = item.Value });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSettingNameList()
        {
            List<string> list = new List<string>();

            foreach (var setting in Enum.GetValues(typeof(ESetting)))
            {
                list.Add(setting.ToString());
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateSetting(Setting setting)
        {
            bool isSuccess = true;
            try
            {
                Setting s = this.settingService.GetSetting(setting.Name);
                if (s == null)
                {
                    this.settingService.CreateSetting(setting);
                    UpdateSettingCacheValue();
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSetting(Setting setting)
        {
            bool isSuccess = true;
            try
            {
                this.settingService.UpdateSetting(setting);
                UpdateSettingCacheValue();
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteSetting(Setting setting)
        {
            bool isSuccess = true;
            try
            {
                this.settingService.DeleteSetting(setting);
                UpdateSettingCacheValue();
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        private void UpdateSettingCacheValue()
        {
            List<Setting> settingList = new List<Setting>();
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(ConstKey.ckSettings))
            {
                cache.Remove(ConstKey.ckSettings);
            }

            // Get all settings from DB
            settingList = this.settingService.GetSettings().ToList();

            // Store data in the cache
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.SlidingExpiration = TimeSpan.FromDays(1);
            cache.Add(ConstKey.ckSettings, settingList, cacheItemPolicy);

        }
	}
}