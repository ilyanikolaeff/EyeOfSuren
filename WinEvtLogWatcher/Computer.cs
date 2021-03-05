using System;
using System.Net;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class Computer
    {
        public string Ip { get; private set; }
        public string Name
        {
            get; private set;
        }
        public Computer(string ipAddress)
        {
            Ip = ipAddress;
            Name = Dns.GetHostEntryAsync(ipAddress).Result.HostName;
        }

        private async Task<IPHostEntry> GetComputerName(string ipAddress)
        {
            return await Dns.GetHostEntryAsync(ipAddress);
        }
    }
}
