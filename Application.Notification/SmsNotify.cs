using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notification
{
    public class SmsNotify
    {        
        public int SendSMS(string to, string message, bool isBangla)
        {
            int successCount = 0;

            if (String.IsNullOrEmpty(to))
            {
                return 0;
            }

            to = to.TrimEnd(',');
            string[] smsToList = to.Split(',').Distinct().ToArray();

            foreach (string mobileNo in smsToList)
            {
                bool isSuccess = DeliverSMS("88" + RemoveBDCode(mobileNo), message, isBangla);
                if (isSuccess)
                {
                    successCount++;
                }
            }

            return successCount;
        }

        public bool DeliverSMS(string to, string message, bool isBangla)
        {
            return true;
        }

        private string RemoveBDCode(string mobile)
        {
            if (mobile.StartsWith("88"))
            {
                mobile = mobile.Substring(2);
            }
            else if (mobile.StartsWith("+88"))
            {
                mobile = mobile.Substring(3);
            }

            return mobile;
        }

    }
}
