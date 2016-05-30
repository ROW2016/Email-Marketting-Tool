using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class Country
    {
        public Country()
        {
            this.NewLists = new HashSet<NewList>();
        }
        [Key]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public virtual ICollection<NewList> NewLists { get; set; }
    }
}