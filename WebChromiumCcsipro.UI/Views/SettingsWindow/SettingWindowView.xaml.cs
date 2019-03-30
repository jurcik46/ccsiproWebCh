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
using WebChromiumCcsipro.Domain.Interfaces;

namespace WebChromiumCcsipro.UI.Views.SettingsWindow
{
    /// <summary>
    /// Interaction logic for SettingWindowView.xaml
    /// </summary>
    public partial class SettingWindowView : Window, IClosable
    {
        public SettingWindowView()
        {
            InitializeComponent();
        }
    }
}
