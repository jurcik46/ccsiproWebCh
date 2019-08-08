using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Language;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ApplicationSettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<ApplicationSettingViewModel>();
        public RelayCommand SaveCommand { get; set; }

        private ISettingsService SettingsService { get; set; }
        private string _selectedLanguage;

        public int ObjectId { get; set; }
        public int UserId { get; set; }
        public string HomePage { get; set; }

        public string ReloadTime { get; set; }


        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> Language { get; set; }

        public Action CloseAction { get; set; }

        public ApplicationSettingViewModel(ISettingsService settingsService)
        {
            Logger.Information(ApplicationSettingViewModelEvents.CreateInstance, "Creating new instance of ApplicationSettingViewModel");
            SettingsService = settingsService;
            ObjectId = SettingsService.ObjectId;
            UserId = SettingsService.UserId;
            HomePage = SettingsService.HomePage;
            ReloadTime = SettingsService.ReloadTime;
            SelectedLanguage = LanguageSource.GetValues()[SettingsService.Language];
            SaveCommand = new RelayCommand(Save, CanSave);
            Language = new ObservableCollection<string>();
            foreach (var lang in LanguageSource.GetValues())
            {
                Language.Add(lang.Value);
            }
        }

        private bool CanSave()
        {
            return true;
        }

        private void Save()
        {
            var langKey = LanguageSource.GetValues().FirstOrDefault(x => x.Value == SelectedLanguage).Key;

            Logger.Information(ApplicationSettingViewModelEvents.SaveSettingCommand, $"ObjectId: {ObjectId} " +
                                                                                     $"UserId: {UserId} HomePage: {HomePage} " +
                                                                                     $"ReloadTime: {ReloadTime} LangKey: {langKey}");
            SettingsService.ChromiumSettingSave(ObjectId, UserId, HomePage, ReloadTime, langKey);
            CloseAction?.Invoke();
        }
    }
}
