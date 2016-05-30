using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ListSusbscriber
    {
        [Key]
        public int ListSubscribersID { get; set; }
        [ForeignKey("NewList")]
        public Nullable<int> ListID { get; set; }
        [ForeignKey("Subscriber")]
        public Nullable<int> SubscribersID { get; set; }

        public bool isSent { get; set; }
        public virtual M_List NewList { get; set; }
        public virtual M_Subscriber Subscriber { get; set; }
    }
}