using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class M_CampTypes
    {
        public M_CampTypes()
        {
            this.M_Campaigns = new HashSet<M_Campaigns>();
        }
        [Key]
        public int CTId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<M_Campaigns> M_Campaigns { get; set; }
    }
}