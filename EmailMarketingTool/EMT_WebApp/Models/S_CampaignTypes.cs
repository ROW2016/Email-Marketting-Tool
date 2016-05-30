using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class S_CampaignTypes
    {
        public static string GetURL()
        {
            HttpRequest request = HttpContext.Current.Request;
            string url = request.Url.ToString();
            return url;
        }
        public S_CampaignTypes()
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

        static ApplicationDbContext dbcontext = null;
        static List<S_CampaignTypes> campTypes = null;
        static M_CustomException obj = null;
        /// <summary>
        /// Gets list of campaign types from database
        /// </summary>
        /// <returns></returns>
        public static List<S_CampaignTypes> GetCampTypes()
        {
            campTypes = new List<S_CampaignTypes>();
            using (dbcontext = new ApplicationDbContext())
            {

                try
                {
                    campTypes = dbcontext.S_CampaignTypes.ToList();
                    return campTypes;
                }
                catch (SqlException ex)
                {

                    M_CustomException obj = new M_CustomException((int)ErorrTypes.SqlExceptions, "Problem in saving data", ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), GetURL(), ex.LineNumber);

                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, "Some problem occured while processing request", ex.StackTrace, ErorrTypes.others.ToString(), GetURL());

                    obj.LogException();
                    throw obj;
                }
            }
        }
    }
}