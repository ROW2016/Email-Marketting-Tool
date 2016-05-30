using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EMT_Utility;
using System.IO;
using EMT_Exception;

namespace EMT_CampaignService
{
    partial class CampaignChecker : ServiceBase
    {

        private CancellationTokenSource cts = new CancellationTokenSource();
        private TimeSpan WaitAfterErrorInterval;
        private TimeSpan WaitAfterSuccessInterval;
        private TimeSpan WaitForCampaign;
        
        //private GetSubscriberIdsFromDB obj = new GetSubscriberIdsFromDB();
        private Task mainTask = null;

        public CampaignChecker()
        {
             InitializeComponent();
        }
        public void ondebug()
        {
            OnStart(null);
        }
        /// <summary>
        /// Initiate the service and check for campaigns 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            WaitAfterErrorInterval = TimeSpan.FromMinutes(5);
            WaitAfterSuccessInterval = TimeSpan.FromMinutes(1);
            WaitForCampaign = TimeSpan.Zero;
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "LogsForService.txt"))
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "LogsForService.txt");
            }

            using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "LogsForService.txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("Service Started : " + DateTime.Now);
            }

            mainTask = new Task(InitiateCampaign, cts.Token, TaskCreationOptions.LongRunning);
            mainTask.Start();
            Trace.Write("Service Started : " + DateTime.Now);
        }

        protected override void OnStop()
        {
            using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "LogsForService.txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("Service Stopped : " + DateTime.Now);
            }
            cts.Cancel();
            mainTask.Wait();
            Trace.Write("Service Stopped     : " + DateTime.Now);
        }
        /// <summary>
        /// Check for campaigns and start them
        /// </summary>
        private void InitiateCampaign()
        {
            CancellationToken cancellation = cts.Token;      
            TimeSpan interval = TimeSpan.Zero;
            CampaignManager camp = new CampaignManager();
            while (!cancellation.WaitHandle.WaitOne(interval))
            {
                try
                {
                    if (camp.CheckCampaignAvailability())
                    {
                        camp.StartCampaign();
                        using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "LogsForService.txt", FileMode.Append, FileAccess.Write))
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine("Success Campaign Successfuly Send  : " + DateTime.Now);
                        }
                        interval = WaitAfterSuccessInterval;                        
                    }
                    else
                    {
                        interval = WaitForCampaign;
                    }

                    if (cancellation.IsCancellationRequested)
                    {
                        Trace.Write("Service Cancelled     : " + DateTime.Now);
                        OnStop();
                    }
                }
                catch (Exception caught)
                {
                    // Log the exception.
                    EMTException ex = new EMTException((int)EMTException.ErorrTypes.SqlExceptions, caught.Message, caught.StackTrace, EMTException.ErorrTypes.SMTPExceptions.ToString());
                    ex.LogException();
                    interval = WaitAfterErrorInterval;
                    Trace.Write("Service Failure  : " + DateTime.Now);
                    Trace.Write("Failure Reason   : " + caught.InnerException.Message);
                    using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "LogsForService.txt", FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine("Service Failure  : " + DateTime.Now);
                        sw.WriteLine("Failure Reason   : " + caught.Message);
                    }
                }
            }
        }
    }
}
