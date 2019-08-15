using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private bool _reloadEnable;
        private TimeSpan? _reloadTime;

        public int ObjectId { get; set; }
        public int UserId { get; set; }
        public string HomePage { get; set; }

        public bool ReloadEnable
        {
            get { return _reloadEnable; }
            set
            {
                _reloadEnable = value;
                RaisePropertyChanged();
            }
        }

        public TimeSpan? ReloadTime
        {
            get { return _reloadTime; }
            set
            {
                _reloadTime = value;
                RaisePropertyChanged();
            }
        }


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

            ReloadEnable = SettingsService.ReloadTimeEnable;
            HomePage = SettingsService.HomePage;
            ReloadTime = SettingsService.ReloadTime.TimeOfDay;
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
            //TODO fix bus with saving time reload

            if (!ReloadTime.HasValue)
                ReloadTime = DateTime.Now.TimeOfDay;
            Logger.Information(ApplicationSettingViewModelEvents.SaveSettingCommand, $"ObjectId: {ObjectId} " +
                                                                                     $"UserId: {UserId} HomePage: {HomePage} " +
                                                                                     $"Reload Time: {ReloadTime:G} " +
                                                                                     $"ReloadTimeEnable {ReloadEnable} LangKey: {langKey}");

            SettingsService.ChromiumSettingSave(ObjectId, UserId, HomePage, ReloadEnable,
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ReloadTime.Value.Hours, ReloadTime.Value.Minutes,
                ReloadTime.Value.Seconds), langKey);
            CloseAction?.Invoke();
            //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
            //TODO reload app fix
        }
    }
}
