﻿using Live_Quiz.Models;
using Live_Quiz.Models.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace Live_Quiz.Controllers
{
    [Authorize]

    public class CollectionsController : Controller
    {
        private static ApplicationDbContext context = new ApplicationDbContext();

        UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        private DataModel db = new DataModel();

        // GET: Collections
        public ActionResult Index()
        {
            string idd = User.Identity.GetUserId();
            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId == idd);


            return View(userPr.Collections.ToList() as IEnumerable<Collection>);
        }

        // GET: Collections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection collection = db.Collections.Find(id);
            if (collection == null)
            {
                return HttpNotFound();
            }
            return View(collection);
        }

        // GET: Collections/Create
        public ActionResult Create()
        {
            //ViewBag.email = UserManager.FindById(User.Identity.GetUserId()).Email;
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            HttpPostedFileBase file = Request.Files["ImageData"];
            //ContentRepository service = new ContentRepository();
            if (file==null)
            {
                return View();
            }
            ContentRepository service = new ContentRepository();
            //ImageFile imageFile = new ImageFile();
            int i = service.UploadImageInDataBase(file,new ImageFielView() { });
            
          
             
            if (i == 0)
            {
                return View(collection);
            }
           
            string idd = User.Identity.GetUserId();

            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));

            collection.Email = UserManager.FindById(idd).Email;
            collection.User = userPr;
            collection.UserProfileId = userPr.Id;
            collection.ImageId = i;
            db.Collections.Add(collection);
            db.SaveChanges();
            //db.SaveChanges();
            //return View(collection);
            return RedirectToAction("Index");

        }

        // GET: Collections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection collection = db.Collections.Find(id);
            if (collection == null)
            {
                return HttpNotFound();
            }
            return View(collection);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,isPublic,Description")] Collection collection)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(collection);
        }

        // GET: Collections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection collection = db.Collections.Find(id);
            if (collection == null)
            {
                return HttpNotFound();
            }
            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Collection collection = db.Collections.Find(id);
            db.Collections.Remove(collection);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = GetImageFromDataBase(id);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }

        public byte[] GetImageFromDataBase(int Id)
        {
            var q = from temp in db.Images where temp.Id == Id select temp.Image;
            byte[] cover = q.First();
            return cover;
        }
    }
}