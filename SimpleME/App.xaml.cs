using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SimpleME.Classes;
using Squirrel;

namespace SimpleME
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            // run Squirrel first, as the app may exit after these run
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: OnAppInstall,
                onAppUninstall: OnAppUninstall,
                onAppUpdate: OnAppUpdate,
                onEveryRun: OnAppRun);
            // ... other app init code after ...
        }

        private static void OnAppInstall(SemanticVersion version, IAppTools tools)
        {
            //tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
            tools.CreateShortcutForThisExe();
            Environment.Exit(0);
        }

        private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
        {
            //tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
            tools.RemoveShortcutForThisExe();
            Environment.Exit(0);
        }

        private void OnAppUpdate(SemanticVersion version, IAppTools tools)
        {
            Environment.Exit(0);
        }

        private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
        {
            tools.SetProcessAppUserModelId();
            if (firstRun)
            {
                //// show a welcome message when the app is first installed
                //MessageBox.Show("Thanks for installing my application!");
            }
        }
    }
}
