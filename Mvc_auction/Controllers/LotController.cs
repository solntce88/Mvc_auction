using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc_auction.Models;
using System.IO;

namespace Mvc_auction.Controllers
{
    public class LotController : Controller
    {
        LotRepository lotrep = new LotRepository();
        // GET: /Lot/
        public ActionResult Index()
        {
            var review = new LotRepository().GetAllLots();

            return PartialView(review);
        }


        // GET: /Lot/Details/5
        public ActionResult Details(int id)
        {
            var review = new LotRepository().GetLot(id);
            long ticks = review.DateEnd.Subtract(review.StartTime).Ticks;
            ViewBag.Ticks = review.DateEnd.Subtract(review.StartTime).TotalSeconds;
            if (review == null)
            {
                return RedirectToAction("Http404", "Error");
            }
            LotEditModel newreview = new LotEditModel(review);
            return View(newreview);
        }

        [HttpPost]
        public ActionResult Details(LotEditModel lot, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var review = new LotRepository().GetLot(lot.Id);
                var oldCustomer = new UserRepository().GetDBUser(review.Customer_id);
                if (review.Customer_id != 0)
                {                   
                    MailSender.SendMail(5, oldCustomer, review);
                }
               
                var customer = new UserRepository().GetDBUser(User.Identity.Name);
                if (oldCustomer.user_id != customer.user_id)
                {
                    lotrep.SetPrice(lot.Id, lot.Price, customer.user_id);
                }
                else
                {
                    lotrep.SetPrice(lot.Id, lot.Price);
                }
                ViewBag.PriceValid = "You rate has been accepted";
            }
            var lotEdit = new LotRepository().GetLot(lot.Id);
            LotEditModel newreview = new LotEditModel(lotEdit);
            return View(newreview);
        }

        // GET: /Lot/Create
        [Authorize]
        public ActionResult Create()
        {
            var review = new NewLot();
            return View(review);
        }

        //
        // POST: /Lot/Create
        [Authorize]
        [HandleError]
        [HttpPost]
        public ActionResult Create(Lot newLot, IEnumerable<HttpPostedFileBase> fileUpload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LoadPicture(fileUpload, ref newLot);
                    // TODO: Add insert logic here
                    var user_id = new UserRepository().GetDBUser(User.Identity.Name).user_id;
                    newLot.Owner_id = user_id;
                    var review = new LotRepository().InsertLot(newLot);

                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    return View(newLot);
                }
            }
            return View(newLot);
        }

        //
        // GET: /Lot/Edit/5
         [Authorize]
        [HandleError]
        public ActionResult Edit(int id)
        {
            var review = new LotRepository().GetLot(id);
            if (review == null)
            {
                return RedirectToAction("Http404", "Error");
            }
            int timerTime = review.DateEnd.Subtract(review.StartTime).Milliseconds;
            return View(review);
        }

        //
        // POST: /Lot/Edit/5
        [HandleError]
        [HttpPost]
        [Authorize]
        // роль админа или собственность пользователя
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lot review, IEnumerable<HttpPostedFileBase> fileUpload)
        {
            if (TryUpdateModel(review))
            {
                LoadPicture(fileUpload, ref review);
                LotRepository lotrep = new LotRepository();
                lotrep.UpdateLot(review);

                return RedirectToAction("Index", "Home");
            }
            else
                return View(review);

        }

        private void LoadPicture(IEnumerable<HttpPostedFileBase> fileUpload, ref Lot lot)
        {
            // to do проверить существует ли лот с таким id
            if (fileUpload.FirstOrDefault() != null)
            {
                string[] mimeTypes = { "image/jpeg", "image/gif", "image/png", "image/bmp", "image/tiff", "image/x-jg", "image/x-icon", "image/ief", "image/pjpeg" };
                foreach (var file in fileUpload)
                {
                    if (file == null) continue;

                    string path = AppDomain.CurrentDomain.BaseDirectory + "UploadedFiles";
                    string filename = Path.GetFileName(file.FileName);
                    string type = file.ContentType;
                    bool isCorrect = false;
                    foreach (string mimeType in mimeTypes)
                    {
                        if (string.Equals(type, mimeType))
                        {
                            isCorrect = true;
                            break;
                        }
                    }

                    if (isCorrect)
                    {
                        //удаление старого
                        string picture = lot.Picture;
                        if (picture != null)
                        {
                            DeletePicture(picture);
                        }
                        // сохранение нового
                        string imgPath = "/UploadedFiles/" + filename;
                        lot.Picture = imgPath;
                        if (System.IO.File.Exists(filename))
                        {
                            filename = System.IO.Path.GetRandomFileName();
                        }
                        file.SaveAs(Path.Combine(path, filename));
                    }
                    else
                    {
                        throw new Exception("Type of file is incorrect! Required image file!!!");
                    }
                }
            }
        }
        private void DeletePicture(string file)
        {
            try
            {
                file = file.Remove(0, 1);
                System.IO.File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file));
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        //
        // GET: /Lot/Delete/5
         [Authorize]
        public ActionResult Delete(Lot review)
        {
          //  HttpUtility.HtmlEncode(id);
            Lot lot = lotrep.GetLot(review.Id);
            if (lot == null)
            {/// return RedirectToAction("Http404", "Error");
                throw new Exception("This lot doesn't exist!!!");
            }

            DeletePicture(lot.Picture);
            // послать сообщение об удалении
            User userOwner = new UserRepository().GetDBUser(lot.Owner_id);
            MailSender.SendMail(2, userOwner, lot);
            lotrep.DeleteLot(lot.Id);

            return RedirectToAction("Index", "Home");
        }


       
    }
}
