using EMT_WebApp.Helpers;
using EMT_WebApp.Models;
using EMT_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMT_WebApp.Controllers
{
    /// <summary>
    /// Contoller is responsible for routing actiontions related to campaign
    /// </summary>
    [Authorize(Roles ="User")]
    public class CampaignController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        UsersCampaign user = new UsersCampaign();
        List<int?> cid = new List<int?>();
        List<string> CampNames = new List<string>();
        static string userID = null;
        MailerQueue que = null;
        M_Tracking track = null;
        // static List<M_Campaigns> campaigns = null;
        /// <summary>
        /// gets the userid of current logged in user
        /// </summary>
        /// <returns>userid</returns>
        public string GetUser()
        {
            userID = User.Identity.GetUserId();
            //userID = "5387c8c4-ffc3-4b30-8712-efa591b4a062";
            return userID;
        }
        // GET: Campaign

        /// <summary>
        /// action will get all the campaigns created by current user
        /// </summary>
        /// <returns>list of campaigns</returns>
        public ActionResult Campaign()
        {
            userID = GetUser();
            CampaignViewModel model = new CampaignViewModel();
            try
            {
                model.Campaigns = M_Campaigns.ViewCampaigns(userID);
            }
            catch (M_CustomException ex)
            {
                ex.LogException();
                ModelState.AddModelError("viewcamp", "problem whilw showing campaigns");
                return View();
            }
            return View(model);

        }
        /// <summary>
        /// action is responsible to get view for campaign creation
        /// </summary>
        /// <returns></returns>
        public ActionResult Creation()
        {
            userID = GetUser();
            try
            {
                ViewBag.campTypes = new SelectList(S_CampaignTypes.GetCampTypes(), "CTId", "Name");
                ViewBag.List = new SelectList(M_List.GetLists(userID), "ListID", "ListName");
            }
            catch (M_CustomException ex)
            {
                ex.LogException();
                ModelState.AddModelError("viewcamp", "problem while showing campaigns");
                return View("Error");
            }

            return View();
        }
        /// <summary>
        /// action will get view for HTML designer for campaign
        /// </summary>
        /// <returns></returns>
        public ActionResult Designer()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
        /// <summary>
        /// display thank you view on susccesfully running campaign
        /// </summary>
        /// <returns></returns>
        public ActionResult Thanks()
        {
            return View();
        }
        /// <summary>
        /// gets update design view
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateDesign()
        {
            return View();
        }
        /// <summary>
        /// saves information in creation form after post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult registerCampaign(Models.M_Campaigns model)
        {
            TempData["CampInfo"] = model;

            return View("Designer", model);

        }
        /// <summary>
        /// save campaign information with tempate to database
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public ActionResult saveCampInfo(string UserName)
        {
            bool result = false;
            user.UsersID = GetUser();
            M_Campaigns model = (M_Campaigns)TempData["CampInfo"];
            if (ModelState.IsValid)
            {
                try
                {
                    var emailContent = WebUtility.HtmlEncode(UserName);
                    model.EmailContent = emailContent;
                    model.StatusId = 1;
                    model.IsActive = true;
                    result = model.SaveCampaign(userID);
                    if (result == true)
                    {
                        return RedirectToAction("Campaign");
                    }
                    else {
                        ModelState.AddModelError("nameExists", "Campaign name already exist");
                        return RedirectToAction("Creation");
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
            return RedirectToAction("Creation");
        }
        /// <summary>
        /// gets view with values to edit perticular campaign
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult editCampaign(int? id)
        {
            userID = GetUser();
            UpdateCampModel campaign = new UpdateCampModel();
            try
            {
                // campaign.Campaigns = dbContext.M_Campaigns.Find(id);
                campaign.Campaigns = M_Campaigns.FindCampaign(id);
                //campaign.Subscribers = dbContext.NewLists.Find(campaign.Campaigns.ListId).Subscribers.ToList();
                campaign.Subscribers = M_Campaigns.subscribersToCampaign(campaign.Campaigns.ListId);
                //WebUtility.HtmlDecode(campaign.Campaigns.EmailContent);
                ViewBag.campTypes = new SelectList(S_CampaignTypes.GetCampTypes(), "CTId", "Name");
                ViewBag.List = new SelectList(M_List.GetLists(userID), "ListID", "ListName");
                TempData["emailContent"] = campaign.Campaigns.EmailContent;
                return View("UpdateCamp", campaign);
            }
            catch (M_CustomException ex)
            {
                if (ex.ErrorCode == 100)
                {
                    ModelState.AddModelError("error", ex.message);
                    return RedirectToAction("Campaign");
                }
                else if (ex.ErrorCode == 101)
                {
                    ModelState.AddModelError("Error", "logical exception");
                    return RedirectToAction("Campaign");
                }
            }
            catch (InvalidOperationException ex)
            {
                M_CustomException obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                ModelState.AddModelError("Error", "Invalid operation");
                return RedirectToAction("Campaign");
            }

            return RedirectToAction("Campaign");
        }
        /// <summary>
        /// updates chages made in campaign after editing
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult editCampaign(UpdateCampModel model)
        {
            if (ModelState.IsValid)
            {
                model.Campaigns.StatusId = 1;
                model.Campaigns.IsActive = true;
                model.Campaigns.EmailContent = TempData["emailContent"].ToString();
                // model.Campaigns.EmailContent=WebUtility.HtmlEncode(model.Campaigns.EmailContent);

                try
                {
                    //dbContext.Entry(model.Campaigns).State = System.Data.Entity.EntityState.Modified;
                    //dbContext.SaveChanges();
                    M_Campaigns.UpdateCampaign(model);
                    return RedirectToAction("Campaign");
                }
                catch (M_CustomException ex)
                {
                    if (ex.ErrorCode == 100)
                    {
                        ModelState.AddModelError("error", ex.message);
                        return RedirectToAction("Campaign");
                    }
                    else if (ex.ErrorCode == 101)
                    {
                        ModelState.AddModelError("Error", "logical exception");
                        return RedirectToAction("Campaign");
                    }
                }
            }
            return RedirectToAction("Campaign");
        }

        /// <summary>
        /// get edit designer for perticular campaign to edit template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult editDesigner(int? id)
        {
            M_Campaigns cmodel = new M_Campaigns();
            TempData["CID"] = id;
            //String EmailContent = dbContext.M_Campaigns.Where(c => c.Cid == id).Select(e => e.EmailContent).FirstOrDefault();
            try
            {
                cmodel.EmailContent = M_Campaigns.EditTemplate(id);
                return View("UpdateDesign", cmodel);
            }
            catch (M_CustomException ex)
            {
                if (ex.ErrorCode == 100)
                {
                    ModelState.AddModelError("error", ex.message);
                    return RedirectToAction("Campaign");
                }
                else if (ex.ErrorCode == 101)
                {
                    ModelState.AddModelError("Error", "logical exception");
                    return RedirectToAction("Campaign");
                }
            }
            return RedirectToAction("Campaign");
        }
        /// <summary>
        /// update HTML template for perticular campaign
        /// </summary>
        /// <param name="EmailContent"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult editDesigner(string EmailContent)
        {
            M_Campaigns model = new M_Campaigns();
            int id = Convert.ToInt32(TempData["CID"]);
            // model = dbContext.M_Campaigns.SingleOrDefault(c => c.Cid == id);
            try
            {
                M_Campaigns.UpdateTemplate(EmailContent, id);
                //if (model != null)
                //{
                //    model.EmailContent = WebUtility.HtmlEncode(EmailContent);
                //    dbContext.SaveChanges();
                //}
                return RedirectToAction("Index", "Home", null);
            }
            catch (M_CustomException ex)
            {
                if (ex.ErrorCode == 100)
                {
                    ModelState.AddModelError("error", ex.message);
                    return RedirectToAction("Campaign");
                }
                else if (ex.ErrorCode == 101)
                {
                    ModelState.AddModelError("Error", "logical exception");
                    return RedirectToAction("Campaign");
                }

            }
            return RedirectToAction("Campaign");

        }
        /// <summary>
        /// disable perticular campaign
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCamp(int? id)
        {
            if (id != null)
            {
                //M_Campaigns model = new M_Campaigns();
                //model = dbContext.M_Campaigns.Find(id);
                //model.IsActive = false;
                //dbContext.SaveChanges();
                try
                {
                    M_Campaigns.DisableCampaign(id);
                }
                catch (M_CustomException ex)
                {
                    if (ex.ErrorCode == 100)
                    {
                        ModelState.AddModelError("error", ex.message);
                        return RedirectToAction("Campaign");
                    }
                    else if (ex.ErrorCode == 101)
                    {
                        ModelState.AddModelError("Error", "logical exception");
                        return RedirectToAction("Campaign");
                    }
                }
            }
            return RedirectToAction("Campaign"); ;
        }
        /// <summary>
        /// change status of campaign enable to disable or disable to enable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditStatus(int? id)
        {
            if (id != null)
            {
                //M_Campaigns model = new M_Campaigns();
                //model = dbContext.M_Campaigns.Find(id);
                //model.IsActive = true;
                //dbContext.SaveChanges();
                try
                {
                    M_Campaigns.EnableCampaign(id);
                }
                catch (M_CustomException ex)
                {
                    if (ex.ErrorCode == 100)
                    {
                        ModelState.AddModelError("error", ex.message);
                        return RedirectToAction("Campaign");
                    }
                    else if (ex.ErrorCode == 101)
                    {
                        ModelState.AddModelError("Error", "logical exception");
                        return RedirectToAction("Campaign");
                    }
                }
            }
            return RedirectToAction("Campaign");
        }


        /// <summary>
        /// when user clecks on send campaign link it add the campaign to queue to be used by windows service 
        /// to send email
        /// </summary>
        /// <param name="campaignID"></param>
        /// <returns></returns>
        public async Task<ActionResult> AddToQueue(int campaignID)
        {
            userID = GetUser();
            que = new MailerQueue();
            que.CampaignID = campaignID;
            try
            {
                //que.IdentifierCampaign= M_Tracking.AddSubscribersToTracking(campaignID, userID);
                //await Task.Run(() =>
                //{
                //    que.IdentifierCampaign = M_Tracking.AddSubscribersToTrackingasync(campaignID, userID);
                //});
                que.IdentifierCampaign = await M_Tracking.AddSubscribersToTrackingasync(campaignID, userID);
                MailerQueue.EnqueueCampaign(que);
            }
          
            catch (M_CustomException ex)
            {
                if (ex.ErrorCode == 100)
                {
                    ModelState.AddModelError("error", ex.message);
                    return RedirectToAction("Campaign");
                }
                else if (ex.ErrorCode == 101)
                {
                    ModelState.AddModelError("Error", "logical exception");
                    return RedirectToAction("Campaign");
                }
                else if (ex.ErrorCode == 102)
                {
                    ModelState.AddModelError("Error", "Argument exception");
                    return RedirectToAction("Campaign");
                }
            }

            return RedirectToAction("Thanks");
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}