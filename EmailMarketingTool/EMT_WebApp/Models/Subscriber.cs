using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class Subscriber
    {
        public Subscriber()
        {
            this.ListSusbscribers = new HashSet<ListSusbscriber>();
        }

        [Key]
        public int SubscriberID { get; set; }
        public Nullable<int> ListID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AlternateEmailAddress { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<System.DateTime> LastChanged { get; set; }

        [DefaultValue(false)]
        public bool Unsubscribe { get; set; }

        public virtual NewList NewList { get; set; }
       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListSusbscriber> ListSusbscribers { get; set; }

       
    }
   
}
