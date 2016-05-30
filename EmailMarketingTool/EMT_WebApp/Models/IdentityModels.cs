using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using EMT_WebApp.Models;

namespace EMT_WebApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("UltimateEMT", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<S_Country> S_Countries { get; set; }

        public DbSet<M_Campaigns> M_Campaigns { get; set; }

        public DbSet<S_CampaignTypes> S_CampaignTypes { get; set; }

        public DbSet<M_List> M_Lists { get; set; }

        public DbSet<S_Status> S_Status { get; set; }

        public DbSet<M_Subscriber> M_Subscribers { get; set; }

        public DbSet<ListSusbscriber> ListSusbscribers { get; set; }

        public DbSet<UsersCampaign> UsersCampaigns { get; set; }

        public DbSet<UsersList> UsersList { get; set; }

        public DbSet<M_Tracking> M_Trackings { get; set; }

        public DbSet<MailerQueue> MailerQueues { get; set; }

        public DbSet<M_CustomException> M_CustomExceptions { get; set; }
        public DbSet<M_MailStatus> M_MailStatus { get; set; }

        public DbSet<M_Profile> M_Profiles { get; set; }

        public DbSet<EMT_WebApp.Models.M_MailPlans> M_MailPlans { get; set; }

        public DbSet<M_Subscription> M_Subscription { get; set; }

        public DbSet<UsersProfile> UsersProfile { get; set; }

    }

  
}