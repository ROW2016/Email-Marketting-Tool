using EMT_WebApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class MailerQueue
    {
        [Key]
        public int MqId { get; set; }

        [ForeignKey("Campaigns")]
        public int CampaignID { get; set; }

        public string IdentifierCampaign { get; set; }
        public virtual M_Campaigns Campaigns { get; set; }

        static ApplicationDbContext dbcontext;
        //static MailerQueue queue;
        static M_CustomException obj;
        /// <summary>
        /// add new campaign to database queue
        /// </summary>
        /// <param name="campaignId"></param>
        public static void EnqueueCampaign(MailerQueue que)
        {
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    dbcontext.MailerQueues.Add(que);
                    dbcontext.SaveChanges();
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                    obj.LogException();
                    throw obj;
                }

            }
        }
    }
}