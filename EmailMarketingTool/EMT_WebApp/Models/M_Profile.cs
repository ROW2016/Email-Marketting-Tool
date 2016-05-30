using EMT_WebApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class M_Profile
    {
        public M_Profile()
        {
            Pid = 0;
            CompanyName = null;
            CompanyLogo = null;
            Address = null;
            Domain = null;
        }
        [Key]
        public int Pid { get; set; }

        public string CompanyName { get; set; }

        public string CompanyLogo { get; set; }

        public string Address { get; set; }

        public string Domain { get; set; }

        [ForeignKey("Users")]
        public string UserID { get; set; }

        public virtual ApplicationUser Users { get; set; }

        public static string GetURL()
        {
            HttpRequest request = HttpContext.Current.Request;
            string url = request.Url.ToString();
            return url;
        }
        static ApplicationDbContext dbcontext;
        static M_CustomException obj;
        static M_Profile profile;
        /// <summary>
        /// saves organization profile to database
        /// </summary>
        public void SaveProfile(string userID)
        {
            UsersProfile domain = new UsersProfile();
            using (dbcontext = new ApplicationDbContext())
            {
                using (var trans = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        dbcontext.M_Profiles.Add(this);
                        dbcontext.SaveChanges();
                        domain.ProfileID = this.Pid;
                        domain.UserID = userID;
                        dbcontext.UsersProfile.Add(domain);
                        dbcontext.SaveChanges();
                        trans.Commit();
                    }
                    catch (SqlException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), GetURL(), ex.LineNumber);

                        obj.LogException();
                        trans.Rollback();
                        throw obj;
                    }
                    catch (Exception ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), GetURL());
                        obj.LogException();
                        trans.Rollback();
                        throw obj;
                    }
                }
            }
        }

        public static M_Profile Edit(int pid)
        {
            profile = new M_Profile();
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    profile = dbcontext.M_Profiles.Find(pid);
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                    obj.LogException();
                    throw obj;
                }
              
            }
            return profile;
        }

        public void UpdateProfile()
        {
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    dbcontext.Entry(this).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                    obj.LogException();
                    throw obj;
                }
            }
          
        }

        public static M_Profile ViewProfile(int? pid)
        {
            profile = new M_Profile();
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    profile = dbcontext.M_Profiles.Find(pid);
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL(), ex.LineNumber);
                    obj.LogException();
                    throw obj;
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                    obj.LogException();
                    throw obj;
                }
            }
                return profile;
        }
        
    }
}