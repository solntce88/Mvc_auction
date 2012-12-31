using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc_auction.Models;

namespace Mvc_auction.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private UserRepository _db = new UserRepository();
        //
        // GET: /User/

        public ActionResult Index()
        {
            var review = new UserRepository().GetAllUsers();
            return View(review);
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
            var review = new UserRepository().GetDBUser(id);
            return View(review);
        }

        
        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            var review =_db.GetDBUser(id);
            string[] roles=new MyRoleProvider().GetRolesForUser(review.userName);

            ViewBag.Roles = roles[0];
            return View(review);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, User review)
        {
            if (TryUpdateModel(review))
            {
                _db.SaveUser(review);
                return RedirectToAction("Index");
            }

            return View(review);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                User userOwner = new UserRepository().GetDBUser(id);
                MailSender.SendMail(3, userOwner);
                var lotrep = new LotRepository().DeleteUserLots(id);
                _db.DeleteUser(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index");
        }
    }
}
