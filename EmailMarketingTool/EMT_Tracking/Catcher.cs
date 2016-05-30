using EMT_Exception;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace EMT_Tracking
{
    public class Catcher : IHttpModule
    {
        EMTException exp = null;
        private string imgName;
        private string pattern = @"http://uemttrk.azurewebsites.net/image/(?<key>.*)\.png";
        private string logoFile = "http://emailertool.azurewebsites.net/Logo/";
        // private string logoFile = "http://localhost:49392/Logo/";
        // private string logoFile = "http://emailertool.azurewebsites.net/Logo/9ec08d9d-1496-4b70-b40d-5b2216254ee5.png";
        // AppSettingsReader MyReader = new AppSettingsReader();

        //dbConnectionString = MyReader.GetValue("Connection", typeof(string)).ToString();
        private string dbConnectionString = "Server=tcp:row.database.windows.net,1433;Data Source=row.database.windows.net;Initial Catalog=EmailTool;Persist Security Info=False;User ID=paul;Password=awesome123#;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public Catcher() { }
        public void Dispose()
        {

        }
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(GetImage_BeginRequest);
        }
        public void GetImage_BeginRequest(object sender, System.EventArgs args)
        {
            //cast the sender to HttpApplication object
            System.Web.HttpApplication application = (System.Web.HttpApplication)sender;
            //get the url path
            string url = application.Request.Path;
            //create the regex to match for beacon images
            Regex r = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (r.IsMatch(url))
            {
                MatchCollection mc = r.Matches(url);
                if ((mc != null) && (mc.Count > 0))
                {
                    string key = (mc[0].Groups["key"].Value);
                    //getting the campaignId and SubsriberId
                    //string[] keys = key.Split('_');
                    //int campId, SubsId;
                    string SIdentifier;
                    SIdentifier = key;
                    
                    // campId = Convert.ToInt32(keys[0]);
                    // SubsId = Convert.ToInt32(keys[1]);

                    //update into database
                    record(SIdentifier);

                    //get image name from database
                    imgName = GetLogo(SIdentifier);
                    //logoFile += imgName;
                    //Uri imageurl = new Uri(logoFile + imgName);
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(logoFile + imgName, application.Request.MapPath("\\Logos\\" + SIdentifier));
                        // client.DownloadFile(logoFile,@"F:\Anupama\Final Package\Paul\EmailMarketingTool\EMT_Tracking\Logos" + imgName);
                    }
                }
                //send the REAL image to the client
                application.Response.ContentType = "image/png";
                application.Response.WriteFile(application.Request.MapPath("\\Logos\\" + imgName));
                //application.Response.WriteFile(application.Request.MapPath(logoFile + imgName));
                application.Response.End();

            }

        }


        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            string filepath = "/Logos/";
            File.WriteAllBytes(filepath, e.Result);
        }

        /// <summary>
        /// Save the subsbscriber and campaign on the date which he has opened.
        /// </summary>
        /// <param name="cId">Camapign </param>
        /// <param name="sid">Subscriber </param>
        public void record(string SIdentifier)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(dbConnectionString))
                {
                    //bool = 
                    if (isUserAvailable(SIdentifier))
                    {
                        return;
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_tracking_recordmailopend", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add("@SIdentifier", System.Data.SqlDbType.NVarChar).Value = SIdentifier;
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
                throw;
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
                throw;
            }
        }

        /// <summary>
        /// before recording the campaign check if he has opened the campaign.
        /// </summary>
        /// <param name="cId"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public bool isUserAvailable(string SIdentifier)
        {
            using (SqlConnection con = new SqlConnection(dbConnectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("usp_tracking_checkIsmailOpend", con))
                    {

                        System.Data.DataSet ds = new System.Data.DataSet();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SIdentifier", System.Data.SqlDbType.NVarChar).Value = SIdentifier;
                        //cmd.Parameters.Add("@SubsciberId", System.Data.SqlDbType.Int).Value = sid;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        //rows = cmd.ExecuteNonQuery();
                         return  Convert.ToBoolean(ds.Tables[0].Rows[0]["IsOpened"]);
                        //if (ds.Tables[0].Rows.Count > 0)
                        //{
                        //    return false;
                        //}
                        //else
                        //{
                        //    return true;
                        //}
                    }
                }
                catch (SqlException ex)
                {
                    exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                    exp.LogException();
                    throw;
                }
                catch (Exception ex)
                {
                    exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                    exp.LogException();
                    throw;
                }
            }
        }


        /// <summary>
        /// get the logo of the user.
        /// 
        /// </summary>
        /// <param name="campaignID"></param>
        /// <returns></returns>
        public string GetLogo(string subscriberidentifier)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(dbConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_tracking_getcompanylogo", con))
                    {

                        System.Data.DataSet ds = new System.Data.DataSet();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@subscriberidentifier", System.Data.SqlDbType.NVarChar).Value = subscriberidentifier;

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0].Rows[0][0].ToString();

                        }
                        else
                        {
                            return "row_Logo.png";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
                throw;
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
                throw;
            }
        }

    }
}
