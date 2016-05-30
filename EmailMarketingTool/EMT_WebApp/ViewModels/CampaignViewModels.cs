using EMT_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMT_WebApp.ViewModels
{
    public class CampaignViewModel
    {
        public List<M_Campaigns> Campaigns { get; set; }
        public CampaignViewModel()
        {
            Campaigns = new List<M_Campaigns>();
        }
    }

    public class CampaignEditVM
    {
        public string SelectedValue { get; set; }
        public IEnumerable<SelectListItem> Campaigns { get; set; }
       
        // public IEnumerable<SelectListItem> List { get; set; }
    }

    public class UpdateCampModel
    {
        public M_Campaigns Campaigns { get; set; }

        public List<M_Subscriber> Subscribers { get; set; }

        public UpdateCampModel()
        {
            Subscribers = new List<M_Subscriber>();
        }
    }
}