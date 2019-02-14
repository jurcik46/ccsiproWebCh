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
using WebChromiumCcsipro.UI.ViewModels.SettingViewModel;

namespace WebChromiumCcsipro.UI.Views.SettingsWindow
{
    /// <summary>
    /// Interaction logic for ApplicationSettingWindowView.xaml
    /// </summary>
    public partial class ApplicationSettingWindowView : Window
    {
        public ApplicationSettingWindowView()
        {
            InitializeComponent();
        }

        private void PasswordTextBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ApplicationSettingViewModel;
            var passwordBox = sender as PasswordBox;
            if (viewModel == null || passwordBox == null)
            {
                return;
            }

            viewModel.Password = passwordBox.Password;

        }
    }
}
