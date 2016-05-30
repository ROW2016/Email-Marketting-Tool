using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class M_Subscription
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string  UserId { get; set; }

        [ForeignKey("Plans")]
        public int PlanId { get; set; }
        
        public int MailCount { get; set; }


        public virtual M_MailPlans Plans { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}