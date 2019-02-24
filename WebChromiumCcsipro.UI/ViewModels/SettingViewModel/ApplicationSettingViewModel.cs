using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Resources.Enums;
using WebChromiumCcsipro.Resources.Extensions;
using WebChromiumCcsipro.Resources.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Language;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ApplicationSettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<ApplicationSettingViewModel>();
        public RelayCommand SaveCommand { get; set; }

        private ISettingsService SettingsService { get; set; }
        private string _selectedLanguage;

        public string ObjectId { get; set; }
        public string UserId { get; set; }
        public string HomePage { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string AllowedSite { get; set; }

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
            Logger.Information(ApplicationSettingViewModelEvents.CreateInstance);
            SettingsService = settingsService;
            ObjectId = SettingsService.ObjectId;
            UserId = SettingsService.UserId;
            HomePage = SettingsService.HomePage;
            SelectedLanguage = LanguageSource.GetValues()[SettingsService.Language];
            SaveCommand = new RelayCommand(Save, CanSave);
            Language = new ObservableCollection<string>();
            foreach (var lang in LanguageSource.GetValues())
            {
                Language.Add(lang.Value);
            }
            AllowedSite = SettingsService.AllowedSite;
            Login = SettingsService.WebLogin;
            Password = SettingsService.WebPassword;
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
                                                                                     $"LangKey: {langKey} AllowedSite: {AllowedSite}");
            SettingsService.ChromiumSettingSave(ObjectId, UserId, HomePage, langKey, AllowedSite, Login, Password);
            CloseAction?.Invoke();
        }
    }
}
