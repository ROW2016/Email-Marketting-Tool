using EMT_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMT_WebApp.ViewModels
{
    public class CampaignReportVM
    {
      
        public List<M_Campaigns> Campaigns { get; set; }
        public List<M_Tracking> Tracks { get; set; }

        public List<M_Subscriber> subscribers { get; set; }
        public int NoOfSubscribers { get; set; }
        public int NoOfEmailOpened { get; set; }
        public string CampaignStatus { get; set; }

    }
    public class CampaignSubVM
        {
        
            public M_Campaigns camp { get; set; }
            public int noOdcanmsub { get; set; }

            public int noOfOpenedEm { get; set; }
        public int NoOfUnsubscribe { get; set; }

    }

        public class ReportCamp
        {
        
            public List<CampaignSubVM> campgn { get; set; }
        }

    public class CMTRCK
    {

        public M_Subscriber subscriber { get; set; }
        public string Name { get; set; }
        public string EmailID { get; set; }
        public DateTime openedDateNTime { get; set; }
    }
    public class CMTRCKDone
    {
        public List<CMTRCK> subsciberTrks { get; set; }
    }

    public class CampaignTrackVM
    {
        public List<M_Tracking> trckDts { get; set; }
        public List<M_Subscriber> subscribers { get; set; }
        public DateTime openedDateNTime { get; set; }
    }
}