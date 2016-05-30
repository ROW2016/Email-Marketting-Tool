using EMT_WebApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class S_Country
    {
        //public static string GetURL()
        //{
        //    HttpRequest request = HttpContext.Current.Request;
        //    string url = request.Url.ToString();
        //    return url;
        //}
        public S_Country()
        {
            this.NewLists = new HashSet<M_List>();
        }
        [Key]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public virtual ICollection<M_List> NewLists { get; set; }

        static ApplicationDbContext dbcontext = null;
        static List<S_Country> countries = new List<S_Country>();
        static M_CustomException obj = null;
        /// <summary>
        /// Get list of coutries from database
        /// </summary>
        /// <returns></returns>
        public static List<S_Country> GetCountries()
        {
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    countries = dbcontext.S_Countries.ToList();
                    return countries;
                }
                catch (SqlException ex)
                {

                    M_CustomException obj = new M_CustomException((int)ErorrTypes.SqlExceptions, "Problem in saving data", ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, "Some problem occured while processing request", ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());

                    obj.LogException();
                    throw obj;
                }
            }
        }

        public static void SaveCountry(S_Country country)
        {
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    dbcontext.S_Countries.Add(country);
                    dbcontext.SaveChanges();
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.SqlExceptions, "Problem in saving data", ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, "Some problem occured while processing request", ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                    obj.LogException();
                    throw obj;
                }

            }
        }
    }
}