﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT_EmailHelper
{
    public class EmailDetails
    {

        public  string ToEmail { get; set; }
        public string ToName { get; set; }
        public string EmailSubject { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string EmailBody { get; set; }

    }
}
