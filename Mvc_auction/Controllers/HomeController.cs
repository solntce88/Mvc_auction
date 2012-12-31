using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc_auction.Models;
using System.Web.Security;
using System.Web.Routing;

namespace Mvc_auction.Controllers
{
    public class HomeController : Controller
    {
        public IRoleService RoleService { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
            if (RoleService == null)
            {
                RoleService = new AccountRoleService();
            }
            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {
            ViewBag.Title = System.Configuration.ConfigurationManager.AppSettings["SiteName"];       
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        //public ActionResult Admin()
        //{
        //    return View();
        //}
      
    }
}
