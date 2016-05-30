using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class S_Status
    {
        public S_Status()
        {
            this.M_Campaigns = new HashSet<M_Campaigns>();
        }
        [Key]
        public int Statid { get; set; }
        public string Name { get; set; }

      //  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<M_Campaigns> M_Campaigns { get; set; }
    }
}