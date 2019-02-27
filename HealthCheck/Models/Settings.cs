using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheck.Models
{
    public class Settings
    {
        public string Url { get; set; }
        public string ValidationText { get; set; }
        public int Retries { get; set; }
        public long Timeout { get; set; }
        public bool ForceOnline { get; set; }
        public bool RequireValidSSL { get; set; }
    }
}
