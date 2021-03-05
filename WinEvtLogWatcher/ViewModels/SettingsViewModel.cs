using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WinEvtLogWatcher.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        #region Properties

        #region Colors
        public Color CriticalBackgroundColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Critical").BackgroundColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Critical").BackgroundColor = value; }
        }
        public Color CriticalFontColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Critical").FontColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Critical").FontColor = value; }
        }
        public Color ErrorBackgroundColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Error").BackgroundColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Error").BackgroundColor = value; }
        }
        public Color ErrorFontColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Error").FontColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Error").FontColor = value; }
        }
        public Color WarningBackgroundColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Warning").BackgroundColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Warning").BackgroundColor = value; }
        }
        public Color WarningFontColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Warning").FontColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Warning").FontColor = value; }
        }
        public Color InfoBackgroundColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Informational").BackgroundColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Informational").BackgroundColor = value; }
        }
        public Color InfoFontColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Informational").FontColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Informational").FontColor = value; }
        }
        public Color VerboseBackgroundColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Verbose").BackgroundColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Verbose").BackgroundColor = value; }
        }
        public Color VerboseFontColor
        {
            get { return Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Verbose").FontColor; }
            set { Settings.GetInstance().DefinedColorSettings.FirstOrDefault(p => p.Name == "Verbose").FontColor = value; }
        }
        #endregion

        public int MaxDisplayEvents
        {
            get { return Settings.GetInstance().MaxEventsToDisplay; }
            set { Settings.GetInstance().MaxEventsToDisplay = value; }
        }
        public int PeriodDisplayEvents
        {
            get { return Settings.GetInstance().PeriodToDisplayEvents; }
            set { Settings.GetInstance().PeriodToDisplayEvents = value; }
        }
        public bool UseDarkTheme
        {
            get { return Settings.GetInstance().UseDarkTheme; }
            set
            {
                if (!value)
                    ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019ColorfulName;
                else
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;

                Settings.GetInstance().UseDarkTheme = value;
            }
        }

        #region Sounds files
        public string CriticalSoundFile
        {
            get { return Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Critical").FileName; }
            set { Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Critical").FileName = value; RaisePropertyChanged(); }
        }
        public string ErrorSoundFile
        {
            get { return Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Error").FileName; }
            set { Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Error").FileName = value; RaisePropertyChanged(); }
        }
        public string WarningSoundFile
        {
            get { return Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Warning").FileName; }
            set { Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Warning").FileName = value; RaisePropertyChanged(); }
        }
        public string InfoSoundFile
        {
            get { return Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Informational").FileName; }
            set { Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Informational").FileName = value; RaisePropertyChanged(); }
        }
        public string VerboseSoundFile
        {
            get { return Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Verbose").FileName; }
            set { Settings.GetInstance().SoundsFiles.FirstOrDefault(p => p.Name == "Verbose").FileName = value; RaisePropertyChanged(); }
        }
        #endregion

        #endregion

        readonly IDialogService dialogService = new DefaultDialogService();

        public SettingsViewModel()
        {
            TestPlaySoundCommand = new DelegateCommand<string>(
                (fileName) =>
                    {
                        var soundPlayer = new SoundPlayer(fileName);
                        soundPlayer.Play();
                    },
                (fileName) =>
                    {
                        return File.Exists(fileName);
                    }
                );

            OpenSoundFileCommand = new DelegateCommand<object>(
                (level) =>
                    {
                        if (dialogService.OpenFileDialog() == true)
                        {
                            var intValue = Convert.ToInt32(level);
                            var fileName = dialogService.FilePath;
                            if (intValue == 1)
                                CriticalSoundFile = fileName;
                            else if (intValue == 2)
                                ErrorSoundFile = fileName;
                            else if (intValue == 3)
                                WarningSoundFile = fileName;
                            else if (intValue == 4)
                                InfoSoundFile = fileName;
                            else if (intValue == 5)
                                VerboseSoundFile = fileName;
                        }
                    }
                );
        }
        
        #region Commands
        public ICommand TestPlaySoundCommand { get; private set; }

        public ICommand OpenSoundFileCommand { get; private set; }
        #endregion
    }
}
