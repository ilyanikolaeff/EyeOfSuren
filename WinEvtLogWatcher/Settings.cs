using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;

namespace WinEvtLogWatcher
{
    class Settings
    {
        // singleton
        private static Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());
        public static Settings GetInstance()
        {
            return _instance.Value;
        }

        // settings
        public string QueryString { get; set; }
        public List<Computer> Computers { get; set; }
        public int MaxEventsToDisplay { get; set; } = 1000;
        public bool AutoSubscribeOnStartup { get; set; } = false;
        public int PeriodToDisplayEvents { get; set; } = 5;
        public bool UseDarkTheme { get; set; } = false;
        public List<EventSound> Sounds { get; set; }
        public List<User> Users { get; set; }
        public List<SoundFile> SoundsFiles { get; set; } = new List<SoundFile>()
        {
            new SoundFile() { Name = "Critical", FileName = @"Sounds\Critical.wav" },
            new SoundFile() { Name = "Error", FileName = @"Sounds\Error.wav" },
            new SoundFile() { Name = "Warning", FileName = @"Sounds\Warning.wav" },
            new SoundFile() { Name = "Informational", FileName = @"Sounds\Info.wav" },
            new SoundFile() { Name = "Verbose", FileName = @"Sounds\Verbose.wav" }
        };

        public List<ColorSettings> DefinedColorSettings { get; set; } = new List<ColorSettings>()
        {
            new ColorSettings() { Name = "Critical", FontColor = Color.FromRgb(255, 255, 0), BackgroundColor = Color.FromRgb(255, 0, 0) },
            new ColorSettings() { Name = "Error", FontColor = Color.FromRgb(0, 0, 0), BackgroundColor = Color.FromRgb(255, 0, 0) },
            new ColorSettings() { Name = "Warning", FontColor = Color.FromRgb(0, 0, 0), BackgroundColor = Color.FromRgb(255, 255, 0) },
            new ColorSettings() { Name = "Informational", FontColor = Color.FromRgb(0, 0, 0), BackgroundColor = Color.FromRgb(255, 255, 255) },
            new ColorSettings() { Name = "Verbose", FontColor = Color.FromRgb(0, 0, 0), BackgroundColor = Color.FromRgb(255, 255, 255) },
        };

        public event EventHandler SettingsChanged;
        private Settings()
        {
            SettingsChanged += Settings_SettingsChanged;
            Load();
        }

        private void Settings_SettingsChanged(object sender, EventArgs e)
        {
            Save();
        }
        private void Load()
        {
            var xRoot = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "Settings.xml").Root;

            // Settings
            QueryString = xRoot.Element("QueryString").Attribute("Value").Value;
            MaxEventsToDisplay = int.Parse(xRoot.Element("MaxEventsToDisplay").Attribute("Value").Value);
            PeriodToDisplayEvents = int.Parse(xRoot.Element("PeriodToDisplayEvents").Attribute("Value").Value);
            AutoSubscribeOnStartup = bool.Parse(xRoot.Element("AutoSubscribeOnStartup").Attribute("Value").Value);

            var xUseDarkTheme = xRoot.Element("UseDarkTheme");
            if (xUseDarkTheme != null)
                UseDarkTheme = bool.Parse(xUseDarkTheme.Attribute("Value").Value);

            // computers
            Computers = new List<Computer>();
            foreach (var xComp in xRoot.Element("Computers").Elements("Computer"))
                Computers.Add(new Computer(xComp.Attribute("Ip").Value));

            // sounds
            var xSounds = xRoot.Element("Sounds")?.Elements("SoundFile");
            if (xSounds != null)
            {
                SoundsFiles = new List<SoundFile>();
                foreach (var xSound in xSounds)
                {
                    var name = xSound.Attribute("Name").Value;
                    var fileName = xSound.Attribute("FileName").Value;
                    SoundsFiles.Add(new SoundFile() { Name = name, FileName = fileName });
                }
            }

            // users
            var xUsers = xRoot.Element("Users")?.Elements("User");
            if (xUsers != null)
            {
                Users = new List<User>();
                foreach (var xUser in xUsers)
                {
                    Users.Add(new User()
                    {
                        Name = xUser.Attribute("Name").Value,
                        DisplayName = xUser.Attribute("DisplayName").Value
                    });
                }
            }

            // colors 
            var xColors = xRoot.Element("Colors")?.Elements("Color");
            if (xColors != null)
            {
                foreach (var xColor in xColors)
                {
                    var name = xColor.Attribute("Name").Value;
                    var color = DefinedColorSettings.FirstOrDefault(p => p.Name == name);
                    color.FontColor = (Color)ColorConverter.ConvertFromString(xColor.Attribute("FontColor").Value);
                    color.BackgroundColor = (Color)ColorConverter.ConvertFromString(xColor.Attribute("BackgroundColor").Value);
                }
            }
        }
        public void Save()
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "Settings.xml";

            var xDoc = XDocument.Load(filePath);
            var xRoot = xDoc.Root;
            
            // изменяемые настройки
            xRoot.Element("MaxEventsToDisplay").SetAttributeValue("Value", MaxEventsToDisplay);
            xRoot.Element("PeriodToDisplayEvents").SetAttributeValue("Value", PeriodToDisplayEvents);
            xRoot.Element("UseDarkTheme").SetAttributeValue("Value", UseDarkTheme);
            
            // computers
            xRoot.Element("Computers").DescendantNodes().Remove();
            foreach (var computer in Computers)
            {
                var xComp = new XElement("Computer");
                xComp.SetAttributeValue("Ip", computer.Ip);
                xRoot.Element("Computers").Add(xComp);
            }
            
            // colors
            var xColors = xRoot.Element("Colors");
            if (xColors != null)
                xColors.DescendantNodes().Remove();
            else
            {
                xColors = new XElement("Colors");
                xRoot.Add(xColors);
            }
            foreach (var color in DefinedColorSettings)
                xColors.Add(color.ToXElement());

            // sounds
            var xSounds = xRoot.Element("Sounds");
            if (xSounds != null)
                xSounds.DescendantNodes().Remove();
            else
            {
                xSounds = new XElement("Sounds");
                xRoot.Add(xSounds);
            }
            foreach (var soundFile in SoundsFiles)
                xSounds.Add(soundFile.ToXElement());

            xDoc.Save(filePath);
        }
        public void RestoreDefault()
        {
            var settingsFile = GetResourceSettings();

            if (File.Exists("Settings.xml"))
                File.Move("Settings.xml", "Settings_backup.xml");

            File.WriteAllText("Settings.xml", settingsFile);

            Load();
        }
        private string GetResourceSettings()
        {
            string result = string.Empty;
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream($"WinEvtLogWatcher.Settings.xml"))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        public class ColorSettings
        {
            public string Name { get; set; }
            public Color BackgroundColor { get; set; }
            public Color FontColor { get; set; }

            public XElement ToXElement()
            {
                var xColor = new XElement("Color");
                xColor.SetAttributeValue("Name", Name);
                xColor.SetAttributeValue("BackgroundColor", BackgroundColor);
                xColor.SetAttributeValue("FontColor", FontColor);
                return xColor;
            }
        }

        public class SoundFile
        {
            public string Name { get; set; }
            public string FileName { get; set; }

            public XElement ToXElement()
            {
                var xSoundFile = new XElement("SoundFile");
                xSoundFile.SetAttributeValue("Name", Name);
                xSoundFile.SetAttributeValue("FileName", FileName);
                return xSoundFile;
            }
        }
    }
}
