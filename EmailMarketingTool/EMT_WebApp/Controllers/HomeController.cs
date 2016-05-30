using EMT_WebApp.Models;
using EMT_WebApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
using EMT_WebApp.Helpers;
using System.Data.SqlClient;

namespace EMT_WebApp.Controllers
{
    /// <summary>
    /// controller is responsible for displaying content on dashboared after login for perticular user
    /// </summary>
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        List<M_Subscriber> Slists = new List<M_Subscriber>();
        List<int?> listIDs = new List<int?>();
        List<M_Campaigns> campaigns = new List<M_Campaigns>();
        int cnt, cnt1;
        M_CustomException obj;
        /// <summary>
        /// gets number of campaigns,lists,subscribers for perticular user to display on dashboared
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string uid = User.Identity.GetUserId();
            DashboardViewModel model = new DashboardViewModel();
            //listIDs = dbContext.UsersList.Where(u => u.UsersID == uid).Select(l => l.ListID).ToList();
            try
            {
                cnt = (from sub in dbContext.M_Subscribers
                       join users in dbContext.UsersList
                       on sub.ListID equals users.ListID
                       where (users.UsersID == uid && sub.Unsubscribe == false)
                       select sub).Count();

                cnt1 = (from sub in dbContext.M_Subscribers
                        join users in dbContext.UsersList
                        on sub.ListID equals users.ListID
                        where (users.UsersID == uid && sub.Unsubscribe == true)
                        select sub).Count();

            }
            catch (DataException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
            //foreach (var item in listIDs)
            //{

            //    cnt += dbContext.Subscribers.Where(l => l.ListID == item && l.Unsubscribe==false).Count();
            //    cnt1 += dbContext.Subscribers.Where(l => l.ListID == item && l.Unsubscribe == true).Count();
            //}
            model.NoOfLists = dbContext.UsersList.Where(u => u.UsersID == uid).Count();
            model.NoOfSubscribers = cnt;
            model.NoOfUnSubscribers = cnt1;
            model.NoOfCampaigns = dbContext.UsersCampaigns.Where(u => u.UsersID == uid).Count();
            return View(model);
            // return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}