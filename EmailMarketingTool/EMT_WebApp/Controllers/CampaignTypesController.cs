using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EMT_WebApp.Models;

namespace EMT_WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CampaignTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CampaignTypes
        public ActionResult Index()
        {
            return View(db.S_CampaignTypes.ToList());
        }

        // GET: CampaignTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            S_CampaignTypes s_CampaignTypes = db.S_CampaignTypes.Find(id);
            if (s_CampaignTypes == null)
            {
                return HttpNotFound();
            }
            return View(s_CampaignTypes);
        }

        // GET: CampaignTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CampaignTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CTId,Name,Description,IsActive")] S_CampaignTypes s_CampaignTypes)
        {
            if (ModelState.IsValid)
            {
                db.S_CampaignTypes.Add(s_CampaignTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(s_CampaignTypes);
        }

        // GET: CampaignTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            S_CampaignTypes s_CampaignTypes = db.S_CampaignTypes.Find(id);
            if (s_CampaignTypes == null)
            {
                return HttpNotFound();
            }
            return View(s_CampaignTypes);
        }

        // POST: CampaignTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CTId,Name,Description,IsActive")] S_CampaignTypes s_CampaignTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(s_CampaignTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(s_CampaignTypes);
        }

        // GET: CampaignTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            S_CampaignTypes s_CampaignTypes = db.S_CampaignTypes.Find(id);
            if (s_CampaignTypes == null)
            {
                return HttpNotFound();
            }
            return View(s_CampaignTypes);
        }

        // POST: CampaignTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            S_CampaignTypes s_CampaignTypes = db.S_CampaignTypes.Find(id);
            db.S_CampaignTypes.Remove(s_CampaignTypes);
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
