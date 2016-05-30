using EMT_WebApp.Models;
using EMT_WebApp.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel;
using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.IO;

namespace EMT_WebApp.Controllers
{
    /// <summary>
    /// controller is responsible for actions related to subscribers
    /// </summary>
    /// 
    //[Authorize]
    public class SubscriberController : Controller
    {
        string userID = null;
        M_List list = null;
        M_Subscriber subscriber = null;
        /// <summary>
        /// gets userid of currently logged in user
        /// </summary>
        /// <returns></returns>
        public string GetUser()
        {
            userID = User.Identity.GetUserId();
            return userID;
        }
        /// <summary>
        /// gets view to add single subscriber to perticular list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Subscriber
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult AddSubcriber(int? id)
        {
            userID = GetUser();
            list = new M_List();
            subscriber = new M_Subscriber();
            if (id == null)
            {
                // ViewBag.ListName = new SelectList(dbcontext.NewLists, "ListID", "ListName", "Select List");
                ViewBag.ListName = new SelectList(M_List.GetLists(), "ListID", "ListName", "Select List");
            }
            else {
                subscriber.ListID = id;
                return View(subscriber);
            }
            return View(subscriber);
        }
        /// <summary>
        /// saves subscriber to perticular list
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult AddSubcriber(M_Subscriber model)
        {
            userID = GetUser();
            if (model != null)
            {
                try
                {
                    model.saveSubscriber(userID);
                }
                catch (M_CustomException ex)
                {
                    ModelState.AddModelError("addsub", ex.message);
                    return RedirectToAction("AddSubcriber/" + model.ListID);
                }

            }
            return RedirectToAction("Index", "List");
        }
        /// <summary>
        /// view all subscribers to perticular list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult ViewSubscribers(int? id, int? page)
        {
            userID = GetUser();
            const int pageSize = 30;
            int pageNumber = (page ?? 1);
            subscriber = new M_Subscriber();
            List<M_Subscriber> subscribersToList = new List<M_Subscriber>();
            if (id != null)
            {
                ViewBag.ListID = id;
                try
                {
                    subscribersToList = subscriber.GetSubscribersbyListID(id);
                    return View(subscribersToList.ToPagedList(pageNumber, pageSize));
                }
                catch (M_CustomException ex)
                {
                    ModelState.AddModelError("viewsub", ex.message);
                    return RedirectToAction("index", "List");
                }

            }
            else {
                try
                {
                    subscribersToList = subscriber.GetAllSubscribers(userID);
                    return View(subscribersToList.ToPagedList(pageNumber, pageSize));
                }
                catch (M_CustomException ex)
                {
                    ModelState.AddModelError("viewsub", ex.message);
                    return RedirectToAction("index", "Home");
                };
            }
        }
        /// <summary>
        /// gets view to import subscribers for list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult ImportSubcriber(int? id)
        {
            SubscribersViewModel model = new SubscribersViewModel();
            if (id == null)
            {
                ViewBag.ListName = new SelectList(M_List.GetLists(), "ListID", "ListName", "Select List");
            }
            else {
                model.ListID = id;
                return View(model);
            }
            return View(model);
        }
        /// <summary>
        /// takes excel or CSV file uploaded by user and save list of subscribers to database fro peticular list
        /// </summary>
        /// <param name="UploadFile"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ImportSubcriber(HttpPostedFileBase UploadFile, SubscribersViewModel model)
        {
            subscriber = new M_Subscriber();
            if (ModelState.IsValid)
            {
                if (UploadFile != null && UploadFile.ContentLength > 0)
                {
                    if (UploadFile.FileName.EndsWith(".xlsx"))
                    {
                        try
                        {
                            subscriber.ImportExcel(UploadFile, model);
                        }
                        catch (M_CustomException ex)
                        {
                            ModelState.AddModelError("importsub", ex.message);
                            return RedirectToAction("ImportSubcriber/" + model.ListID);
                        }

                    }
                    else if (UploadFile.FileName.EndsWith(".xls"))
                    {
                        try
                        {
                            subscriber.ImportExcel(UploadFile, model);
                        }
                        catch (M_CustomException ex)
                        {
                            ModelState.AddModelError("importsub", ex.message);
                            return RedirectToAction("ImportSubcriber/" + model.ListID);
                        }

                    }
                    else if (UploadFile.FileName.EndsWith(".csv"))
                    {
                        try
                        {
                            subscriber.ImportCSV(UploadFile, model);
                        }
                        catch (M_CustomException ex)
                        {
                            ModelState.AddModelError("importsub", ex.message);
                            return RedirectToAction("ImportSubcriber/" + model.ListID);
                        }
                    }
                }
            }
            return RedirectToAction("ViewSubscribers", model.ListID);
        }
        /// <summary>
        /// get edit view for perticular subscriber
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult EditSusbscriber(int? id)
        {
            if (id != null)
            {
                M_Subscriber model = new M_Subscriber();
                try
                {
                    model = model.Edit(id);
                    return View(model);
                }
                catch (M_CustomException ex)
                {
                    ModelState.AddModelError("editsub", ex.message);
                    return RedirectToAction("EditSusbscriber/" + id);
                }
            }
            else {
                ModelState.AddModelError("idnull", "Error while processing request contact administrator");
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// update changed information for perticular subscriber
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult EditSusbscriber(M_Subscriber model)
        {
            if (model != null)
            {
                try
                {
                    model.Update();
                    return RedirectToAction("ViewSubscribers/" + model.ListID);
                }
                catch (M_CustomException ex)
                {
                    ModelState.AddModelError("editsub", ex.message);
                    return RedirectToAction("EditSusbscriber/" + model.SubscriberID);
                }
            }
            else {
                ModelState.AddModelError("modelnull", "Error while processing request contact administrator");
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// delete subscriber from list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        public ActionResult DeleteSusbscriber(int? id)
        {
            M_Subscriber model = new M_Subscriber();
            if (id != null)
            {
                try
                {
                    model.Delete(id);
                    return RedirectToAction("Index", "List");
                }
                catch (M_CustomException ex)
                {
                    ModelState.AddModelError("editsub", ex.message);
                    return RedirectToAction("ViewSubscribers/" + ViewBag.id);
                }
            }
            else {
                ModelState.AddModelError("idnull", "Error while processing request contact administrator");
                return RedirectToAction("Index", "List");
            }
        }
        /// <summary>
        /// unsubscribe subscriber from list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Unsubscribers(int? id)
        {
            userID = GetUser();
            subscriber = new M_Subscriber();
            SubscribersViewModel model = new SubscribersViewModel();
            try
            {
                model.SubscribersToList = subscriber.Unsubscriber(id);
                return View(model);
            }
            catch (M_CustomException ex)
            {
                ModelState.AddModelError("unsub", ex.message);
                return RedirectToAction("Index", "List");
            }
        }
        [AllowAnonymous]
        public ActionResult UnsubscribeEmail(string subid)
        {
            try
            {
                M_Subscriber.Unsub(subid);
                return JavaScript("alert('successfully unsubscribed');");
            }
            catch (M_CustomException ex)
            {
                ModelState.AddModelError("editsub", ex.message);
                return RedirectToAction("ViewSubscribers/" + ViewBag.id);
            }
            
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}