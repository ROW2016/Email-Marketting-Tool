using EMT_WebApp.Helpers;
using EMT_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace EMT_WebApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class M_Campaigns
    {
        public M_Campaigns()
        {
            this.M_UsersListCampaign = new HashSet<M_UsersListCampaign>();
        }
        [Key]
        public int? Cid { get; set; }
        public string Name { get; set; }

        [ForeignKey("M_CampTypes")]
        public int CTypeId { get; set; }

        [ForeignKey("NewList")]
        public int ListId { get; set; }
        public string EmailSubject { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string EmailContent { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; }

        public int? ScheduleCampaign { get; set; }

        public int? noOfEmailSendPerInterval { get; set; }

        public virtual S_CampaignTypes M_CampTypes { get; set; }
        public virtual M_List NewList { get; set; }
        public virtual S_Status S_Status { get; set; }
        public virtual ICollection<M_UsersListCampaign> M_UsersListCampaign { get; set; }

        static ApplicationDbContext dbcontext;
        static M_CustomException obj;
        static List<int?> listIDs;
        static List<int?> subscribersID;
        List<string> campaignNames;
        List<int?> campIds;
        static M_Campaigns campaign;
        static List<M_Campaigns> campaigns = null;
        public static string GetURL()
        {
            HttpRequest request = HttpContext.Current.Request;
            string url = request.Url.ToString();
            return url;
        }
        /// <summary>
        /// Saves campaign to database
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool SaveCampaign(string userID)
        {
            UsersCampaign user = new UsersCampaign();

            try
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    if (!CheckCampaignExist(this.Name, userID,dbcontext))
                    {
                        using (var trans = dbcontext.Database.BeginTransaction())
                        {
                            try
                            {
                                dbcontext.M_Campaigns.Add(this);

                                dbcontext.SaveChanges();

                                user.CampaignID = this.Cid;
                                user.UsersID = userID;
                                dbcontext.UsersCampaigns.Add(user);
                                dbcontext.SaveChanges();
                                trans.Commit();
                                return true;
                            }
                            catch (SqlException ex)
                            {
                                trans.Rollback();
                                obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), userID, Utlities.GetURL(), ex.LineNumber);

                                obj.LogException();
                                throw obj;
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), userID, Utlities.GetURL());

                                obj.LogException();
                                throw obj;
                            }
                        }
                    }//
                    else {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), userID, Utlities.GetURL());

                obj.LogException();
                throw obj;
            }
        }
          
        
    /// <summary>
    /// Checks if campaign name already created by perticular user
    /// </summary>
    /// <param name="campaignName"></param>
    /// <param name="userID"></param>
    /// <returns></returns>
    public bool CheckCampaignExist(string campaignName, string userID,ApplicationDbContext dbcontext)
    {
        listIDs = new List<int?>();
        bool res = false;
        // listIDs = GetCampaignIDs(userID);
        campaignNames = new List<string>();
        //using (dbcontext = new ApplicationDbContext())
        //{
            try
            {

                var campaigndata = from camp in dbcontext.M_Campaigns
                                   join user in dbcontext.UsersCampaigns
                                   on camp.Cid equals user.CampaignID
                                   where user.UsersID == userID
                                   select new { camp.Name };

                //where user.UsersID == userID);
                //select camp.Name.Any();
                if (campaigndata != null)
                {
                    res = campaigndata.Any(c => c.Name == campaignName);
                    // res = true;
                }
                //foreach (var item in listIDs)
                //{
                //    campaignNames.Add(dbcontext.M_Campaigns.Where(c => c.Cid == item).Select(c => c.Name).FirstOrDefault());
                //}
                //res= campaignNames.Any(c => c == campaignName);
                return res;
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
       // }

    }
    /// <summary>
    /// Get all campaign ids for perticular user
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public List<int?> GetCampaignIDs(string userID)
    {
        campIds = new List<int?>();
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                campIds = dbcontext.UsersCampaigns.Where(u => u.UsersID == userID).Select(c => c.CampaignID).ToList();
                return campIds;
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }
    }
    /// <summary>
    /// Get list of campaigns created by perticular user
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public static List<M_Campaigns> ViewCampaigns(string userID)
    {
        campaigns = new List<M_Campaigns>();
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                campaigns = (from camp in dbcontext.M_Campaigns
                             join user in dbcontext.UsersCampaigns
                             on camp.Cid equals user.CampaignID
                             where user.UsersID == userID
                             select camp).ToList();
                return campaigns;

            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, "Some problem occured while processing request", ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }
    }
    /// <summary>
    /// Serch for campaign by campaign ID
    /// </summary>
    /// <param name="cid"></param>
    /// <returns>Complete campaign information</returns>
    public static M_Campaigns FindCampaign(int? cid)
    {
        campaign = new M_Campaigns();
        try
        {
            using (dbcontext = new ApplicationDbContext())
            {
                campaign = dbcontext.M_Campaigns.Find(cid);
                //campaign = (from c in dbcontext.M_Campaigns
                //                             where c.Cid == cid
                //                             select new { c.Cid, c.CTypeId, c.EmailSubject, c.FromName, c.FromEmail, c.Name, c.ListId }).Single<M_Campaigns>();


                return campaign;
            }
        }
        catch (SqlException ex)
        {
            obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
            obj.LogException();
            throw obj;
        }
        catch (Exception ex)
        {
            obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
            obj.LogException();
            throw obj;
        }
    }
    /// <summary>
    /// gets list of subscribers for prerticular campaign
    /// </summary>
    /// <param name="Lid"></param>
    /// <returns>List of subscribers</returns>
    public static List<M_Subscriber> subscribersToCampaign(int? Lid)
    {
        List<M_Subscriber> Subscribers = new List<M_Subscriber>();
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                Subscribers = dbcontext.M_Lists.Find(Lid).Subscribers.ToList();
                return Subscribers;
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }
    }
    /// <summary>
    /// Update campaign information to database
    /// </summary>
    /// <param name="model"></param>
    public static void UpdateCampaign(UpdateCampModel model)
    {
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                dbcontext.Entry(model.Campaigns).State = System.Data.Entity.EntityState.Modified;
                dbcontext.SaveChanges();
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }
    }
    /// <summary>
    /// get email template from database
    /// </summary>
    /// <param name="cid"></param>
    /// <returns>Email content</returns>
    public static string EditTemplate(int? cid)
    {
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                // campaign = new M_Campaigns();
                string emailContent = null;
                emailContent = dbcontext.M_Campaigns.Where(c => c.Cid == cid).Select(e => e.EmailContent).FirstOrDefault();
                return emailContent;
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, "Some problem occured while processing request", ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }
    }
    /// <summary>
    /// saves updated email template to database
    /// </summary>
    /// <param name="EmailContent"></param>
    /// <param name="cid"></param>
    public static void UpdateTemplate(string EmailContent, int? cid)
    {
        campaign = new M_Campaigns();
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                campaign = dbcontext.M_Campaigns.SingleOrDefault(c => c.Cid == cid);
                if (campaign != null)
                {
                    campaign.EmailContent = WebUtility.HtmlEncode(EmailContent);
                    dbcontext.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }

    }
    /// <summary>
    /// Deactivate campaign by changing status id
    /// </summary>
    /// <param name="cid"></param>
    public static void DisableCampaign(int? cid)
    {
        campaign = new M_Campaigns();
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                campaign = dbcontext.M_Campaigns.Find(cid);
                campaign.IsActive = false;
                dbcontext.SaveChanges();
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }
    }
    /// <summary>
    /// enables campaign by hanging status id
    /// </summary>
    /// <param name="cid"></param>
    public static void EnableCampaign(int? cid)
    {
        campaign = new M_Campaigns();
        using (dbcontext = new ApplicationDbContext())
        {
            try
            {
                campaign = dbcontext.M_Campaigns.Find(cid);
                campaign.IsActive = true;
                dbcontext.SaveChanges();
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
            catch (Exception ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                obj.LogException();
                throw obj;
            }
        }
    }
        /// <summary>
        /// gets subscribers IDs for prticular campaign by campaignid
        /// </summary>
        /// <param name="campaignID"></param>
        public static List<int?> GetSubscribersForCampaign(int? campaignID)
        {
            subscribersID = new List<int?>();
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    int lid = dbcontext.M_Campaigns.Where(c => c.Cid == campaignID).Select(c => c.ListId).FirstOrDefault();
                    subscribersID = dbcontext.ListSusbscribers.Where(l => l.ListID == lid).Select(l => l.SubscribersID).ToList();
                    return subscribersID;
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                    obj.LogException();
                    throw obj;
                }

            }
        }
}
}