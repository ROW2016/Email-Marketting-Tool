using EMT_WebApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EMT_WebApp.Models
{
    public class M_Tracking
    {
        [Key]
        public int TrackID { get; set; }

        [ForeignKey("M_Campaigns")]
        public int? CampId { get; set; }

        [ForeignKey("Subscriber")]
        public int? SubsciberId { get; set; }
        public bool IsOpened { get; set; }

        public bool MailStatus { get; set; }

        public DateTime? DateExecuted { get; set; }

        public DateTime? DateOpened { get; set; }

        //[Index("ISIndex", IsUnique = true)]
        //[StringLength(450)]
        public string IdentifierSubscriber { get; set; }

        //[Index("ICIndex", IsUnique = true)]
        //[StringLength(450)]
        public string IdentifierCampaign { get; set; }

        [ForeignKey("Users")]
        public string UserID { get; set; }

        public virtual ApplicationUser Users { get; set; }
        public virtual M_Campaigns M_Campaigns { get; set; }
        public virtual M_Subscriber Subscriber { get; set; }

        static ApplicationDbContext dbcontext;
        static List<int?> subIDs;
        static M_Tracking track;
        static M_CustomException obj;

        public static string AddSubscribersToTracking(int cid,string userID)
        {
            subIDs = M_Campaigns.GetSubscribersForCampaign(cid);
            string id = Guid.NewGuid().ToString();
            if (subIDs.Count != 0)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    try
                    {
                        foreach (var item in subIDs)
                        {
                            
                                track = new M_Tracking();
                                track.CampId = cid;
                                track.SubsciberId = item;
                                track.IsOpened = false;
                                track.IdentifierSubscriber = Guid.NewGuid().ToString();
                                track.IdentifierCampaign = id;
                                track.MailStatus = false;
                                track.UserID = userID;
                                dbcontext.M_Trackings.Add(track);
                                dbcontext.SaveChanges();

                        }
                    }
                    catch (SqlException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                        obj.LogException();
                        throw obj;
                    }
                    catch (InvalidOperationException ex) {
                        obj = new M_CustomException((int)ErorrTypes.InvalidOperation, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                        obj.LogException();
                        throw obj;
                    }
                    catch (ArgumentNullException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.ArgumentNullExceptions, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
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
            return track.IdentifierCampaign;

        }

        //Async method
        public static  Task<string> AddSubscribersToTrackingasync(int cid, string userID)
        {
            subIDs = M_Campaigns.GetSubscribersForCampaign(cid);
            string id = Guid.NewGuid().ToString();
            if (subIDs.Count != 0)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    try
                    {
                        foreach (var item in subIDs)
                        {

                                track = new M_Tracking();
                                track.CampId = cid;
                                track.SubsciberId = item;
                                track.IsOpened = false;
                                track.IdentifierSubscriber = Guid.NewGuid().ToString();
                                track.IdentifierCampaign = id;
                                track.MailStatus = false;
                                track.UserID = userID;
                                dbcontext.M_Trackings.Add(track);
                                dbcontext.SaveChanges();
                        }
                    }
                    catch (SqlException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                        obj.LogException();
                        throw obj;
                    }
                    catch (InvalidOperationException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.InvalidOperation, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
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

            // return track.IdentifierCampaign;
       return Task.Delay(1000).ContinueWith(t => track.IdentifierCampaign);

        }
    }
}