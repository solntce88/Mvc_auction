using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using Mvc_auction.Models;

namespace Mvc_auction
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "User", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "User", action = "Index", id = UrlParameter.Optional }); // Parameter defaults
            routes.MapRoute(
               "Lot", // Route name
               "{controller}/{action}/{id}", // URL with parameters
               new { controller = "Lot", action = "Index", id = UrlParameter.Optional }); // Parameter of Lot route
            
            routes.MapRoute(
                     "Activate",
                    "Account/Activate/{username}/{key}",
                 new
                     {
                     controller = "Account",
                     action = "Activate",
                     username = UrlParameter.Optional,
                     key = UrlParameter.Optional
                     });
            routes.MapRoute("Error", "{*url}"/*"{controller}/{action}"*/, new {controller = "Error",action = "Http404"});

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

           Thread check = new Thread(MailChecker.CheckAndSend);            
            check.Priority = ThreadPriority.AboveNormal;
            check.IsBackground = true;
            check.Name = "MailChecker";
            while (true)
            {
                check.Start();
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
           
           
        }
    }
}