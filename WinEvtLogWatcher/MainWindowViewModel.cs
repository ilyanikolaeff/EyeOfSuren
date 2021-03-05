using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WinEvtLogWatcher
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Commands
        public ICommand ClearCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand AddComputerCommand { get; private set; }
        public ICommand RemoveComputerCommand { get; private set; }
        public ICommand ShowAboutWindowCommand { get; private set; }
        public ICommand RestoreCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand ActivateMainWindowCommand { get; private set; }
        public ICommand SaveSettingsCommand { get; private set; }
        #endregion

        #region Properties
        public ObservableCollection<WatcherEventRecord> Events { get; set; } = new ObservableCollection<WatcherEventRecord>();
        public ObservableCollection<EventLogSubscriber> LogSubscribers { get; set; } = new ObservableCollection<EventLogSubscriber>();
        public bool SubscribeStatus
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public bool SoundsActive
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public WatcherEventRecord CurrentItem
        {
            get { return GetValue<WatcherEventRecord>(); }
            set { SetValue(value); }
        }
        public EventLogSubscriber SelectedLogSubscriber
        {
            get { return GetValue<EventLogSubscriber>(); }
            set { SetValue(value); }
        }
        public List<User> Users
        {
            get { return _currentSettings.Users; }
        }
        public User CurrentUser
        {
            get { return GetValue<User>(); }
            set { SetValue(value); }
        }


        #endregion

        #region Fields
        readonly IDialogService dialogService = new DefaultDialogService();
        private System.Timers.Timer _checkEventsTimer;
        private SoundQueue _watcherSoundQueue;
        private Settings _currentSettings;
        private Logger _logger = new Logger();
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get { return GetService<ICurrentWindowService>(); } }
        protected INotifyIconService NotifyIconService { get { return GetService<INotifyIconService>(); } }
        #endregion


        public MainWindowViewModel()
        {
            Initialize();
            SetCommands();
        }

        /// <summary>
        /// Присваивание действий командам
        /// </summary>
        private void SetCommands()
        {
            // ack all 
            ClearCommand = new DelegateCommand(
                () =>
                {
                    var exportEvents = new List<WatcherEventRecord>(Events);
                    ExportHelper.ExportWithAckAsync(exportEvents, CurrentUser);
                    Events.Clear();
                },
                () => Events.Count > 0);

            // save command 
            SaveCommand = new DelegateCommand(
                () =>
                {
                    if (dialogService.SaveFileDialog() == true)
                    {
                        ExportHelper.Export(Events, dialogService.FilePath);
                        dialogService.ShowMessage($"Файл: {dialogService.FilePath} сохранен!");
                    }
                },
                () => Events.Count > 0);

            // stop subscribe
            StopCommand = new DelegateCommand(
                () => { OnSubscribeStatusChanged(false); },
                () => SubscribeStatus);

            // start subsribe
            StartCommand = new DelegateCommand(
                () => OnSubscribeStatusChanged(true),
                () => !SubscribeStatus);

            // add computer command
            AddComputerCommand = new DelegateCommand(
                () =>
                {
                    var addComputerDialog = new AddComputerDialog();
                    if (addComputerDialog.ShowDialog() == true)
                    {
                        var computer = new Computer(addComputerDialog.IpAddress);
                        var logSubscriber = new EventLogSubscriber(computer, SubscribeStatus);
                        LogSubscribers.Add(logSubscriber);
                        Subscribe(logSubscriber);
                        _currentSettings.Computers.Add(computer);
                    }
                });

            // remove computer command
            RemoveComputerCommand = new DelegateCommand(
                () =>
                {
                    if (SelectedLogSubscriber != null)
                    {
                        _currentSettings.Computers.Remove(SelectedLogSubscriber.Computer);
                        Unsubscribe(SelectedLogSubscriber);
                        LogSubscribers.Remove(SelectedLogSubscriber);
                    }
                },
                () => SelectedLogSubscriber != null);

            ShowAboutWindowCommand = new DelegateCommand(
                () =>
                {
                    var aboutWind = new AboutWindow();
                    aboutWind.ShowDialog();
                });

            RestoreCommand = new DelegateCommand(
                () =>
                {
                    CurrentWindowService.SetWindowState(WindowState.Normal);
                    CurrentWindowService.Activate();
                }
                );

            CloseCommand = new DelegateCommand(
                () =>
                {
                    CurrentWindowService.Close();
                });
        }

        /// <summary>
        /// Обработка изменения состояния подписки (включена / отключена)
        /// </summary>
        /// <param name="subscribeStatus"></param>
        private void OnSubscribeStatusChanged(bool subscribeStatus)
        {
            foreach (var logSub in LogSubscribers)
            {
                logSub.LogWatcher.Enabled = subscribeStatus;
            }
            SubscribeStatus = subscribeStatus;
            if (_checkEventsTimer != null)
                _checkEventsTimer.Enabled = subscribeStatus;
        }

        /// <summary>
        /// Инициализация VM
        /// </summary>
        private void Initialize()
        {
            _currentSettings = Settings.GetInstance();

            SubscribeStatus = _currentSettings.AutoSubscribeOnStartup;
            SoundsActive = _currentSettings.AutoSubscribeOnStartup;

            // users
            InitializeInfinitySecurity();
            // sub
            InitializeLogSubscribers();
            // initialize sound player
            InitializeWatcherSoundPlayer();
            // timer to check events in journal
            StartCheckEventsTimer();
        }

        /// <summary>
        /// Инициализация InfinitySecurity
        /// </summary>
        private void InitializeInfinitySecurity()
        {
            // try infinity security
            //try
            //{
            //    _infinitySecurity = new Security();
            //}
            //catch (Exception ex)
            //{
            //    DXMessageBox.Show($"Ошибка инициализации Infinity Security\n" +
            //        $"{ex}");
            //}
        }
        /// <summary>
        /// Инициализация подписчиков на журнал событий
        /// </summary>
        private void InitializeLogSubscribers()
        {
            // computers
            foreach (var computer in _currentSettings.Computers)
            {
                var currentLogger = new EventLogSubscriber(computer, SubscribeStatus);
                if (!currentLogger.IsInitialized)
                { }
                Subscribe(currentLogger);
                LogSubscribers.Add(currentLogger);
            }
        }

        private void Subscribe(EventLogSubscriber eventLogSubscriber)
        {
            if (eventLogSubscriber != null)
            {
                if (eventLogSubscriber.IsInitialized)
                {
                    eventLogSubscriber.LogWatcher.EventRecordWritten += LogWatcher_EventRecordWritten;
                }
            }
        }

        private void Unsubscribe(EventLogSubscriber eventLogSubscriber)
        {
            if (eventLogSubscriber != null)
            {
                if (eventLogSubscriber.IsInitialized)
                {
                    eventLogSubscriber.LogWatcher.EventRecordWritten -= LogWatcher_EventRecordWritten;
                }
            }
        }
        /// <summary>
        /// Инициализация таймера для проверки времени нахождения событий в гриде
        /// </summary>
        private void StartCheckEventsTimer()
        {
            if (_currentSettings.PeriodToDisplayEvents != 0)
            {
                SetCheckEventsTimerInterval(_currentSettings.PeriodToDisplayEvents);
            }
        }
        private void SetCheckEventsTimerInterval(double interval)
        {
            if (_checkEventsTimer == null)
            {
                _checkEventsTimer = new System.Timers.Timer(TimeSpan.FromMinutes(interval).TotalMilliseconds)
                {
                    AutoReset = true,
                    Enabled = true
                };
                _checkEventsTimer.Elapsed += CheckEventsTimer_Elapsed;
            }
            else
            {
                if (interval > 0)
                {
                    _checkEventsTimer.Interval = TimeSpan.FromMinutes(interval).TotalMilliseconds;
                    _checkEventsTimer.Enabled = true;
                }
                else
                {
                    _checkEventsTimer.Enabled = false;
                }
            }
        }
        private void InitializeWatcherSoundPlayer()
        {
            _watcherSoundQueue = new SoundQueue();
        }
        private void CheckEventsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ClearEvents();
            });
        }
        private void LogWatcher_EventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {
            try
            {
                if (e.EventRecord != null)
                {
                    AddEvent(e.EventRecord);
                }
            }
            catch (Exception ex)
            {
                _logger.Log("Ошибка добавления события ->");
                _logger.Log(ex.ToString());
                _logger.Log("XML описание события ->");
                _logger.Log(e.EventRecord.ToXml());
            }
        }
        private void AddEvent(EventRecord eventRecord)
        {
            if (Application.Current != null)
            {
                var watcherEventRecord = new WatcherEventRecord(eventRecord);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Если превысили количество, то очищаем все, потом только добавляем
                    if (Events.Count >= _currentSettings.MaxEventsToDisplay && _currentSettings.MaxEventsToDisplay > 0)
                        ClearEvents();

                    Events.Add(watcherEventRecord);

                    if (SoundsActive)
                    {
                        var expressionEval = GetExpressionEvaluator(out CriteriaOperator filterCriteria);
                        if (expressionEval != null && !(filterCriteria is null))
                        {
                            //var test = expressionEval.Evaluate(watcherEventRecord);
                            bool evaluateResult = Convert.ToBoolean(expressionEval.Evaluate(watcherEventRecord));
                            if (evaluateResult)
                                _watcherSoundQueue.Add(Convert.ToInt32(watcherEventRecord.Level));
                        }
                        else
                        {
                            _watcherSoundQueue.Add(Convert.ToInt32(watcherEventRecord.Level));
                        }
                    }
                });
                CurrentItem = watcherEventRecord;
            }
        }
        private ExpressionEvaluator GetExpressionEvaluator(out CriteriaOperator filterCriteria)
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                filterCriteria = (Application.Current.MainWindow as MainWindow).eventsGridControl.FilterCriteria;
                return new ExpressionEvaluator(new EvaluatorContextDescriptorDefault(typeof(WatcherEventRecord)),
                    (Application.Current.MainWindow as MainWindow).eventsGridControl.FilterCriteria);
            }
            else
            {
                filterCriteria = null;
                return null;
            }
        }
        private void ClearEvents()
        {
            if (Events.Count > 0)
            {
                ExportHelper.ExportWhenClearAsync(new List<WatcherEventRecord>(Events));
                Events.Clear();
            }
        }
    }
}
