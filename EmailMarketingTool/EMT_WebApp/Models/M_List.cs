using EMT_WebApp.Helpers;
using EMT_WebApp.Models;
using EMT_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Models
{
    public class M_List
    {
        /// <summary>
        /// constructor to intialize navigation properties
        /// </summary>
        public M_List()
        {
            this.M_Campaigns = new HashSet<M_Campaigns>();
            this.Subscribers = new HashSet<M_Subscriber>();
            this.M_UsersListCampaign = new HashSet<M_UsersListCampaign>();
            this.ListSusbscribers = new HashSet<ListSusbscriber>();
        }
        //public static string GetURL()
        //{
        //    HttpRequest request = HttpContext.Current.Request;
        //    string url = request.Url.ToString();
        //    return url;
        //}
        [Key]
        public int ListID { get; set; }
        public string ListName { get; set; }
        public string DefaultFromEmail { get; set; }
        public string DefaultFromName { get; set; }
        public string CompanyorOrganization { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [ForeignKey("Country")]
        public Nullable<int> CountryID { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public string ExcelCSVFilePath { get; set; }
        public string PostalCode { get; set; }

        public virtual S_Country Country { get; set; }
        public virtual ICollection<M_Campaigns> M_Campaigns { get; set; }
        public virtual ICollection<M_Subscriber> Subscribers { get; set; }
        public virtual ICollection<M_UsersListCampaign> M_UsersListCampaign { get; set; }
        public virtual ICollection<ListSusbscriber> ListSusbscribers { get; set; }

        static ApplicationDbContext dbcontext;
        List<string> ListNames = new List<string>();
        static List<int?> listIds;
        UsersList user = new UsersList();
        ListViewModel LVmodel = new ListViewModel();
        static List<M_List> lists;
        M_List newList = null;
        static M_CustomException obj;

        /// <summary>
        /// Get list ids collection for perticular user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>listids collection</returns>
        public static List<int?> GetListIds(string userID)
        {

            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    listIds = new List<int?>();
                    listIds = dbcontext.UsersList.Where(u => u.UsersID == userID).Select(l => l.ListID).ToList();
                    return listIds;
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
        /// <summary>
        /// gets all lists
        /// </summary>
        /// <returns></returns>
        public static List<M_List> GetLists()
        {
            List<M_List> lists1 = new List<M_List>();
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    lists1 = dbcontext.M_Lists.ToList();
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
            return lists1;
        }
        /// <summary>
        ///     Function to save New List object,and add list id into userslist table
        /// </summary>
        /// <param name="obj">New List type object</param>
        /// <param name="userID">User for which list is created</param>
        /// <returns></returns>
        public string SaveList(string userID)
        {
            if (CheckListExist(this.ListName, userID) == false)
            {
                user.UsersID = userID;
                try
                {
                    using (dbcontext = new ApplicationDbContext())
                    {
                        //begin transaction
                        using (var trans = dbcontext.Database.BeginTransaction())
                        {
                            try
                            {

                                this.CreatedDate = DateTime.Now;
                                dbcontext.M_Lists.Add(this);
                                dbcontext.SaveChanges();

                                user.ListID = this.ListID;
                                dbcontext.UsersList.Add(user);
                                dbcontext.SaveChanges();
                                trans.Commit();


                            }
                            catch (SqlException ex)
                            {

                                trans.Rollback();
                                obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), userID, Utlities.GetURL(), ex.LineNumber);

                                obj.LogException();
                                throw obj;
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), userID, Utlities.GetURL());

                                obj.LogException();
                                throw obj;
                            }

                        }
                    }
                    return "Saved";
                }
                catch (Exception ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), userID, Utlities.GetURL());
                    obj.LogException();
                    throw obj;
                }
            }
            //if listname already exist
            else {
                return "List name already exist";
            }

        }

        /// <summary>
        /// Function to check if list name is already exist
        /// </summary>
        /// <param name="listName">user want to create list by this name</param>
        /// <param name="UserID">User for which list is created</param>
        /// <returns>true if listname is already exist and false if listname does not exist</returns>
        public bool CheckListExist(string listName, string userID)
        {
            listIds = new List<int?>();
            //  listIds = GetListIds(userID);
            bool res = false;
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    var listdata = from list in dbcontext.M_Lists
                                   join users in dbcontext.UsersList
                                   on list.ListID equals users.ListID
                                   where users.UsersID == userID
                                   select new { list.ListName };
                    //foreach (var item in listIds)
                    //{
                    //    ListNames.Add(dbcontext.NewLists.Where(l => l.ListID == item).Select(l => l.ListName).FirstOrDefault());
                    //}
                    if (listdata != null)
                    {
                        res = listdata.Any(l => l.ListName == listName);
                    }
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
            // return ListNames.Any(l => l == listName);
            return res;
        }

        /// <summary>
        /// Get lists to display for user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>return list view model which contains list information and count of its subcribers and unsubscribers</returns>
        public ListViewModel ViewList(string userID)
        {
            // NewList lst = null;
            //listIds = new List<int?>();
            // listIds = GetListIds(userID);
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    LVmodel.lists = (from list in dbcontext.M_Lists
                                     join users in dbcontext.UsersList
                                     on list.ListID equals users.ListID
                                     where users.UsersID == userID
                                     select list).ToList();

                    foreach (var i in LVmodel.lists)
                    {
                        //int cnt = (from list in LVmodel.lists
                        //           join sub in dbcontext.Subscribers
                        //           on list.ListID equals sub.ListID
                        //           where sub.ListID == i.ListID
                        //           select sub).Count();
                        int cnt = (from list in LVmodel.lists
                                   join sub in dbcontext.ListSusbscribers
                                   on list.ListID equals sub.ListID
                                   where sub.ListID == i.ListID
                                   select list).Count();
                        LVmodel.NoOfSubscribers.Add(cnt);
                    }

                    //foreach (var item in listIds)
                    //{
                    //    lst = new NewList();
                    //    lst = dbcontext.NewLists.Where(u => u.ListID == item).FirstOrDefault();
                    //    LVmodel.lists.Add(lst);
                    //    var cnt = dbcontext.Subscribers.Where(l => l.ListID == item && l.Unsubscribe == false).Count();
                    //    LVmodel.NoOfSubscribers.Add(cnt);
                    //}
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
            return LVmodel;
        }
        /// <summary>
        /// gets list of lists created by perticular user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>List of created list</returns>
        public static List<M_List> GetLists(string userID)
        {
            // listIds = new List<int?>();
            lists = new List<M_List>();
            listIds = GetListIds(userID);
            foreach (var item in listIds)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    lists.Add(dbcontext.M_Lists.Where(l => l.ListID == item).FirstOrDefault());
                }
            }
            return lists;
        }
        /// <summary>
        /// Gets list information to be edited
        /// </summary>
        /// <param name="listID"></param>
        /// <returns>List obejct</returns>
        public M_List EditList(int? listID)
        {
            newList = new M_List();
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    newList = dbcontext.M_Lists.Find(listID);
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
            return newList;
        }
        /// <summary>
        /// Update list to database
        /// </summary>
        /// <returns>true or false</returns>
        public bool UpdateList()
        {
            this.LastUpdated = DateTime.Now;
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
            return true;
        }

    }
}