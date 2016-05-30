using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT_EmailHelper
{
    class CampaignDetails
    {
        public int Cid { get; set; }
        public int CTypeId { get; set; }
        public string Name { get; set; }
        public int ListId { get; set; }
        public string EmailSubject { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string EmailContent { get; set; }
        public int ScheduleCampaign { get; set; }
        public int noOfEmailSendPerInterval { get; set; }
    }

  
}
