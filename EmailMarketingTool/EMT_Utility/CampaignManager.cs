using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMT_EmailHelper;
using EMT_DataAccessLayer;
using EMT_Exception;

namespace EMT_Utility
{
    public class CampaignManager
    {
        private string dbConnection;
        private int CampaignCount;
        AppSettingsReader MyReader;
        List<Campaigns> campaigns;
        EMTException exp = null;

        public CampaignManager()
        {
            MyReader = new AppSettingsReader();
            dbConnection = MyReader.GetValue("Connection", typeof(string)).ToString();
        }
        public bool CheckCampaignAvailability()
        {
            DataSet ds = new DataSet();
            ds = campaignChecker();
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// start campaign threads
        /// </summary>
        public void StartCampaign()
        {
            MailUtility obj = new MailUtility();
                campaigns = new List<Campaigns>();
                campaigns = getCampaigns();

            #region ParallelTask
            try
            {
                Parallel.ForEach(campaigns, (camp) => {
                    obj.SendMessage(camp);
                });
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
          

            #endregion
        }


        /// <summary>
        /// check campaigns available in queue and get all if available
        /// </summary>
        /// <returns></returns>
        private DataSet campaignChecker()
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(dbConnection))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_getcampaignfromqueue", con))
                    {
                        
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        return ds;
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
            return ds;
        }
        /// <summary>
        /// get campaigns from mailer queue
        /// </summary>
        /// <returns></returns>
        private List<Campaigns> getCampaigns()
        {
            //List<Campaigns>   
            campaigns = new List<Campaigns>();  
            DataSet ds = new DataSet();
            ds = campaignChecker();
            CampaignCount = ds.Tables[0].Rows.Count;
            if (ds.Tables[0].Rows.Count > 0)
            {
                Campaigns camp = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    camp = new Campaigns();
                    camp.CampaignID = (int)row["CampaignID"];
                    camp.IdentifierCampaign = row["IdentifierCampaign"].ToString();
                    campaigns.Add(camp);
                }
                return campaigns;
            }
            else
            {
                return null;
            }
        }

        
    }

}
