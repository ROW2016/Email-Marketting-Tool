using EMT_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT_Exception
{
    public class EMTException : Exception
    {

        private int ID { get; set; }
        private int ErrorCode { get; set; }

        private string message { get; set; }
        private string stackTrace { get; set; }

        private string Type { get; set; }

        private DateTime? time { get; set; }

        private string url { get; set; }

        private bool status { get; set; }

        private string userId { get; set; }
        private int lineNumber { get; set; }

        private string dbConnection;
        public enum ErorrTypes
        {
            SqlExceptions = 100,
            LogicExceptions = 101,
            ArgumentNullExceptions = 102,
            ApplicationExceptions = 103,
            HttpExceptions = 104,
            others = 105,
            SMTPExceptions = 106
        }

        public EMTException(int ec, string msg, string st, string type) : base()
        {
            this.ErrorCode = ec;
            this.message = msg;
            this.stackTrace = st;
            this.Type = type;
            this.time = DateTime.Now;
        }
        public EMTException(int ec, string msg, string st, string type, string user, string url, int linenum) : base()
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

        public EMTException(int ec, string msg, string st, string type, string url, int linenum) : base()
        {
            this.ErrorCode = ec;
            this.message = msg;
            this.stackTrace = st;
            this.Type = type;
            this.time = DateTime.Now;
            this.url = url;
            this.lineNumber = linenum;
        }
        public EMTException(int ec, string msg, string st, string type, string url)
        {
            this.ErrorCode = ec;
            this.message = msg;
            this.stackTrace = st;
            this.Type = type;
            this.url = url;
            this.time = DateTime.Now;
        }
        public void LogException()
        {
            AppSettingsReader MyReader = new AppSettingsReader();
            dbConnection = MyReader.GetValue("Connection", typeof(string)).ToString();

            //using (SqlConnection con = new SqlConnection(dbConnection))
            //{
                try
                {
                    //using (SqlCommand cmd = new SqlCommand("usp_recordexception", con))
                    //{
                    //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //    cmd.Parameters.Add("@ErrorCode", System.Data.SqlDbType.Int).Value = ErrorCode;
                    //    cmd.Parameters.Add("@message", System.Data.SqlDbType.NVarChar).Value = message;
                    //    cmd.Parameters.Add("@stackTrace", System.Data.SqlDbType.NVarChar).Value = stackTrace;
                    //    cmd.Parameters.Add("@Type", System.Data.SqlDbType.NVarChar).Value = Type;
                    //    cmd.Parameters.Add("@url", System.Data.SqlDbType.NVarChar).Value = url;
                    //    cmd.Parameters.Add("@status", System.Data.SqlDbType.Bit).Value = status;
                    //    cmd.Parameters.Add("@userId", System.Data.SqlDbType.NVarChar).Value = userId;
                    //    cmd.Parameters.Add("@lineNumber", System.Data.SqlDbType.Int).Value = lineNumber;
                    //    cmd.Parameters.Add("@HelpLink", System.Data.SqlDbType.NVarChar).Value = HelpLink;
                    //    cmd.Parameters.Add("@Source", System.Data.SqlDbType.NVarChar).Value = 1;
                    //    cmd.Parameters.Add("@HResult", System.Data.SqlDbType.Int).Value = HResult;

                    //    con.Open();

                        DataHelper.ExecuteNonQuery(CommandType.StoredProcedure, "usp_recordexception",
                                    new SqlParameter []{new SqlParameter("@ErrorCode", ErrorCode) ,
                                                        new SqlParameter("@message", message),
                                                        new SqlParameter("@stackTrace", stackTrace),
                                            new SqlParameter("@Type", Type),new SqlParameter("@url", url),
                                    new SqlParameter("@status", status),new SqlParameter("@userId", userId),
                                    new SqlParameter("@lineNumber", lineNumber),new SqlParameter("@HelpLink", HelpLink),
                                    new SqlParameter("@Source", Source),new SqlParameter("@HResult", HResult)});
                    //    cmd.ExecuteNonQuery();
                    //}
                }
                catch (Exception)
                {

                    throw;
                }
            //}
        }
    }
}
