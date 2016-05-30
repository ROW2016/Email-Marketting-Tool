using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EMT_CampaignService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //#if DEBUG
            //            CampaignChecker s = new CampaignChecker();
            //            s.ondebug();
            //            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            //            #else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CampaignChecker()
            };
            ServiceBase.Run(ServicesToRun);
     //       #endif
        }
    }
}
