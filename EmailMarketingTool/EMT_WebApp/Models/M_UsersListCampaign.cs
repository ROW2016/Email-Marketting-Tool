using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class M_UsersListCampaign
    {
        [Key]
        public int ID { get; set; }
        public string UsersID { get; set; }
        public Nullable<int> ListID { get; set; }
        public Nullable<int> CampaignID { get; set; }

        public virtual M_Campaigns M_Campaigns { get; set; }
        public virtual M_List NewList { get; set; }

        public virtual ApplicationUser Users { get; set; }
                                           // public virtual User User { get; set; }
    }
}