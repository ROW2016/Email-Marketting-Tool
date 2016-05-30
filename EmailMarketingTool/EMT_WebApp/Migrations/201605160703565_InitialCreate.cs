namespace EMT_WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ListSusbscribers",
                c => new
                    {
                        ListSubscribersID = c.Int(nullable: false, identity: true),
                        ListID = c.Int(),
                        SubscribersID = c.Int(),
                        isSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ListSubscribersID)
                .ForeignKey("dbo.M_List", t => t.ListID)
                .ForeignKey("dbo.M_Subscriber", t => t.SubscribersID)
                .Index(t => t.ListID)
                .Index(t => t.SubscribersID);
            
            CreateTable(
                "dbo.M_List",
                c => new
                    {
                        ListID = c.Int(nullable: false, identity: true),
                        ListName = c.String(),
                        DefaultFromEmail = c.String(),
                        DefaultFromName = c.String(),
                        CompanyorOrganization = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        CountryID = c.Int(),
                        City = c.String(),
                        PhoneNumber = c.String(),
                        CreatedDate = c.DateTime(),
                        LastUpdated = c.DateTime(),
                        ExcelCSVFilePath = c.String(),
                        PostalCode = c.String(),
                    })
                .PrimaryKey(t => t.ListID)
                .ForeignKey("dbo.S_Country", t => t.CountryID)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.S_Country",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        CountryName = c.String(),
                    })
                .PrimaryKey(t => t.CountryID);
            
            CreateTable(
                "dbo.M_Campaigns",
                c => new
                    {
                        Cid = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CTypeId = c.Int(nullable: false),
                        ListId = c.Int(nullable: false),
                        EmailSubject = c.String(),
                        FromName = c.String(),
                        FromEmail = c.String(),
                        EmailContent = c.String(),
                        StatusId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ScheduleCampaign = c.Int(),
                        noOfEmailSendPerInterval = c.Int(),
                        S_Status_Statid = c.Int(),
                    })
                .PrimaryKey(t => t.Cid)
                .ForeignKey("dbo.S_CampaignTypes", t => t.CTypeId)
                .ForeignKey("dbo.M_List", t => t.ListId)
                .ForeignKey("dbo.S_Status", t => t.S_Status_Statid)
                .Index(t => t.CTypeId)
                .Index(t => t.ListId)
                .Index(t => t.S_Status_Statid);
            
            CreateTable(
                "dbo.S_CampaignTypes",
                c => new
                    {
                        CTId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CTId);
            
            CreateTable(
                "dbo.M_UsersListCampaign",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UsersID = c.String(maxLength: 128),
                        ListID = c.Int(),
                        CampaignID = c.Int(),
                        M_Campaigns_Cid = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.M_Campaigns", t => t.M_Campaigns_Cid)
                .ForeignKey("dbo.M_List", t => t.ListID)
                .ForeignKey("dbo.AspNetUsers", t => t.UsersID)
                .Index(t => t.UsersID)
                .Index(t => t.ListID)
                .Index(t => t.M_Campaigns_Cid);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.S_Status",
                c => new
                    {
                        Statid = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Statid);
            
            CreateTable(
                "dbo.M_Subscriber",
                c => new
                    {
                        SubscriberID = c.Int(nullable: false, identity: true),
                        ListID = c.Int(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        EmailAddress = c.String(),
                        AlternateEmailAddress = c.String(),
                        Address = c.String(),
                        Country = c.String(),
                        City = c.String(),
                        AddedDate = c.DateTime(),
                        LastChanged = c.DateTime(),
                        Unsubscribe = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriberID)
                .ForeignKey("dbo.M_List", t => t.ListID)
                .Index(t => t.ListID);
            
            CreateTable(
                "dbo.M_CustomException",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ErrorCode = c.Int(nullable: false),
                        message = c.String(),
                        stackTrace = c.String(),
                        Type = c.String(),
                        time = c.DateTime(),
                        url = c.String(),
                        status = c.Boolean(nullable: false),
                        userId = c.String(maxLength: 128),
                        lineNumber = c.Int(nullable: false),
                        HelpLink = c.String(),
                        Source = c.String(),
                        HResult = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.userId)
                .Index(t => t.userId);
            
            CreateTable(
                "dbo.M_MailPlans",
                c => new
                    {
                        PlanId = c.Int(nullable: false, identity: true),
                        PlanName = c.String(),
                        MailsAlloted = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PlanId);
            
            CreateTable(
                "dbo.M_MailStatus",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        CampId = c.Int(),
                        ListId = c.Int(),
                        SubscriberId = c.Int(),
                        S_StatusId = c.Int(),
                    })
                .PrimaryKey(t => t.StatusId)
                .ForeignKey("dbo.M_Campaigns", t => t.CampId)
                .ForeignKey("dbo.M_List", t => t.ListId)
                .ForeignKey("dbo.S_Status", t => t.S_StatusId)
                .ForeignKey("dbo.M_Subscriber", t => t.SubscriberId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CampId)
                .Index(t => t.ListId)
                .Index(t => t.SubscriberId)
                .Index(t => t.S_StatusId);
            
            CreateTable(
                "dbo.M_Profile",
                c => new
                    {
                        Pid = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        CompanyLogo = c.String(),
                        Address = c.String(),
                        Domain = c.String(),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Pid)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.M_Subscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        PlanId = c.Int(nullable: false),
                        MailCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.M_MailPlans", t => t.PlanId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.PlanId);
            
            CreateTable(
                "dbo.M_Tracking",
                c => new
                    {
                        TrackID = c.Int(nullable: false, identity: true),
                        CampId = c.Int(),
                        SubsciberId = c.Int(),
                        //IsOpened = c.DateTime(),
                        MailStatus = c.Boolean(nullable: false),
                        DateExecuted = c.DateTime(),
                        DateOpened = c.DateTime(),
                        Identifier = c.String(),
                    })
                .PrimaryKey(t => t.TrackID)
                .ForeignKey("dbo.M_Campaigns", t => t.CampId)
                .ForeignKey("dbo.M_Subscriber", t => t.SubsciberId)
                .Index(t => t.CampId)
                .Index(t => t.SubsciberId);
            
            CreateTable(
                "dbo.MailerQueues",
                c => new
                    {
                        MqId = c.Int(nullable: false, identity: true),
                        CampaignID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MqId)
                .ForeignKey("dbo.M_Campaigns", t => t.CampaignID)
                .Index(t => t.CampaignID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UsersCampaigns",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UsersID = c.String(maxLength: 128),
                        CampaignID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.M_Campaigns", t => t.CampaignID)
                .ForeignKey("dbo.AspNetUsers", t => t.UsersID)
                .Index(t => t.UsersID)
                .Index(t => t.CampaignID);
            
            CreateTable(
                "dbo.UsersLists",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UsersID = c.String(maxLength: 128),
                        ListID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.M_List", t => t.ListID)
                .ForeignKey("dbo.AspNetUsers", t => t.UsersID)
                .Index(t => t.UsersID)
                .Index(t => t.ListID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersLists", "UsersID", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersLists", "ListID", "dbo.M_List");
            DropForeignKey("dbo.UsersCampaigns", "UsersID", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersCampaigns", "CampaignID", "dbo.M_Campaigns");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.MailerQueues", "CampaignID", "dbo.M_Campaigns");
            DropForeignKey("dbo.M_Tracking", "SubsciberId", "dbo.M_Subscriber");
            DropForeignKey("dbo.M_Tracking", "CampId", "dbo.M_Campaigns");
            DropForeignKey("dbo.M_Subscription", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.M_Subscription", "PlanId", "dbo.M_MailPlans");
            DropForeignKey("dbo.M_Profile", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.M_MailStatus", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.M_MailStatus", "SubscriberId", "dbo.M_Subscriber");
            DropForeignKey("dbo.M_MailStatus", "S_StatusId", "dbo.S_Status");
            DropForeignKey("dbo.M_MailStatus", "ListId", "dbo.M_List");
            DropForeignKey("dbo.M_MailStatus", "CampId", "dbo.M_Campaigns");
            DropForeignKey("dbo.M_CustomException", "userId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ListSusbscribers", "SubscribersID", "dbo.M_Subscriber");
            DropForeignKey("dbo.ListSusbscribers", "ListID", "dbo.M_List");
            DropForeignKey("dbo.M_Subscriber", "ListID", "dbo.M_List");
            DropForeignKey("dbo.M_Campaigns", "S_Status_Statid", "dbo.S_Status");
            DropForeignKey("dbo.M_Campaigns", "ListId", "dbo.M_List");
            DropForeignKey("dbo.M_UsersListCampaign", "UsersID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.M_UsersListCampaign", "ListID", "dbo.M_List");
            DropForeignKey("dbo.M_UsersListCampaign", "M_Campaigns_Cid", "dbo.M_Campaigns");
            DropForeignKey("dbo.M_Campaigns", "CTypeId", "dbo.S_CampaignTypes");
            DropForeignKey("dbo.M_List", "CountryID", "dbo.S_Country");
            DropIndex("dbo.UsersLists", new[] { "ListID" });
            DropIndex("dbo.UsersLists", new[] { "UsersID" });
            DropIndex("dbo.UsersCampaigns", new[] { "CampaignID" });
            DropIndex("dbo.UsersCampaigns", new[] { "UsersID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.MailerQueues", new[] { "CampaignID" });
            DropIndex("dbo.M_Tracking", new[] { "SubsciberId" });
            DropIndex("dbo.M_Tracking", new[] { "CampId" });
            DropIndex("dbo.M_Subscription", new[] { "PlanId" });
            DropIndex("dbo.M_Subscription", new[] { "UserId" });
            DropIndex("dbo.M_Profile", new[] { "UserID" });
            DropIndex("dbo.M_MailStatus", new[] { "S_StatusId" });
            DropIndex("dbo.M_MailStatus", new[] { "SubscriberId" });
            DropIndex("dbo.M_MailStatus", new[] { "ListId" });
            DropIndex("dbo.M_MailStatus", new[] { "CampId" });
            DropIndex("dbo.M_MailStatus", new[] { "UserId" });
            DropIndex("dbo.M_CustomException", new[] { "userId" });
            DropIndex("dbo.M_Subscriber", new[] { "ListID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.M_UsersListCampaign", new[] { "M_Campaigns_Cid" });
            DropIndex("dbo.M_UsersListCampaign", new[] { "ListID" });
            DropIndex("dbo.M_UsersListCampaign", new[] { "UsersID" });
            DropIndex("dbo.M_Campaigns", new[] { "S_Status_Statid" });
            DropIndex("dbo.M_Campaigns", new[] { "ListId" });
            DropIndex("dbo.M_Campaigns", new[] { "CTypeId" });
            DropIndex("dbo.M_List", new[] { "CountryID" });
            DropIndex("dbo.ListSusbscribers", new[] { "SubscribersID" });
            DropIndex("dbo.ListSusbscribers", new[] { "ListID" });
            DropTable("dbo.UsersLists");
            DropTable("dbo.UsersCampaigns");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.MailerQueues");
            DropTable("dbo.M_Tracking");
            DropTable("dbo.M_Subscription");
            DropTable("dbo.M_Profile");
            DropTable("dbo.M_MailStatus");
            DropTable("dbo.M_MailPlans");
            DropTable("dbo.M_CustomException");
            DropTable("dbo.M_Subscriber");
            DropTable("dbo.S_Status");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.M_UsersListCampaign");
            DropTable("dbo.S_CampaignTypes");
            DropTable("dbo.M_Campaigns");
            DropTable("dbo.S_Country");
            DropTable("dbo.M_List");
            DropTable("dbo.ListSusbscribers");
        }
    }
}
