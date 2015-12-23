using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;

using ServerTrack.Models;

namespace ServerTrack.Controllers
{
    public class ServerInfoController : ApiController
    {
     
        //A static object that holds ServerLoadInfo in memory.  We need this since we don't store data in DB
        static Dictionary<string, List<ServerLoadInfo>> data = new Dictionary<string,List<ServerLoadInfo>>();
       
        [HttpPost]
        [ActionName("LoadServerLoad")]
        public void LoadServerInfo([FromBody] ServerLoadInfo info)
        {   
            info.UpdatedAt = DateTime.Now;

            if (!data.Keys.Contains(info.ServerName))
            {
                data[info.ServerName] = new List<ServerLoadInfo>() { info };
            }
            else
            {
                data[info.ServerName].Add(info);
            }
           
        }

        [HttpGet]
        [ActionName("RetrieveServerLoadLastHour")]
        public List<ServerLoadInfo> RetrieveServerLoadLastHour(string name)
        {
            return RetrieveServerLoad(RetrieveLoadInterval.HOURLY, name);
        }

        [HttpGet]
        [ActionName("RetrieveServerLoadLastDay")]
        public List<ServerLoadInfo> RetrieveServerLoadLastDay(string name)
        {
            return RetrieveServerLoad(RetrieveLoadInterval.DAILY, name);
        }


        //Heper method that helps retrieve data based on specified input
        private List<ServerLoadInfo> RetrieveServerLoad(RetrieveLoadInterval type, string name)
        {
            if (!data.Keys.Contains(name))
            {
                return null;
            }
            else
            {
                int sample_length_in_minutes = 1;
                int sample_intervals = 60;
                if (type == RetrieveLoadInterval.DAILY)
                {
                    sample_length_in_minutes = 60;
                    sample_intervals = 24;
                }


                List<ServerLoadInfo> output = new List<ServerLoadInfo>();
                DateTime now = DateTime.Now;
                for (int i = 1; i <= sample_intervals; ++i)
                {
                    ServerLoadInfo avg = new ServerLoadInfo() { ServerName = name, CPULoad = 0.0f, RAMLoad = 0.0f };
                    int count = 0;
                    foreach (var info in data[name].Where(sli => sli.UpdatedAt >= now.AddMinutes(-sample_length_in_minutes*i)))
                    {
                        ++count;
                        avg.CPULoad += info.CPULoad;
                        avg.RAMLoad += info.RAMLoad;

                    }
                    avg.CPULoad /= count;
                    avg.RAMLoad /= count;
                    avg.UpdatedAt = now.AddMinutes(-sample_length_in_minutes * i);
                    output.Add(avg);

                }
                return output;
               
            }
        }
       
    }
}
