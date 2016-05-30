using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class UsersProfile
    {
        [Key]
        public int UPID { get; set; }

        public int ProfileID { get; set; }

        [ForeignKey("Users")]
        public string UserID { get; set; }

        public virtual ApplicationUser Users { get; set; }

        static ApplicationDbContext dbcontext;

        public static int findProfile(string user)
        {
            int id = 0;
            using (dbcontext = new ApplicationDbContext())
            {
                id = dbcontext.UsersProfile.Where(u => u.UserID == user).Select(u => u.ProfileID).FirstOrDefault();

            }
            return id;
        }
    }
}