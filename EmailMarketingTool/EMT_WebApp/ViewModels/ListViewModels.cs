using EMT_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PagedList;

namespace EMT_WebApp.ViewModels
{
    public class ListViewModel
    {
        public List<int> NoOfSubscribers { get; set; }
        public List<M_List> lists { get; set; }

        public ListViewModel()
        {
            NoOfSubscribers = new List<int>();
            lists = new List<M_List>();
        }
    }

    public class SubscribersViewModel
    {
        public int? ListID { get; set; }
        public List<M_Subscriber> SubscribersToList { get; set; }

        public DataTable dataTable { get; set; }

        public SubscribersViewModel()
        {
            SubscribersToList = new List<M_Subscriber>();
        }
    }
}