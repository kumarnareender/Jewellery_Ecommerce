using Application.Data;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Application.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //BundleTable.EnableOptimizations = true;
            Bootstrapper.Run();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ////// Implement HTTP compression
            ////HttpApplication app = (HttpApplication)sender;
            
            ////// Retrieve accepted encodings
            ////string encodings = app.Request.Headers.Get("Accept-Encoding");
            ////if (encodings != null)
            ////{
            ////    // Check the browser accepts deflate or gzip (deflate takes preference)
            ////    encodings = encodings.ToLower();
            ////    if (encodings.Contains("gzip"))
            ////    {
            ////        app.Response.Filter = new GZipStream(app.Response.Filter, CompressionMode.Compress);
            ////        app.Response.AppendHeader("Content-Encoding", "gzip");
            ////    }
            ////    else if (encodings.Contains("deflate"))
            ////    {
            ////        app.Response.Filter = new DeflateStream(app.Response.Filter, CompressionMode.Compress);
            ////        app.Response.AppendHeader("Content-Encoding", "deflate");
            ////    }                
            ////}
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;

                        using (Application.Data.Models.ApplicationEntities entities = new Application.Data.Models.ApplicationEntities())
                        {
                            Application.Model.Models.User user = entities.Users.SingleOrDefault(u => u.Username == username || u.Email == username);

                            foreach (Application.Model.Models.Role role in user.Roles)
                            {
                                roles += role.Name + ";";
                            }                            
                        }
                        
                        //Let us set the Pricipal with our user specific details
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                          new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
                    }
                    catch (Exception)
                    {
                        //somehting went wrong
                    }
                }
            }
        }
    }
}
