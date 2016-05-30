using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class UsersCampaign
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Users")]
        public string UsersID { get; set; }

        [ForeignKey("M_Campaign")]
        public Nullable<int> CampaignID { get; set; }

        public virtual M_Campaigns M_Campaign { get; set; }

        public virtual ApplicationUser Users { get; set; }
    }
}