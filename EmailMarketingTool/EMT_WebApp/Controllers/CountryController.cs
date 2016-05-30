using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EMT_WebApp.Models;
using System.Data.SqlClient;

namespace EMT_WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Country
        public ActionResult Index()
        {
            return View(S_Country.GetCountries());
        }

        // GET: Country/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            S_Country s_Country = db.S_Countries.Find(id);
            if (s_Country == null)
            {
                return HttpNotFound();
            }
            return View(s_Country);
        }

        // GET: Country/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CountryID,CountryName")] S_Country s_Country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //db.S_Countries.Add(s_Country);
                    //db.SaveChanges();
                    S_Country.SaveCountry(s_Country);
                    return RedirectToAction("Index");
                }
                catch (M_CustomException ex)
                {
                    
                }
             
            }

            return View(s_Country);
        }

        // GET: Country/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            S_Country s_Country = db.S_Countries.Find(id);
            if (s_Country == null)
            {
                return HttpNotFound();
            }
            return View(s_Country);
        }

        // POST: Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CountryID,CountryName")] S_Country s_Country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(s_Country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(s_Country);
        }

        // GET: Country/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            S_Country s_Country = db.S_Countries.Find(id);
            if (s_Country == null)
            {
                return HttpNotFound();
            }
            return View(s_Country);
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            S_Country s_Country = db.S_Countries.Find(id);
            db.S_Countries.Remove(s_Country);
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
    }
}
