using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class ExportHelper
    {
        public static void Export(IEnumerable<WatcherEventRecord> events, string fileName, bool append = false, string userName = null)
        {
            using (var outputFile = new StreamWriter(fileName, append, Encoding.Default))
            {
                if (userName != null)
                {
                    outputFile.WriteLine($"Пользователь: {userName}");
                }

                // header 
                outputFile.WriteLine("Время события\tИмя компьютера\tИсточник события\tЖурнал" +
                    "\tОписание события\tУровень события\tГруппа события");

                string separator = fileName.EndsWith(".csv") ? ";" : "\t";

                if (events != null)
                {
                    foreach (var evt in events)
                    {
                        if (evt != null)
                        {
                            // если хоть одно из нужных свойств 
                            if (evt.TimeCreated == null || evt.Level == null)
                                continue;

                            outputFile.WriteLine($"{evt.TimeCreated.Value:dd.MM.yyyy HH:mm:ss}{separator}" +
                                $"{evt.MachineName}{separator}" +
                                $"{evt.ProviderName}{separator}" +
                                $"{evt.LogName}{separator}" +
                                $"{evt.Description}{separator}" +
                                $"{evt?.Level}{separator}" +
                                $"{(StandardEventLevel)evt?.Level}");
                        }
                    }
                }
            }
        }

        public static async void ExportWithAckAsync(IEnumerable<WatcherEventRecord> events, User ackUser)
        {
            await Task.Run(() =>
            {
                string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\ACK_EVENTS\";
                if (!Directory.Exists(pathName))
                    Directory.CreateDirectory(pathName);

                string fileName = pathName + $"{DateTime.Now:dd_MM_yy HH_mm_ss} ACK_EVENTS.txt";
                string userName = ackUser is null ? "Незарегистрированный" : ackUser.Name;

                Export(events, fileName, false, userName);
            });
        }

        public static async void ExportWhenClearAsync(IEnumerable<WatcherEventRecord> events)
        {
            await Task.Run(() =>
            {
                string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\CLEARED_EVENTS\";
                if (!Directory.Exists(pathName))
                    Directory.CreateDirectory(pathName);

                string fileName = pathName + $"{DateTime.Now:dd_MM_yy HH_mm_ss} CLEARED_EVENTS.txt";

                Export(events, fileName, false);
            });
        }

        public enum ClearType
        {
            Time,
            Count
        }
    }
}
