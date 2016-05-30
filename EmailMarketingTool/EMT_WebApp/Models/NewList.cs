using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class NewList
    {
        public NewList()
        {
            this.M_Campaigns = new HashSet<M_Campaigns>();
            this.Subscribers = new HashSet<Subscriber>();
            this.M_UsersListCampaign = new HashSet<M_UsersListCampaign>();
            this.ListSusbscribers = new HashSet<ListSusbscriber>();
        }
        [Key]
        public int ListID { get; set; }
        public string ListName { get; set; }
        public string DefaultFromEmail { get; set; }
        public string DefaultFromName { get; set; }
        public string CompanyorOrganization { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public string ExcelCSVFilePath { get; set; }
        public string PostalCode { get; set; }

        public virtual Country Country { get; set; }
       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<M_Campaigns> M_Campaigns { get; set; }
      //  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subscriber> Subscribers { get; set; }
       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<M_UsersListCampaign> M_UsersListCampaign { get; set; }
      //  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListSusbscriber> ListSusbscribers { get; set; }
    }
}