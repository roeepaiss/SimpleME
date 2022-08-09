using Squirrel;
using System;
using System.Collections.Generic;
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
        volatile int progressPrecentage;
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
                this.updateManager = new Squirrel.GithubUpdateManager(@"https://github.com/roeepaiss/SimpleME");
                var updateInfo = await updateManager.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    UpdateButton.IsEnabled = true;
                }
                else
                {
                    UpdateButton.IsEnabled = false;
                }
            }
            catch(Exception ex)
            {
                throw ex;
                MessageBox.Show("Try again later.");
                this.Close();
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateButton.IsEnabled = false;
            try
            {
                await updateManager.UpdateApp(progress =>
                {
                    progressPrecentage = progress;
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
