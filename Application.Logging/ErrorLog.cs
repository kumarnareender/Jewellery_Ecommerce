using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elmah;
using System.Web;

namespace Application.Logging
{
    public static class ErrorLog
    {
        public static void LogError(Exception ex, string contextualMessage = null)
        {
            try
            {
                if (contextualMessage != null)
                {
                    var annotatedException = new Exception(contextualMessage, ex);
                    ErrorSignal.FromCurrentContext().Raise(annotatedException, HttpContext.Current);
                }
                else
                {
                    ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                }              
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }

        public static void LogError(string message)
        {
            try
            {
                if (!String.IsNullOrEmpty(message))
                {
                    ErrorSignal.FromCurrentContext().Raise(new Exception(message), HttpContext.Current);
                }                
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }
    }
}
