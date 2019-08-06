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
    /// Interaction logic for EnterSettingWindowView.xaml
    /// </summary>
    public partial class EnterSettingWindowView : Window
    {
        public EnterSettingWindowView()
        {
            InitializeComponent();
            PasswordBox.Focus();
        }

        private void optionsPassowrdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EnterSettingViewModel;
            var passwordBox = sender as PasswordBox;
            if (viewModel == null || passwordBox == null)
            {
                return;
            }

            viewModel.Password = passwordBox.Password;
            //            VizualizePasswordValidation(passwordBox);

        }

        private void VizualizePasswordValidation(PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.BorderThickness = new Thickness(3);
                passwordBox.ToolTip = "Heslo nemôže byť prázdne alebo obsahovať medzery!";
                passwordBox.Background = Brushes.Red;
                enterPasswordButton.IsEnabled = false;
            }
            else
            {
                passwordBox.BorderBrush = Brushes.Black;
                passwordBox.BorderThickness = new Thickness(1);
                passwordBox.ToolTip = null;
                passwordBox.Background = Brushes.White;
                enterPasswordButton.IsEnabled = true;

            }
        }
    }
}
