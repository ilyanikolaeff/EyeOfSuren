using System;
using System.Diagnostics.Eventing.Reader;
using System.Timers;

namespace WinEvtLogWatcher
{
    class EventLogSubscriber
    {
        public EventLogWatcher LogWatcher { get; set; }
        public Computer Computer { get; private set; }
        public bool IsInitialized { get; set; }
        public string InitializeStatus { get; set; }

        private Timer _checkSubsriberTimer;
        public EventLogSubscriber(Computer computer, bool enabled)
        {
            try
            {
                string queryString = Settings.GetInstance().QueryString;
                Computer = computer;

                EventLogQuery eventLogQuery = new EventLogQuery("Application", PathType.LogName, queryString)
                {
                    Session = new EventLogSession(Computer.Ip)
                };

                LogWatcher = new EventLogWatcher(eventLogQuery)
                {
                    Enabled = enabled
                };

                IsInitialized = true;
                InitializeStatus = "OK";
                StartCheckSubsriberTimer();
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                InitializeStatus = ex.Message;
            }
        }

        public void Start()
        {
            LogWatcher.Enabled = true;
            _checkSubsriberTimer.Enabled = true;
        }

        public void Stop()
        {
            LogWatcher.Enabled = false;
            _checkSubsriberTimer.Enabled = false;
        }

        private void StartCheckSubsriberTimer()
        {
            _checkSubsriberTimer = new Timer(1000);
            _checkSubsriberTimer.Elapsed += _checkSubsriberTimer_Elapsed;
            _checkSubsriberTimer.Enabled = true;
        }

        private void _checkSubsriberTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // попробуем ресубскрайб
            if (!LogWatcher.Enabled)
            {
                LogWatcher.Enabled = true;
            }
        }
    }
}
