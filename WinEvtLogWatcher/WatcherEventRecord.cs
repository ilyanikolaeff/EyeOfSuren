using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class WatcherEventRecord
    {
        public DateTime? TimeCreated { get; set; }
        public string MachineName { get; set; }
        public string ProviderName { get; set; }
        public string LogName { get; set; }
        public string Description { get; set; }
        public byte? Level { get; set; }
        public StandardEventLevel LevelGroup { get; set; }

        public WatcherEventRecord(EventRecord eventRecord)
        {
            TimeCreated = eventRecord.TimeCreated;
            MachineName = eventRecord.MachineName;
            ProviderName = eventRecord.ProviderName;
            LogName = eventRecord.LogName;
            Description = string.Join("\n", ((List<EventProperty>)eventRecord.Properties).Select(s => s.Value));
            Level = eventRecord.Level;
            LevelGroup = (StandardEventLevel)eventRecord.Level;
        }
    }
}
