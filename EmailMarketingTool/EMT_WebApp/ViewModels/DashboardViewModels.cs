using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMT_WebApp.ViewModels
{
    public class DashboardViewModel
    {
        public int NoOfLists { get; set; }
        public int NoOfSubscribers { get; set; }
        public int NoOfCampaigns { get; set; }
        public int NoOfUnSubscribers { get; set; }
    }
}