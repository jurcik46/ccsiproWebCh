using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WebChromiumCcsipro.Controls.Interfaces.IServices
{
    public interface IDialogService : GalaSoft.MvvmLight.Views.IDialogService
    {

        Window Owner { get; set; }
    }
}
