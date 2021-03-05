using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WinEvtLogWatcher
{
    class Logger
    {
        private object _locker = new object();
        public void Log(string message)
        {
            lock (_locker)
            {
                using (var logFile = new StreamWriter("TempLog.txt", true))
                {
                    logFile.WriteLine($"{DateTime.Now}\t{message}");
                }
            }
        }
    }
}
