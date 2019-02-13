﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Resources.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Language;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ApplicationSettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<ApplicationSettingViewModel>();
        public RelayCommand SaveCommand { get; set; }

        private ISettingsService SettingsService { get; set; }


        public string ObjectId { get; set; }
        public string UserId { get; set; }
        public string HomePage { get; set; }
        public string SelectedLanguage { get; set; }
        public ObservableCollection<string> Language { get; set; }

        public Action CloseAction { get; set; }


        public ApplicationSettingViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;
            ObjectId = SettingsService.ObjectId;
            UserId = SettingsService.UserId;
            HomePage = SettingsService.HomePage;
            SelectedLanguage = SettingsService.Language;
            SaveCommand = new RelayCommand(Save, CanSave);
            Language = new ObservableCollection<string>();
            foreach (var lang in LanguageSource.GetValues())
            {
                Language.Add(lang.Value);
            }

            //TODO allowed web site
        }



        private bool CanSave()
        {
            return true;
        }

        private void Save()
        {
            foreach (var lang in LanguageSource.GetValues())
            {
                //                if (SelectedLanguage == lang.Value)
                //                {
                SettingsService.ChromiumSettingSave(ObjectId, UserId, HomePage, lang.Key);
                //TODO add language
                //                }
            }
            if (CloseAction != null)
            {
                CloseAction();
            }
        }
    }
}
