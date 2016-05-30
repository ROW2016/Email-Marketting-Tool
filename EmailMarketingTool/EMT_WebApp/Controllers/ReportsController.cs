using EMT_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMT_WebApp.ViewModels;
using Microsoft.AspNet.Identity;

namespace EMT_WebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class ReportsController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        List<int?> cid = new List<int?>();
        public ActionResult Index()
        {
            string uid = User.Identity.GetUserId();
            ReportCamp capaignssubscribers = new ReportCamp();
            capaignssubscribers.campgn = new List<CampaignSubVM>();

            CampaignReportVM model = new CampaignReportVM();
            model.Campaigns = new List<M_Campaigns>();
            model.subscribers = new List<M_Subscriber>();
            model.Tracks = new List<M_Tracking>();
            
            cid = dbContext.UsersCampaigns.Where(u => u.UsersID == uid).Select(c => c.CampaignID).ToList();
            if (cid.Count != 0)
            {
                foreach (var item in cid)
                {
                    model.Campaigns.Add(dbContext.M_Campaigns.Where(u => u.Cid == item && u.IsActive==true).FirstOrDefault());
                    model.Tracks.Add(dbContext.M_Trackings.FirstOrDefault());
                }

            }

           

              //  model.Campaigns = dbContext.M_Campaigns.Where(c => c.IsActive == true).ToList();
               
                foreach (var item in model.Campaigns)
                {
                    CampaignSubVM obj = new CampaignSubVM();
                    obj.camp = item;
                    obj.noOdcanmsub = dbContext.M_Subscribers.Where(l => l.ListID == item.ListId && l.Unsubscribe == false).Count();
                    obj.NoOfUnsubscribe = dbContext.M_Subscribers.Where(l => l.ListID == item.ListId && l.Unsubscribe == true).Count();
                    obj.noOfOpenedEm = dbContext.M_Trackings.Where(c => c.CampId == item.Cid).Count();
                    capaignssubscribers.campgn.Add(obj);
                }

                return View(capaignssubscribers);
            }

        public ActionResult DetailedStat(int? id)
        {
            CampaignTrackVM cvm = new CampaignTrackVM();
            cvm.subscribers = new List<M_Subscriber>();
            cvm.trckDts = dbContext.M_Trackings.Where(s => s.CampId == id && s.IsOpened==true).ToList();

            CMTRCKDone cdcd = new CMTRCKDone();
            cdcd.subsciberTrks = new List<CMTRCK>();


            CMTRCK indsub;
            foreach (var item in cvm.trckDts)
            {
                indsub = new CMTRCK();
                var subb = dbContext.M_Subscribers.Where(s => s.SubscriberID == item.SubsciberId).First();
                indsub.subscriber = subb;
                indsub.openedDateNTime = (DateTime)item.DateOpened;
                cdcd.subsciberTrks.Add(indsub);   
            }

            return View("TrackSubscriber", cdcd);
        }


        public ActionResult downloadOpenedMailUsers(int? cid)
        {
            CampaignTrackVM cvm = new CampaignTrackVM();
            cvm.subscribers = new List<M_Subscriber>();
            cvm.trckDts = dbContext.M_Trackings.Where(s => s.CampId == cid).ToList();

            CMTRCKDone cdcd = new CMTRCKDone();
            cdcd.subsciberTrks = new List<CMTRCK>();


            CMTRCK indsub;
            foreach (var item in cvm.trckDts)
            {
                indsub = new CMTRCK();
                var subb = dbContext.M_Subscribers.Where(s => s.SubscriberID == item.SubsciberId).First();
                indsub.subscriber = subb;
             //   indsub.openedDateNTime = (DateTime)item.IsOpened;
                cdcd.subsciberTrks.Add(indsub);
            }

            return View("TrackSubscriber", cdcd);
        }
    }
}