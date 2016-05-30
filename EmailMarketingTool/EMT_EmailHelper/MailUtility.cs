using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMT_DataAccessLayer;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using EMT_EmailSender;
using System.Text.RegularExpressions;
using System.Net;
using EMT_Exception;

namespace EMT_EmailHelper
{
    public class MailUtility
    {
        CampaignDetails campaign = null;
        List<SubscriberDetails> subs = null;
        SubscriberDetails sub = null;
        List<string> sidentifiers = null;
        EMTException exp;
        public MailMessage MailMessage { get; set; }
        /// <summary>
        /// Get Campaign Details
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        private CampaignDetails getCampaignDetails(int campaignId)
        {
            try
            {
                using (SqlConnection con = DataHelper.CreateOpenConnection())
                {
                    DataSet ds = new DataSet();
                    campaign = new CampaignDetails();
                    ds = DataHelper.ExecuteDataSet(con, CommandType.StoredProcedure, "usp_getemailcontent", new SqlParameter[] { new SqlParameter("@campaignId", campaignId) });

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            campaign.Cid = (int)row["Cid"];
                            campaign.Name = row["Name"].ToString();
                            campaign.EmailContent = row["EmailContent"].ToString();
                            campaign.EmailSubject = row["EmailSubject"].ToString();
                            campaign.FromEmail = row["FromEmail"].ToString();
                            campaign.ListId = (int)row["ListId"];
                            campaign.FromName = row["FromName"].ToString();
                            campaign.CTypeId = (int)row["CTypeId"];

                            if (row["ScheduleCampaign"].ToString() == "")
                            {
                                var MyReader = new AppSettingsReader();
                                campaign.ScheduleCampaign = Convert.ToInt32(MyReader.GetValue("timeinterval", typeof(string)));
                            }
                            else {
                                campaign.ScheduleCampaign = (int)row["ScheduleCampaign"];
                            }
                            if (row["noOfEmailSendPerInterval"].ToString() == "")
                            {
                                var MyReader = new AppSettingsReader();
                                campaign.noOfEmailSendPerInterval = Convert.ToInt32(MyReader.GetValue("NoOfSubscriber", typeof(string)));
                            }
                            else {
                                campaign.noOfEmailSendPerInterval = (int)row["noOfEmailSendPerInterval"];
                            }
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            return campaign;
        }

        /// <summary>
        /// Get Subscriber Details
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        private List<SubscriberDetails> getSubscriberDetails(string cidef)
        {
            subs = new List<SubscriberDetails>();

            try
            {
                using (SqlConnection con = DataHelper.CreateOpenConnection())
                {

                    using (SqlCommand cmd = new SqlCommand("usp_getsubscribersdetails", con))
                    {

                        System.Data.DataSet ds = new System.Data.DataSet();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@campaignIdentifier", SqlDbType.NVarChar).Value = cidef;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        //rows = cmd.ExecuteNonQuery();
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                sub = new SubscriberDetails();
                                sub.SubscriberID = (int)row["SubscriberID"];
                                sub.FirstName = row["FirstName"].ToString();
                                sub.LastName = row["LastName"].ToString();
                                sub.EmailAddress = row["EmailAddress"].ToString();
                                sub.IdentifierSubscriber = row["IdentifierSubscriber"].ToString();
                                sub.IdentifierCampaign = row["IdentifierCampaign"].ToString();
                                sub.userID = row["UserID"].ToString();
                                subs.Add(sub);
                            }
                            return subs;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            return subs;
        }

        /// <summary>
        /// send mail messages
        /// </summary>
        /// <param name="campaigns"></param>
        public void SendMessage(Campaigns campaigns)
        {
            SendGridMailer obj = new SendGridMailer();
            MailComposer composer = new MailComposer();
            try
            {
                UpdateExecutedDate(campaigns.IdentifierCampaign);
                campaign = new CampaignDetails();
                subs = new List<SubscriberDetails>();
                campaign = getCampaignDetails(campaigns.CampaignID);
                subs = new List<SubscriberDetails>();
                subs = getSubscriberDetails(campaigns.IdentifierCampaign);
                string content = null;
                foreach (var email in subs)
                {
                    content = composer.ComposeHTMLMail(campaign.EmailContent, email.FirstName + " " + email.LastName, email.IdentifierSubscriber);
                    obj.SendHtmlMail(email.EmailAddress, email.FirstName + " " + email.LastName, campaign.FromEmail, campaign.FromName, campaign.EmailSubject, content);

                }
                UpdateMailStatus(campaigns.IdentifierCampaign);
                removeCampaign(campaigns.IdentifierCampaign);
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
           
        }

        /// <summary>
        /// remove campaign from queue after completion
        /// </summary>
        /// <param name="identifier"></param>
        public void removeCampaign(string identifier)
        {
            try
            {
                using (SqlConnection con = DataHelper.CreateOpenConnection())
                {
                    DataHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_removecampaignfromque", new SqlParameter[] { new SqlParameter("@campaignidentifier", identifier) });

                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
        }

        /// <summary>
        /// changes status for subscriber to sent after completion of campaign
        /// </summary>
        /// <param name="identifier"></param>
        public void UpdateMailStatus(string identifier)
        {
            try
            {
                using (SqlConnection con = DataHelper.CreateOpenConnection())
                {
                    //System.Data.DataSet ds = new System.Data.DataSet();
                    //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@identifier", System.Data.SqlDbType.NVarChar).Value = identifier;
                    DataHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_changemailstatus", new SqlParameter[] { new SqlParameter("@identifier", identifier) });

                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }

        }


        public List<string> getSubscribersIdentifier(string campaignidentifier)
        {
            sidentifiers = new List<string>();
            try
            {
                using (SqlConnection con = DataHelper.CreateOpenConnection())
                {

                    using (SqlCommand cmd = new SqlCommand("usp_getsubscriberidentifier", con))
                    {

                        System.Data.DataSet ds = new System.Data.DataSet();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@campaignidentifier", System.Data.SqlDbType.NVarChar).Value = campaignidentifier;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                sidentifiers.Add(row["IdentifierSubscriber"].ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            return sidentifiers;
        }
        /// <summary>
        /// updates the exceution date for campaign in m_tracking table
        /// </summary>
        /// <param name="campaignIdentifier"></param>
        private void UpdateExecutedDate(string campaignIdentifier)
        {
            try
            {
                using (SqlConnection con = DataHelper.CreateOpenConnection())
                {
                    DataHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_updateexecutedateforcampaign", new SqlParameter[] { new SqlParameter("@campaignIdentifier", campaignIdentifier) });
                }
            }
            catch (SqlException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
        }
    }
}
