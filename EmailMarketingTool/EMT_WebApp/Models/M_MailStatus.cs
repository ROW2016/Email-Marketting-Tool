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
    public class M_MailStatus
    {
        [Key]
        public int StatusId { get; set; }

        [ForeignKey("Users")]
        public string UserId { get; set; }

        [ForeignKey("Campaigns")]
        public int? CampId { get; set; }

        [ForeignKey("List")]
        public int? ListId { get; set; }

        [ForeignKey("Subscriber")]
        public int? SubscriberId { get; set; }

        [ForeignKey("Status")]
        public int? S_StatusId { get; set; }


        /// <summary>
        /// Navigation Properties
        /// </summary>
        public virtual M_Campaigns Campaigns { get; set; }
        public virtual M_List List { get; set; }
        public virtual M_Subscriber Subscriber { get; set; }
        public virtual S_Status Status { get; set; }
        public virtual ApplicationUser Users { get; set; }


        static ApplicationDbContext dbContext;
        static M_CustomException obj;

      
        /// <summary>
        /// Save the subscriber with thier status , so that the Windows service can send mail to user
        /// </summary>
        /// <param name="usrId"></param>
        /// <param name="campId"></param>
        /// <param name="listId"></param>
        /// <param name="subId"></param>
        public void settingMail(string usrId, int campId, int listId, int subId)
        {
            dbContext = new ApplicationDbContext();
            this.UserId = usrId;
            this.CampId = campId;
            this.ListId = listId;
            this.StatusId = 4;
            this.SubscriberId = subId;
            try
            {
                dbContext.M_MailStatus.Add(this);
                dbContext.SaveChanges();
            }
            catch (SqlException ex)
            {               
                obj = new M_CustomException((int)ErorrTypes.SqlExceptions, "Problem in saving data", ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), UserId, Utlities.GetURL(), ex.LineNumber);
                obj.LogException();
                throw obj;
            }
        }

     
    }
}