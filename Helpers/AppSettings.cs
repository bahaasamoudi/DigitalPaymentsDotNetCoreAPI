﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Helpers
{
    public class AppSettings
    {
        public string Site { get; set; }
        public string Audience { get; set; }
        public string ExpireTime { get; set; }
        public string Secret { get; set; }


        // Sendgrind
        public string SendGridKey { get; set; }
        public string SendGridUser { get; set; }
    }
}
