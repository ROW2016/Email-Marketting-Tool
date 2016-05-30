using System;
using System.Web;
using System.Web.Mvc;
using EMT_WebApp.Models;
using Microsoft.AspNet.Identity;


namespace EMT_WebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class UserProfileController : Controller
    {
        string userID = null;
        public string GetUser()
        {
            userID = User.Identity.GetUserId();
            //userID = "5387c8c4-ffc3-4b30-8712-efa591b4a062";
            return userID;
        }
        // GET: UserProfile
        public ActionResult Index()
        {
            userID = GetUser();
            int id = UsersProfile.findProfile(userID);
            if (id == 0)
            {
                return View();
            }
            else {
               // TempData["id"] = id;
                return RedirectToAction("ShowProfile");
            }
        }

        [HttpPost]
        public ActionResult Save(HttpPostedFileBase Uploadlogo, M_Profile model)
        {
            int pid=0;
            userID = GetUser();
            if (ModelState.IsValid)
            {
                if (Uploadlogo != null && Uploadlogo.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(Uploadlogo.FileName);
                    string ext = System.IO.Path.GetExtension(Uploadlogo.FileName);
                    try
                    {
                        string logofile = User.Identity.GetUserId() + ext;
                        var path = System.IO.Path.Combine(Server.MapPath("~/Logo"), logofile);
                        Uploadlogo.SaveAs(path);
                        model.CompanyLogo = logofile;
                        model.UserID = User.Identity.GetUserId();
                        try
                        {
                            model.SaveProfile(userID);
                            pid = model.Pid;
                            TempData["id"] = pid;
                        }
                        catch (M_CustomException ex)
                        {
                            ex.LogException();
                            return RedirectToAction("Index");
                        }

                    }
                    catch (M_CustomException ex)
                    {
                        if (ex.ErrorCode == 100)
                        {
                            ModelState.AddModelError("error", ex.message);
                            return RedirectToAction("Creation");
                        }
                        else if (ex.ErrorCode == 101)
                        {
                            ModelState.AddModelError("Error", "logical exception");
                            return RedirectToAction("Creation");
                        }
                    }
                }
            }
            return RedirectToAction("ShowProfile", pid);
        }
        [HttpGet]
        public ActionResult ShowProfile()
        {
            userID = GetUser();
            int id = UsersProfile.findProfile(userID);
            M_Profile model = new M_Profile();
            if (id != 0)
            {
                model = M_Profile.ViewProfile(id);
                
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            M_Profile model = new M_Profile();
            model= M_Profile.Edit(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase Uploadlogo, M_Profile model)
        {
            if (Uploadlogo != null && Uploadlogo.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(Uploadlogo.FileName);
                string ext = System.IO.Path.GetExtension(Uploadlogo.FileName);
                string logofile = User.Identity.GetUserId() + ext;
                var path = System.IO.Path.Combine(Server.MapPath("~/Logo"), logofile);
                Uploadlogo.SaveAs(path);
                model.CompanyLogo = logofile;
                
                try
                {
                    model.UpdateProfile();
                    TempData["id"] = model.Pid;
                }
                catch (Exception)
                {

                    throw;
                }
            }

            else {
                try
                {
                    model.UpdateProfile();
                    TempData["id"] = model.Pid;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return RedirectToAction("ShowProfile", model.Pid);
        }

    }
}