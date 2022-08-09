using SimpleME.Classes;
using Squirrel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleME.Resources
{
    /// <summary>
    /// Interaction logic for UpdateManager.xaml
    /// </summary>
    public partial class UpdateManagerWindow : Window
    {
        private ProgressState state;
        private UpdateManager updateManager;
        public UpdateManagerWindow()
        {
            InitializeComponent();
            this.Loaded += UpdateManagerWindow_Loaded;
            this.Closing += UpdateManagerWindow_Closing;
        }

        private void UpdateManagerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.updateManager.Dispose();
        }

        private async void UpdateManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                updateManager = new GithubUpdateManager(@"https://github.com/roeepaiss/SimpleME");
                var updateInfo = await updateManager.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                    UpdateButton.IsEnabled = true;
            }
            catch(Exception ex)
            {
                LabelState.Content = "Try again later.";
                throw ex;
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            state = new ProgressState();
            state.Progress += State_Progress;
            UpdateButton.IsEnabled = false;
            LabelState.Content = "Updating...0%";
            var newVersion = await updateManager.UpdateApp(progress =>
            {
                state.Value = progress;
            });
            // You must restart to complete the update. 
            // This can be done later / at any time.
            if (newVersion != null) UpdateManager.RestartApp();
        }

        private void State_Progress(object sender, ProgressChangedEventArgs e)
        {
            LabelState.Content = string.Format("Updating...{0}%", e.ProgressPercentage);
            ProgBar.Value = e.ProgressPercentage;
        }
    }
}
