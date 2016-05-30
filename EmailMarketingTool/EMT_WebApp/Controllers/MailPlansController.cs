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
    [Authorize(Roles = "User")]
    public class MailPlansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: M_MailPlans
        public ActionResult Index()
        {
            return View(db.M_MailPlans.ToList());
        }

        // GET: M_MailPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            M_MailPlans m_MailPlans = db.M_MailPlans.Find(id);
            if (m_MailPlans == null)
            {
                return HttpNotFound();
            }
            return View(m_MailPlans);
        }

        // GET: M_MailPlans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: M_MailPlans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PlanId,PlanName,IsActive")] M_MailPlans m_MailPlans)
        {
            if (ModelState.IsValid)
            {
                db.M_MailPlans.Add(m_MailPlans);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(m_MailPlans);
        }

        // GET: M_MailPlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            M_MailPlans m_MailPlans = db.M_MailPlans.Find(id);
            if (m_MailPlans == null)
            {
                return HttpNotFound();
            }
            return View(m_MailPlans);
        }

        // POST: M_MailPlans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlanId,PlanName,IsActive")] M_MailPlans m_MailPlans)
        {
            if (ModelState.IsValid)
            {
                db.Entry(m_MailPlans).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(m_MailPlans);
        }

        // GET: M_MailPlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            M_MailPlans m_MailPlans = db.M_MailPlans.Find(id);
            if (m_MailPlans == null)
            {
                return HttpNotFound();
            }
            return View(m_MailPlans);
        }

        // POST: M_MailPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            M_MailPlans m_MailPlans = db.M_MailPlans.Find(id);
            db.M_MailPlans.Remove(m_MailPlans);
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
