using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Excel;
using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.IO;
using EMT_WebApp.ViewModels;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using EMT_WebApp.Helpers;

namespace EMT_WebApp.Models
{
    public class M_Subscriber
    {
        public M_Subscriber()
        {
            this.ListSusbscribers = new HashSet<ListSusbscriber>();
        }
      
        [Key]
        public int SubscriberID { get; set; }

        [ForeignKey("NewList")]
        public Nullable<int> ListID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AlternateEmailAddress { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<System.DateTime> LastChanged { get; set; }

        [DefaultValue(false)]
        public bool Unsubscribe { get; set; }

        //[ForeignKey("Status")]
        //public int StatusID { get; set; }

        //public virtual S_Status Status { get; set; }

        public virtual M_List NewList { get; set; }
        public virtual ICollection<ListSusbscriber> ListSusbscribers { get; set; }

        /// <summary>
        /// Database logic starts here
        /// </summary>
        static ApplicationDbContext dbcontext;
        static List<string> subscriberEmail;
        static M_Subscriber subscriber;
        List<M_Subscriber> subscribersList = new List<M_Subscriber>();
        M_List list = new M_List();
        List<int?> listIds = null;
        static M_CustomException obj;
        /// <summary>
        /// Save subscriber to database
        /// </summary>
        /// <param name="listID">list from which subscriber belogs</param>
        /// <param name="userID"></param>
        public void saveSubscriber(string userID)
        {
            ListSusbscriber lSub = new ListSusbscriber();
            if (isSubscriberExist(this.EmailAddress, this.ListID, userID) == false)
            {
                lSub.ListID = this.ListID;
               // this.StatusID = 3;
                this.AddedDate = DateTime.Now;
                using (dbcontext = new ApplicationDbContext())
                {
                    using (var trans = dbcontext.Database.BeginTransaction())
                    {
                        //Subscriber not present in list
                        try
                        {

                            dbcontext.M_Subscribers.Add(this);
                            dbcontext.SaveChanges();
                            lSub.SubscribersID = this.SubscriberID;
                            dbcontext.ListSusbscribers.Add(lSub);
                            dbcontext.SaveChanges();
                            trans.Commit();

                        }
                        catch (SqlException ex)
                        {
                            //failed to save in subscriber or listsubscriber,clear model ans add model error
                            trans.Rollback();
                            obj = new M_CustomException((int)ErorrTypes.SqlExceptions, "Problem in saving data", ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

                            obj.LogException();
                            throw obj;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            obj = new M_CustomException((int)ErorrTypes.others, "Some problem occured while processing request", ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                            obj.LogException();
                            throw obj;
                        }
                    }
                }
            }
            else {
                //Subscriber already present in list,clear model ans add model error
            }
        }

        /// <summary>
        /// checks if subscriber already present in list or not,if  not present then add else dont
        /// </summary>
        /// <param name="emailAddress">check unique field for subscriber</param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool isSubscriberExist(string emailAddress, int? listID, string userID)
        {
            List<M_Subscriber> subs = new List<M_Subscriber>();
            subs = GetsubscribersToList(listID, userID);
            return subs.Any(s => s.EmailAddress == emailAddress);
        }

        /// <summary>
        /// collection of emailaddress for peticular list
        /// </summary>
        /// <param name="userID">collection of list ids for perticular user</param>
        /// <returns></returns>
        public List<string> getSubscribers(string userID)
        {
            subscriberEmail = new List<string>();
            listIds = new List<int?>();
            listIds = M_List.GetListIds(userID);
            using (dbcontext = new ApplicationDbContext())
            {
                foreach (var item in listIds)
                {
                    try
                    {
                        subscriberEmail.Add(dbcontext.M_Subscribers.Where(l => l.ListID == item).Select(s => s.EmailAddress).FirstOrDefault());
                    }
                    catch (SqlException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

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
            return subscriberEmail;
        }

        /// <summary>
        /// get collection of subscribers for perticular list
        /// </summary>
        /// <param name="listId">if listid is null display all subscribers for user and if listid is not null display subscriber 
        /// for perticular list</param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<M_Subscriber> GetsubscribersToList(int? listId, string userID)
        {

            if (listId != null)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    try
                    {
                        subscribersList = dbcontext.M_Lists.Find(listId).Subscribers.Where(s => s.Unsubscribe == false).ToList();
                    }
                    catch (SqlException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

                        obj.LogException();
                        throw obj;
                    }
                    catch (Exception ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                        obj.LogException();
                        throw obj;
                    }
                }
                return subscribersList;

            }
            return subscribersList;
            //else {
            //    subscribersList = GetAllSubscribers(userID);
            //    return subscribersList;
            //}
        }
        /// <summary>
        /// if listid is not null get subscribers by listid
        /// </summary>
        /// <param name="listID">is not null</param>
        /// <returns></returns>
        public List<M_Subscriber> GetSubscribersbyListID(int? listID)
        {
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    subscribersList = dbcontext.M_Lists.Find(listID).Subscribers.Where(s => s.Unsubscribe == false).ToList();
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

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
            return subscribersList;
        }

        /// <summary>
        /// if listid is null get all subscribers 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<M_Subscriber> GetAllSubscribers(string userID)
        {
            List<M_Subscriber> subscribersList = new List<M_Subscriber>();
            List<M_Subscriber> sub = null;
            listIds = new List<int?>();
            listIds = M_List.GetListIds(userID);
            foreach (var item in listIds)
            {
                sub = new List<M_Subscriber>();
                sub = GetSubscribersbyListID(item);
                foreach (var i in sub.ToList())
                {
                    subscribersList.Add(i);
                }
            }
            return subscribersList;
        }
        /// <summary>
        /// Imports excel data and saves to database
        /// </summary>
        /// <param name="UploadFile"></param>
        /// <param name="model"></param>
        public void ImportExcel(HttpPostedFileBase UploadFile, SubscribersViewModel model)
        {
            DataSet excelResult = new DataSet();
            IExcelDataReader excelDataReader;
            subscriber = new M_Subscriber();
            if (UploadFile != null && UploadFile.ContentLength > 0)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    Stream stream = UploadFile.InputStream;

                    string filename = System.IO.Path.GetFileNameWithoutExtension(UploadFile.FileName);
                    if (UploadFile.FileName.EndsWith(".xlsx"))
                    {

                        string thisFile = filename + "_" + model.ListID + ".xlsx";
                        // var path = System.IO.Path.Combine(Server.MapPath("~/ExcelFiles"), thisFile);
                        excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        excelDataReader.IsFirstRowAsColumnNames = true;
                        excelResult = excelDataReader.AsDataSet();
                        model.dataTable = excelResult.Tables[0];
                        //read column headers
                        //  var columnHeaders = (from DataColumn dc in model.dataTable.Columns select dc.ColumnName).ToArray();
                        // RedirectToAction("SetColumnHeader", columnHeaders);


                        //save list to database
                        if (model.dataTable.Rows.Count > 0)
                        {
                            // UploadFile.SaveAs(path);
                            //ColumnList = ReadExcelHeader(path);
                            for (int i = 0; i < model.dataTable.Rows.Count; i++)
                            {
                                List<string> sub = new List<string>();
                                sub = dbcontext.M_Subscribers.Where(l => l.ListID == model.ListID).Select(m => m.EmailAddress).ToList();
                                bool ispresent = false;
                                try
                                {
                                    ispresent = sub.Any(s => s == model.dataTable.Rows[i]["EmailAddress"].ToString());
                                }
                                catch (ArgumentException ex)
                                {
                                    // ModelState.AddModelError("Fileerr", "Please see sample file format");
                                    // return View();
                                }
                                catch (Exception ex)
                                {
                                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                                    obj.LogException();
                                    throw obj;
                                }
                                if (ispresent == false)
                                {
                                    ListSusbscriber lSub = new ListSusbscriber();

                                    lSub.ListID = model.ListID;
                                    using (var trans = dbcontext.Database.BeginTransaction())
                                    {
                                        //ObjectParameter objParam = new ObjectParameter("ID", typeof(int));
                                        try
                                        {
                                            subscriber.ListID = Convert.ToInt32(model.ListID);
                                            subscriber.FirstName = model.dataTable.Rows[i]["FirstName"].ToString();
                                            subscriber.LastName = model.dataTable.Rows[i]["LastName"].ToString();
                                            subscriber.EmailAddress = model.dataTable.Rows[i]["EmailAddress"].ToString();
                                            subscriber.AlternateEmailAddress = model.dataTable.Rows[i]["AlternateEmailAddress"].ToString();
                                            subscriber.Address = model.dataTable.Rows[i]["Address"].ToString();
                                            subscriber.Country = model.dataTable.Rows[i]["Country"].ToString();
                                            subscriber.City = model.dataTable.Rows[i]["City"].ToString();
                                            subscriber.AddedDate = DateTime.Now;
                                            //subscriber.StatusID = 3;
                                            dbcontext.M_Subscribers.Add(subscriber);
                                            dbcontext.SaveChanges();
                                            //dbcontext.ImportSubscribers(Convert.ToInt32(model.ListID), model.dataTable.Rows[i]["FirstName"].ToString(),
                                            //      model.dataTable.Rows[i]["LastName"].ToString(), model.dataTable.Rows[i]["EmailAddress"].ToString(), model.dataTable.Rows[i]["AlternateEmailAddress"].ToString(),
                                            //      model.dataTable.Rows[i]["Address"].ToString(), model.dataTable.Rows[i]["Country"].ToString(), model.dataTable.Rows[i]["City"].ToString(),
                                            //      DateTime.Now.ToString(), objParam);

                                            //lSub.SubscribersID = Convert.ToInt32(objParam.Value);
                                            // lSub.SubscribersID = (int?)((SqlParameter)param[9]).Value;
                                            lSub.SubscribersID = subscriber.SubscriberID;
                                            dbcontext.ListSusbscribers.Add(lSub);
                                            dbcontext.SaveChanges();
                                            trans.Commit();
                                        }
                                        catch (ArgumentException ex)
                                        {
                                            // ModelState.AddModelError("Fileerr", "Please see sample file format");
                                            // return View();
                                        }
                                        catch (Exception ex)
                                        {
                                            trans.Rollback();
                                            obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                                            obj.LogException();
                                            throw obj;
                                        }
                                    }
                                }
                                else {
                                    // ModelState.AddModelError("present", "Some subscribers already present");
                                }

                            }
                        }
                        // return RedirectToAction("ViewSubscribers/" + model.ListID);
                    }
                    else if (UploadFile.FileName.EndsWith(".xls"))
                    {
                        string thisFile = filename + "_" + model.ListID + ".xls";
                        // var path = System.IO.Path.Combine(Server.MapPath("~/ExcelFiles"), thisFile);
                        excelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        excelDataReader.IsFirstRowAsColumnNames = true;
                        excelResult = excelDataReader.AsDataSet();
                        model.dataTable = excelResult.Tables[0];
                        //read column headers
                        var columnHeaders = (from DataColumn dc in model.dataTable.Columns select dc.ColumnName).ToArray();
                        if (model.dataTable.Rows.Count > 0)
                        {
                            //UploadFile.SaveAs(path);
                            for (int i = 0; i < model.dataTable.Rows.Count; i++)
                            {
                                List<string> sub = new List<string>();
                                sub = dbcontext.M_Subscribers.Where(l => l.ListID == model.ListID).Select(m => m.EmailAddress).ToList();
                                bool ispresent = false;
                                try
                                {
                                    ispresent = sub.Any(s => s == model.dataTable.Rows[i]["EmailAddress"].ToString());
                                }
                                catch (ArgumentException ex)
                                {
                                    //ModelState.AddModelError("Fileerr", "Please see sample file format");
                                    //  return View();
                                }
                                if (ispresent == false)
                                {
                                    ListSusbscriber lSub = new ListSusbscriber();
                                    lSub.ListID = model.ListID;
                                    using (var trans = dbcontext.Database.BeginTransaction())
                                    {
                                        //  ObjectParameter objParam = new ObjectParameter("ID", typeof(int));
                                        try
                                        {
                                            subscriber.ListID = Convert.ToInt32(model.ListID);
                                            subscriber.FirstName = model.dataTable.Rows[i]["FirstName"].ToString();
                                            subscriber.LastName = model.dataTable.Rows[i]["LastName"].ToString();
                                            subscriber.EmailAddress = model.dataTable.Rows[i]["EmailAddress"].ToString();
                                            subscriber.AlternateEmailAddress = model.dataTable.Rows[i]["AlternateEmailAddress"].ToString();
                                            subscriber.Address = model.dataTable.Rows[i]["Address"].ToString();
                                            subscriber.Country = model.dataTable.Rows[i]["Country"].ToString();
                                            subscriber.City = model.dataTable.Rows[i]["City"].ToString();
                                            subscriber.AddedDate = DateTime.Now;
                                           // subscriber.StatusID = 3;
                                            dbcontext.M_Subscribers.Add(subscriber);
                                            dbcontext.SaveChanges();
                                            //dbcontext.ImportSubscribers(Convert.ToInt32(model.ListID), model.dataTable.Rows[i]["FirstName"].ToString(),
                                            //      model.dataTable.Rows[i]["LastName"].ToString(), model.dataTable.Rows[i]["EmailAddress"].ToString(), model.dataTable.Rows[i]["AlternateEmailAddress"].ToString(),
                                            //      model.dataTable.Rows[i]["Address"].ToString(), model.dataTable.Rows[i]["Country"].ToString(), model.dataTable.Rows[i]["City"].ToString(),
                                            //      DateTime.Now.ToString(), objParam);

                                            //lSub.SubscribersID = Convert.ToInt32(objParam.Value);
                                            // lSub.SubscribersID = (int?)((SqlParameter)param[9]).Value;
                                            lSub.SubscribersID = subscriber.SubscriberID;
                                            dbcontext.ListSusbscribers.Add(lSub);
                                            dbcontext.SaveChanges();
                                            trans.Commit();
                                        }
                                        catch (ArgumentException ex)
                                        {
                                            // ModelState.AddModelError("Fileerr", "Please see sample file format");
                                            // return View();
                                        }
                                        catch (SqlException)
                                        {
                                            trans.Rollback();
                                            //  ModelState.AddModelError("Fileerr", "Please see sample file format");
                                        }
                                        catch (Exception ex)
                                        {
                                            obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                                            obj.LogException();
                                            throw obj;
                                        }
                                    }
                                }
                                else {
                                    // ModelState.AddModelError("present", "Some subscribers already present");
                                }
                            }

                        }
                        //  return RedirectToAction("ViewSubscribers/" + model.ListID);
                    }
                }
            }
        }
        /// <summary>
        /// Imports CSV data and saves to database
        /// </summary>
        /// <param name="UploadFile"></param>
        /// <param name="model"></param>
        public void ImportCSV(HttpPostedFileBase UploadFile, SubscribersViewModel model)
        {
            subscriber = new M_Subscriber();
            string filename = System.IO.Path.GetFileNameWithoutExtension(UploadFile.FileName);
            if (UploadFile.FileName.EndsWith(".csv"))
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    Stream stream = UploadFile.InputStream;
                    string thisFile = filename + "_" + model.ListID + ".csv";
                    // var path = System.IO.Path.Combine(Server.MapPath("~/CSVFiles"), thisFile);
                    DataTable csvTable = new DataTable();
                    using (CsvReader csvReader = new CsvReader(new StreamReader(stream), true))
                    {
                        csvTable.Load(csvReader);
                    }
                    model.dataTable = csvTable;
                    //read column headers
                    var columnHeaders = (from DataColumn dc in model.dataTable.Columns select dc.ColumnName).ToArray();
                    //save list to database
                    if (model.dataTable.Rows.Count > 0)
                    {
                        //UploadFile.SaveAs(path);
                        for (int i = 0; i < model.dataTable.Rows.Count; i++)
                        {
                            try
                            {
                                List<string> sub = new List<string>();
                                sub = dbcontext.M_Subscribers.Where(l => l.ListID == model.ListID).Select(m => m.EmailAddress).ToList();
                                bool ispresent = false;
                                try
                                {
                                    ispresent = sub.Any(s => s == model.dataTable.Rows[i]["EmailAddress"].ToString());
                                }
                                catch (ArgumentException ex)
                                {
                                    //ModelState.AddModelError("Fileerr", "Please see sample file format");
                                    //return View();
                                }
                                if (ispresent == false)
                                {
                                    ListSusbscriber lSub = new ListSusbscriber();
                                    lSub.ListID = model.ListID;
                                    using (var trans = dbcontext.Database.BeginTransaction())
                                    {
                                        // ObjectParameter objParam = new ObjectParameter("ID", typeof(int));
                                        try
                                        {
                                            subscriber.ListID = Convert.ToInt32(model.ListID);
                                            subscriber.FirstName = model.dataTable.Rows[i]["FirstName"].ToString();
                                            subscriber.LastName = model.dataTable.Rows[i]["LastName"].ToString();
                                            subscriber.EmailAddress = model.dataTable.Rows[i]["EmailAddress"].ToString();
                                            subscriber.AlternateEmailAddress = model.dataTable.Rows[i]["AlternateEmailAddress"].ToString();
                                            subscriber.Address = model.dataTable.Rows[i]["Address"].ToString();
                                            subscriber.Country = model.dataTable.Rows[i]["Country"].ToString();
                                            subscriber.City = model.dataTable.Rows[i]["City"].ToString();
                                            subscriber.AddedDate = DateTime.Now;
                                           // subscriber.StatusID = 3;
                                            dbcontext.M_Subscribers.Add(subscriber);
                                            dbcontext.SaveChanges();
                                            //dbcontext.ImportSubscribers(Convert.ToInt32(model.ListID), model.dataTable.Rows[i]["FirstName"].ToString(),
                                            //      model.dataTable.Rows[i]["LastName"].ToString(), model.dataTable.Rows[i]["EmailAddress"].ToString(), model.dataTable.Rows[i]["AlternateEmailAddress"].ToString(),
                                            //      model.dataTable.Rows[i]["Address"].ToString(), model.dataTable.Rows[i]["Country"].ToString(), model.dataTable.Rows[i]["City"].ToString(),
                                            //      DateTime.Now.ToString(), objParam);

                                            //lSub.SubscribersID = Convert.ToInt32(objParam.Value);
                                            // lSub.SubscribersID = (int?)((SqlParameter)param[9]).Value;
                                            lSub.SubscribersID = subscriber.SubscriberID;
                                            dbcontext.ListSusbscribers.Add(lSub);
                                            dbcontext.SaveChanges();
                                            trans.Commit();
                                        }
                                        catch (SqlException)
                                        {
                                            trans.Rollback();
                                        }
                                        catch (Exception ex)
                                        {
                                            obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                                            obj.LogException();
                                            throw obj;
                                        }
                                    }
                                }
                                else {
                                    //   ModelState.AddModelError("present", "Some subscribers already present");
                                }
                            }
                            catch (ArgumentException ex)
                            {
                                //ModelState.AddModelError("Fileerr", "Please see sample file format");
                                //return View();
                            }
                            catch (Exception ex)
                            {
                                obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), Utlities.GetURL());
                                obj.LogException();
                                throw obj;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get subscriber to be edited
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public M_Subscriber Edit(int? sid)
        {
            if (sid != null)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    try
                    {
                        subscriber = dbcontext.M_Subscribers.Find(sid);
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
            return subscriber;
        }
        /// <summary>
        /// update subscribers information to database
        /// </summary>
        public void Update()
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
        /// <summary>
        /// Delete perticular subscriber from database
        /// </summary>
        /// <param name="sid"></param>
        public void Delete(int? sid)
        {
            if (sid != null)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    var ls = dbcontext.ListSusbscribers.SingleOrDefault(l => l.SubscribersID == sid);
                    subscriber = dbcontext.M_Subscribers.Find(sid);
                    int? lid = subscriber.ListID;
                    if (ls != null)
                    {
                        try
                        {
                            dbcontext.ListSusbscribers.Remove(ls);
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
                    try
                    {
                        dbcontext.M_Subscribers.Remove(subscriber);
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

        public List<M_Subscriber> Unsubscriber(int? lid)
        {
            // listIds = list.GetListIds(userID);
            if (lid != null)
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    try
                    {
                        subscribersList = new List<M_Subscriber>();
                        subscribersList = dbcontext.M_Subscribers.Where(l => l.ListID == lid && l.Unsubscribe == true).ToList();
                    }
                    catch (SqlException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

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
            return subscribersList;
        }
        public static bool Unsub(string subid)
        {
            using (dbcontext = new ApplicationDbContext())
            {
                subscriber = new M_Subscriber();
                int? id;
                if (subid != null)
                {
                    try
                    {
                        id = dbcontext.M_Trackings.Where(t => t.IdentifierSubscriber == subid).Select(t => t.SubsciberId).FirstOrDefault();
                        subscriber = dbcontext.M_Subscribers.Find(id);
                        subscriber.Unsubscribe = true;
                        dbcontext.M_Subscribers.Attach(subscriber);
                        dbcontext.Entry(subscriber).Property(p => p.Unsubscribe).IsModified = true;
                        dbcontext.SaveChanges();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        obj = new M_CustomException((int)ErorrTypes.SqlExceptions, ex.Message, ex.StackTrace, ErorrTypes.SqlExceptions.ToString(), Utlities.GetURL(), ex.LineNumber);

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
                else
                {
                    return false;
                }
            }
        }
      
    }

}
