using Application.Model.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using System.Drawing;
using System.IO;
using Application.ViewModel;
using System.Text.RegularExpressions;


namespace Application.Common
{
    public static class Utils
    {
        private static HttpContext _context { get { return HttpContext.Current; } }

        public static string GetFormattedDate(DateTime? datetime)
        {
            if (datetime == null)
            {
                return "";
            }
            else
            {
                return ((DateTime)datetime).ToString("dd MMM, yyyy hh:mm tt");
            }
        }
        public static string GetSetting(string name)
        {
            string value = String.Empty;

            List<Setting> settingList = new List<Setting>();
            System.Runtime.Caching.ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;

            if (cache.Contains(ConstKey.ckSettings))
            {
                settingList = (List<Setting>)cache.Get(ConstKey.ckSettings);

                foreach (var setting in settingList)
                {
                    if (setting.Name.ToLower() == name.ToLower())
                    {
                        return setting.Value;
                    }
                }
            }

            return value;
        }

        public static object GetStringValue(string value)
        {
            if (String.IsNullOrEmpty(value))
                return DBNull.Value;
            else
                return value;
        }
        public static object GetDateValue(DateTime? value = null)
        {
            if (value == null)
                return DBNull.Value;
            else
                return value;
        }
        public static object GetIntValue(int? value = null)
        {
            if (value == null)
                return DBNull.Value;
            else
                return value;
        }
        public static string GetConfigValue(string name)
        {
            if (ConfigurationManager.AppSettings[name] == null)
            {
                return String.Empty;
            }
            else
            {
                return ConfigurationManager.AppSettings[name];
            }
        }

        public static string GetCurrencyCode()
        {
            return GetConfigValue("CurrencySymbol");
        }

        public static string GetHotlineNumber()
        {
            return GetSetting("CompanyPhone");
        }

        public static string GetPlaystoreLink()
        {
            return GetSetting("AndroidAppUrl");
        }

        public static string GetAppstoreLink()
        {
            return GetSetting("iOSAppUrl");
        }

        public static string GetFacebookLink()
        {
            return GetSetting("FacebookPage");
        }

        public static string GetTwitterLink()
        {
            return GetSetting("TwitterPage");
        }

        public static string GetLinkedInLink()
        {
            return GetSetting("LinkedInPage");
        }

        public static string GetProductImageSrcPrefix()
        {
            return GetConfigValue("ProductImageSrc") + "/ProductImages";
        }
        public static string GetTableImageSrcPrefix()
        {
            return GetConfigValue("ProductImageSrc") + "/TableImages";
        }

        public static string GetPhotoSrcPrefix()
        {
            return GetConfigValue("PhotoSrc") + "/Photos";
        }

