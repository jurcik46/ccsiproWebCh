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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.UI.ViewModels.SettingViewModel;

namespace WebChromiumCcsipro.UI.Views.SettingsWindow
{
    /// <summary>
    /// Interaction logic for ChangePasswordDialogControl.xaml
    /// </summary>
    public partial class ChangePasswordDialogControl : UserControl
    {
        public ChangePasswordDialogControl()
        {
            InitializeComponent();
        }

        private void VizualizePasswordValidation(PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(passwordBox.Password) || passwordBox.Password.Length < 5)
            {
                passwordBox.ToolTip = lang.ChangePasswordWindowPasswordBoxTooltip;
            }
            else
            {
                passwordBox.ToolTip = null;
            }
        }

        private void newPasswrod1PassowrdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChangePasswordViewModel;
            var passwordBox = sender as PasswordBox;
            if (viewModel == null || passwordBox == null)
            {
                return;
            }
            viewModel.Password1 = passwordBox.Password;
            VizualizePasswordValidation(passwordBox);
        }


        private void newPasswrod2PassowrdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChangePasswordViewModel;
            var passwordBox = sender as PasswordBox;
            if (viewModel == null || passwordBox == null)
            {
                return;
            }
            viewModel.Password2 = passwordBox.Password;
            VizualizePasswordValidation(passwordBox);
        }
    }
}
