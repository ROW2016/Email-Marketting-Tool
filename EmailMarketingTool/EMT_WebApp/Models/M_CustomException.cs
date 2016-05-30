using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

namespace EMT_WebApp.Models
{
    public class M_CustomException : Exception
    {


        public M_CustomException() : base()
        {
            // this.message = Message;
        }
        public M_CustomException(int ec, string msg, string st, string type, string user, string url) : base()
        {
            this.ErrorCode = ec;
            this.message = msg;
            this.stackTrace = st;
            this.Type = type;
            this.time = DateTime.Now;
            this.userId = user;
            this.url = url;
        }
        public M_CustomException(int ec, string msg, string st, string type, string user, string url, int linenum) : base()
        {
            this.ErrorCode = ec;
            this.message = msg;
            this.stackTrace = st;
            this.Type = type;
            this.time = DateTime.Now;
            this.userId = user;
            this.url = url;
            this.lineNumber = linenum;
        }
        public M_CustomException(int ec, string msg, string st, string type, string url, int linenum) : base()
        {
            this.ErrorCode = ec;
            this.message = msg;
            this.stackTrace = st;
            this.Type = type;
            this.time = DateTime.Now;
            this.url = url;
            this.lineNumber = linenum;
        }
        public M_CustomException(int ec, string msg, string st, string type, string url)
        {
            this.ErrorCode = ec;
            this.message = msg;
            this.stackTrace = st;
            this.Type = type;
            this.url = url;
            this.time = DateTime.Now;
        }

        [Key]
        public int ID { get; set; }
        public int ErrorCode { get; set; }

        public string message { get; set; }
        public string stackTrace { get; set; }

        public string Type { get; set; }

        public DateTime? time { get; set; }

        public string url { get; set; }

        public bool status { get; set; }

        public string userId { get; set; }
        public int lineNumber { get; set; }



        public virtual ApplicationUser User { get; set; }

        static ApplicationDbContext dbcontext = null;
        static M_CustomException obj = null;
        public void LogException()
        {
            using (dbcontext = new ApplicationDbContext())
            {
                try
                {
                    dbcontext.M_CustomExceptions.Add(this);
                    dbcontext.SaveChanges();
                }
                catch (SqlException ex)
                {
                    obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), GetURL(), ex.LineNumber);
                    throw obj;
                }
                catch (Exception)
                {
                    //obj = new CustomSqlException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), GetURL());
                    //obj.LogException();
                    //throw obj;
                }
            }
        }
        public static string GetURL()
        {
            HttpRequest request = HttpContext.Current.Request;
            string url = request.Url.ToString();
            return url;
        }

        public static List<M_CustomException> GetExceptionData()
        {
            List<M_CustomException> exp = new List<M_CustomException>();
            try
            {
                using (dbcontext = new ApplicationDbContext())
                {
                    exp = dbcontext.M_CustomExceptions.ToList();
                }
            }
            catch (SqlException ex)
            {
                obj = new M_CustomException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), GetURL(), ex.LineNumber);
                throw obj;
            }
            catch (Exception)
            {
                //obj = new CustomSqlException((int)ErorrTypes.others, ex.Message, ex.StackTrace, ErorrTypes.others.ToString(), GetURL());
                //obj.LogException();
                //throw obj;
            }
            return exp;
        }
    }
    public enum ErorrTypes
    {
        SqlExceptions = 100,
        LogicExceptions = 101,
        ArgumentNullExceptions = 102,
        ApplicationExceptions = 103,
        HttpExceptions = 104,
        others = 105,
        SMTPExceptions = 106,
        InvalidOperation=107
    }
}