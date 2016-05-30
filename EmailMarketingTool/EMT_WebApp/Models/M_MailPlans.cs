using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class M_MailPlans
    {
        [Key]
        public int PlanId { get; set; }
        public string PlanName { get; set; }

        public int MailsAlloted { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}