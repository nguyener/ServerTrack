using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

namespace ServerTrack.Models
{
    public class ServerLoadInfo
    {
        public string ServerName { get; set; }
        public float CPULoad { get; set; }
        public float RAMLoad { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public enum RetrieveLoadInterval { HOURLY, DAILY}; 

   
}