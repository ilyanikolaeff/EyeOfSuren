using DevExpress.Xpf.Core;
using System.Windows;
using System.Reflection;
using System;
using System.IO;
using System.Text;

namespace WinEvtLogWatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            //AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            try
            {
                if (Settings.GetInstance().UseDarkTheme)
                    ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;
                else
                    ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019ColorfulName;
            }
            catch
            {
                if (DXMessageBox.Show($"Ошибка инициализации приложения\nПопробовать восстановить до заводских настроек?",
                                                "Ошибка", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    Settings.GetInstance().RestoreDefault();
                else
                    Environment.Exit(0);
            }


            base.OnStartup(e);
            DXSplashScreen.Show<SplashScreenView>();
            DXSplashScreen.SetState("Выполняется загрузка приложения...");
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Write($"Assembly loaded: {args.LoadedAssembly.FullName}");
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string asmLocation = Assembly.GetExecutingAssembly().Location;
            string asmName = args.Name.Substring(0, args.Name.IndexOf(','));
            string fileName = Path.Combine(asmLocation, asmName);

            if (File.Exists(fileName))
            {
                Write($"FILE EXIST => {fileName}");
                return Assembly.LoadFrom(fileName);
            }
            else
            {
                Write($"FILE DOES NOT EXIST => {fileName}");
                return null;
            }
        }

        private void Write(string data)
        {
            using (var strWriter = new StreamWriter("log.txt", true, Encoding.Default))
            {
                strWriter.WriteLine(data);
            }
        }
    }
}
