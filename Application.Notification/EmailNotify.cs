using Application.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Notification
{
    public class EmailNotify
    {        
        public async Task<bool> SendEmail(string to, string subject, string message, bool isNewThread = false)
        {
            // Smtp configuration
            var senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
            var senderEmailPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];
            var senderName = ConfigurationManager.AppSettings["SenderName"];
            var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            var smtpPort = ConfigurationManager.AppSettings["SmtpPort"];    

            // Deliver the message
            bool isSuccess = await DeliverEmail(isNewThread, senderEmail, senderEmailPassword, senderName, smtpHost, smtpPort, to, subject, message);
            
            return isSuccess;
        }

        private async Task<bool> DeliverEmail(bool isNewThread, string senderEmail, string senderEmailPassword, string senderName, string smtpHost, string smtpPort, string to, string subject, string message)
        {
            bool isSuccess = true;
            MailAddress emailFrom = new MailAddress(senderEmail, senderName);
            MailAddress emailTo = new MailAddress(to, to);
            MailMessage mail = new MailMessage(emailFrom, emailTo);
            
            SmtpClient client = new SmtpClient();
            client.Port = int.Parse(smtpPort);
            client.Host = smtpHost;
            //client.Host = "localhost";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(senderEmail, senderEmailPassword);
            mail.To.Add(emailTo); 
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = message;
            mail.BodyEncoding = UTF8Encoding.UTF8;
            mail.SubjectEncoding = System.Text.Encoding.Default;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            
            try
            {
                if (isNewThread)
                {
                    Thread thread = new Thread(delegate()
                    {
                        client.SendMailAsync(mail);
                    });

                    thread.Start();
                }
                else
                {
                    await client.SendMailAsync(mail);
                }
            }
            catch(Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError("Send Email Failed: " + subject);
            }

            return isSuccess;
        }
    }
}
