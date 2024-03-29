﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Mvc_auction.Models;

namespace Mvc_auction.Controllers
{
    public class AccountController : Controller
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
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            if (!RoleService.AdminExists())
            {
                TempData["Message"] = "Необходимо настроить учётную запись администратора перед началом работы приложения.";
                return RedirectToAction("AdminSetup");
            }
            else
               return PartialView();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    //else
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    return PartialView("LogOnWithErr",model);
                }
            }

            // If we got this far, something failed, redisplay form
            //return View(model);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email,model.LastName, model.Name, true, null, out createStatus);
                
                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Welcome", "Home"); //("Index", "Home")
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // **************************************
        // URL: /Account/Activate/username/key
        // **************************************

        public ActionResult Activate(string username, string key)
        {
            UserRepository _user = new UserRepository();
            if (_user.ActivateUser(username, key) == false)
                ViewBag.Status = "Your account wasn't activated";
            else ViewBag.Status = "Your account was activated";
           return  RedirectToAction("Index", "Home");
            //    return RedirectToAction("Index", "Home");
            //else
            //    return RedirectToAction("LogOn");
            
        }
        // **************************************
        // URL: /Account/AdminSetup
        // **************************************

        public ActionResult AdminSetup()
        {
            if (RoleService.AdminExists())
                return RedirectToAction("Index","Home"); //("LogOn");

            return View();
        }

        [HttpPost]
        public ActionResult AdminSetup(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    RoleService.CreateRole("Admin");
                    RoleService.AddUsersToRoles(new string[] { model.UserName }, new string[] { "Admin" });
                    //  FormsService.SignIn(model.UserName, true);
                    FormsAuthentication.Authenticate(model.UserName, model.Password);

                    return RedirectToAction("Admin", "Home");
                }

                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }
            return View(model);
        }


        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
        // GET: /Account/LogOn
        public ActionResult RestorePassword()
        {
            return View();
        }

        // Set: /Account/RestorePassword
        [HttpPost]
        public ActionResult RestorePassword(RestorePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName == Membership.GetUserNameByEmail(model.Email))
                {
                    var userRep = new UserRepository();
                    var user = userRep.GetDBUser(model.UserName);
                    userRep.ChangePassword(model.UserName);
                   var userNew = userRep.GetDBUser(model.UserName);
                   MailSender.SendMail(8, user);
                    ViewBag.message = "The message has been send to your e-mail";
                   return View();
                }
            }
            
                return View(model);
            
        }
    }

}