        public static User GetLoggedInUser()
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["User"] != null)
            {
                return HttpContext.Current.Session["User"] as User;
            }
            else
            {
                return null;
            }
        }

        public static string GetLoggedInUserId()
        {
            string userId = String.Empty;

            User user = GetLoggedInUser();
            if (user != null)
            {
                return user.Id;
            }

            return userId;
        }

        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string GetCookie(string name)
        {
            string value = String.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                value = cookie.Value;
            }

            return value;
        }

        public static int GetBranchId()
        {
            string branchId = GetCookie("branchId");
            if (branchId == "")
            {
                return 0;
            }
            return Convert.ToInt32(branchId);
        }


        public static string GetLoggedInUserName()
        {
            string name = String.Empty;
            if (HttpContext.Current.Session["User"] != null)
            {
                User uvm = HttpContext.Current.Session["User"] as User;
                if (uvm != null)
                {
                    name = uvm.FirstName + " " + uvm.LastName;
                }
            }

            if (!String.IsNullOrEmpty(name))
            {
                var wordsArray = name.Split();

                if (wordsArray.Count() >= 2)
                {
                    name = wordsArray[0] + ' ' + wordsArray[1];
                }
            }

            return name;
        }

        public static string GetUserFirstName()
        {
            string firstName = String.Empty;
            if (HttpContext.Current.Session["User"] != null)
            {
                User uvm = HttpContext.Current.Session["User"] as User;
                if (uvm != null)
                {
                    return uvm.FirstName;
                }
            }

            return firstName;
        }

        public static string GetCompanyName()
        {
            return GetSetting(ESetting.CompanyName.ToString());
        }

        public static void GetPrice(decimal? price, decimal? discount, out decimal newPrice, out string priceText, out string oldPriceText)
        {
            priceText = String.Empty;
            oldPriceText = String.Empty;
            newPrice = 0;

            if (discount != null && ((decimal)discount) > 0)
            {
                decimal currentPrice = (decimal)price - (decimal)discount;
                newPrice = currentPrice;
                priceText = GetCurrencyCode() + String.Format("{0:N0}", currentPrice);
                oldPriceText = GetCurrencyCode() + String.Format("{0:00.00}", price);
            }
            else
            {
                newPrice = price == null ? 0 : (decimal)price;
                priceText = GetCurrencyCode() + String.Format("{0:00.00}", price);
            }
        }

        public static string GetFileText(string filePath)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(filePath));
            }
            catch
            {
                sr = new StreamReader(filePath);
            }

            string strOut = sr.ReadToEnd();
            sr.Close();
            return strOut;
        }

        public static string GetBidTimeRemaining(DateTime d1, DateTime d2)
        {
            string timeRemains = String.Empty;

            TimeSpan span = d2.Subtract(d1);
            int sec = span.Seconds;
            int mins = span.Minutes;
            int hours = span.Hours;
            int days = span.Days;

            if (days > 0)
            {
                timeRemains += days.ToString() + "d ";
            }

            if (hours > 0 || days > 0)
            {
                timeRemains += hours.ToString() + "h ";
            }

            if (mins > 0 || hours > 0)
            {
                timeRemains += mins.ToString() + "m ";
            }

            return timeRemains;
        }

        public static string GetDateDifference(DateTime d1, DateTime d2)
        {
            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            DateTime fromDate;
            DateTime toDate;
            int year;
            int month;
            int day;
            int increment = 0;

            if (d1 > d2)
            {
                fromDate = d2;
                toDate = d1;
            }
            else
            {
                fromDate = d1;
                toDate = d2;
            }

            // Calculating Days
            if (fromDate.Day > toDate.Day)
            {
                increment = monthDay[fromDate.Month - 1];
            }

            if (increment == -1)
            {
                if (DateTime.IsLeapYear(fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }

            if (increment != 0)
            {
                day = (toDate.Day + increment) - fromDate.Day;
                increment = 1;
            }
            else
            {
                day = toDate.Day - fromDate.Day;
            }

            // Month Calculation
            if ((fromDate.Month + increment) > toDate.Month)
            {
                month = (toDate.Month + 12) - (fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                month = (toDate.Month) - (fromDate.Month + increment);
                increment = 0;
            }

            // Year Calculation
            year = toDate.Year - (fromDate.Year + increment);

            string text = day + " days";

            if (month > 0)
            {
                text = month + " months " + text;
            }

            if (year > 0)
            {
                text = year + " years " + text;
            }

            if (text == "0 days") { text = "Today"; }

            return text;
        }

        public static string GetBidTimeLeft(DateTime bidEndDate, DateTime now)
        {
            if (bidEndDate < now)
            {
                return "Ended";
            }

            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            DateTime fromDate;
            DateTime toDate;
            int year;
            int month;
            int day;
            int hour;
            int min;
            int increment = 0;
            string suffix = String.Empty;

            fromDate = now;
            toDate = bidEndDate;

            // Calculating Mins
            if ((fromDate.Minute + increment) > toDate.Minute)
            {
                min = (toDate.Minute + 60) - (fromDate.Minute + increment);
                increment = 1;
            }
            else
            {
                min = (toDate.Minute) - (fromDate.Minute + increment);
                increment = 0;
            }

            // Calculating Hours
            if ((fromDate.Hour + increment) > toDate.Hour)
            {
                hour = (toDate.Hour + 24) - (fromDate.Hour + increment);
                increment = 1;
            }
            else
            {
                hour = (toDate.Hour) - (fromDate.Hour + increment);
                increment = 0;
            }

            // Calculating Days
            if (fromDate.Day > toDate.Day)
            {
                increment = monthDay[fromDate.Month - 1];
            }

            if (increment == -1)
            {
                if (DateTime.IsLeapYear(fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }

            if (increment != 0)
            {
                day = (toDate.Day + increment) - fromDate.Day;
                increment = 1;
            }
            else
            {
                day = toDate.Day - fromDate.Day;
            }

            // Month Calculation
            if ((fromDate.Month + increment) > toDate.Month)
            {
                month = (toDate.Month + 12) - (fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                month = (toDate.Month) - (fromDate.Month + increment);
                increment = 0;
            }

            // Year Calculation
            year = toDate.Year - (fromDate.Year + increment);

            suffix = day > 1 ? " days" : " day";
            string text = day + suffix;

            if (month > 0)
            {
                suffix = month > 1 ? " months " : " month ";
                text = month + suffix + text;
            }

            if (year > 0)
            {
                suffix = year > 1 ? " years " : " year ";
                text = year + suffix + text;
            }

            if (text == "0 day") { text = "Today"; }

            if (hour > 0)
            {
                suffix = hour > 1 ? " hours " : " hour ";
                text = text + " " + hour + suffix;
            }

            if (min > 0)
            {
                suffix = min > 1 ? " mins " : " min ";
                text = text + " " + min + " mins ";
            }

            if (day < 0)
            {
                text = "Ended";
            }

            return text;
        }

        public static string GenerateSeoTitle(string id, string name, int length = 45)
        {
            string phrase = string.Format("{0}-{1}", id, name);

            string str = RemoveAccent(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= length ? str.Length : length).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        private static string RemoveAccent(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string GetBarcode(int productId)
        {
            string barcode = productId.ToString().PadLeft(5, '0');
            return barcode;
        }

        public static string HasPermission(string text)
        {
            string access = "none";
            User user = GetLoggedInUser();
            if (user != null)
            {
                if (!String.IsNullOrEmpty(user.Permissions) && user.Permissions.Contains(text))
                {
                    access = "";
                }
                else
                {
                    access = "none";
                }
            }

            return access;
        }
    }
}
