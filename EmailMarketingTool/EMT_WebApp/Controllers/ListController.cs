using EMT_WebApp.Models;
using EMT_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace EMT_WebApp.Controllers
{
    /// <summary>
    /// controller is responsible to handle all actions related to list creation,updation,deletion
    /// </summary>
    [Authorize(Roles = "User")]
    public class ListController : Controller
    {
        M_List list = null;
        M_Subscriber subscribers = new M_Subscriber();
        List<int?> listIds = new List<int?>();
        ListViewModel LVmodel = new ListViewModel();
        string userID = null;
        /// <summary>
        /// gets currently logged in users id
        /// </summary>
        /// <returns></returns>
        public string GetUser()
        {
             userID = User.Identity.GetUserId();
            return userID;
        }
        // GET: List
        /// <summary>
        /// action displays lists created by perticular user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            list = new M_List();
            LVmodel = new ListViewModel();
            userID = GetUser();

            try
            {
                LVmodel = list.ViewList(userID);
            }
            catch (M_CustomException ex)
            {
                ModelState.AddModelError("listindex", ex.message);
                return View();
            }

            return View(LVmodel);
        }

        /// <summary>
        /// gets view to create list
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult NewList()
        {
            ViewBag.country = new SelectList(S_Country.GetCountries(), "CountryId", "CountryName");
            return View();
        }
        /// <summary>
        /// post create list view to save into database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult NewList(M_List model)
        {

            if (ModelState.IsValid)
            {
                string UsersID = GetUser();
                List<string> ListNames = new List<string>();
                string success = null;
                if (model != null)
                {
                    try
                    {
                        success = model.SaveList(UsersID);
                        if (success != "Saved")
                        {
                            ModelState.AddModelError("Errorlistname", success);
                            ViewBag.country = new SelectList(S_Country.GetCountries(), "CountryId", "CountryName");
                            return View();
                        }
                    }
                    catch (M_CustomException ex)
                    {
                        if (ex.ErrorCode == 100)
                        {
                            ModelState.AddModelError("error", ex.message);
                            return RedirectToAction("NewList");
                        }
                        else if (ex.ErrorCode == 101)
                        {
                            ModelState.AddModelError("Error", "logical exception");
                            return RedirectToAction("NewList");
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("error", "Errr occured in business logic");
                        return RedirectToAction("NewList");
                    }

                }
                else {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// downloads sample excel file to import list
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Authorize]
        public FilePathResult SampleExcel(string fileName)
        {
            return new FilePathResult(@"~\ExcelFiles\" + fileName + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        /// <summary>
        /// downloads sample CSV file to import list
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Authorize]
        public FileContentResult SampleCSV(string fileName)
        {
            //return new FilePathResult(@"~\CSVFiles\" + fileName + ".csv", "text/csv");
            var path = "FirstName, LastName, EmailAddress, AlternateEmailAddress, Address, Country, City";
            return File(new System.Text.UTF8Encoding().GetBytes(path), "text/csv", "SampleCSV.csv");
        }
        /// <summary>
        /// gets view to edit list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditList(int? id)
        {
            ViewBag.country = new SelectList(S_Country.GetCountries(), "CountryId", "CountryName");
            M_List model = new M_List();
            try
            {
                model = model.EditList(id);
            }
            catch (M_CustomException ex)
            {
                if (ex.ErrorCode == 100)
                {
                    ModelState.AddModelError("error", ex.message);
                    return RedirectToAction("EditList");
                }
                else if (ex.ErrorCode == 101)
                {
                    ModelState.AddModelError("Error", "logical exception");
                    return RedirectToAction("EditList");
                }
            }
            return View(model);
        }
        /// <summary>
        /// updates new list information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult EditList(M_List model)
        {
            bool result;
            try
            {
                result = model.UpdateList();
                return RedirectToAction("Index");
            }
            catch (M_CustomException ex)
            {
                ModelState.AddModelError("upadateList", ex.message);
                return RedirectToAction("EditList");
            }
        }
        /// <summary>
        /// unsubscribe users from list
        /// </summary>
        /// <param name="SubscriberID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult UnSubscribe(int? SubscriberID)
        {
            M_Subscriber model = new M_Subscriber();
            bool res;
            if (SubscriberID != null)
            {
                try
                {
                   // res = model.Unsub(SubscriberID);
                    //if (res)
                    //{
                    //    return JavaScript("window.close();");
                    //}
                }
                catch (M_CustomException ex)
                {
                    ModelState.AddModelError("Unsub", ex.message);
                }
            }
            return JavaScript("window.close()");

        }
        public ActionResult Error()
        {
            return View();
        }
    }
}