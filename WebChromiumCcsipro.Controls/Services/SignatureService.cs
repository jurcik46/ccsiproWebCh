using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using Serilog.Core;
using WebChromiumCcsipro.Resources.Enums;
using WebChromiumCcsipro.Resources.Extensions;
using WebChromiumCcsipro.Resources.Interfaces;
using WebChromiumCcsipro.Resources.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Messages;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.Resources.Settings;

namespace WebChromiumCcsipro.Controls.Services
{
    public class SignatureService : ISignatureService
    {
        private ILogger Logger => Log.Logger.ForContext<SignatureService>();

        private ISettingsService SettingsService { get; set; }
        private IApiService ApiService { get; set; }

        private ISignatureFileModel SignatureFileModel { get; set; }

        public bool InProcces { get; set; }
        private string _appRomaingPath;



        public SignatureService(IApiService apiService, ISettingsService settingsService)
        {
            ApiService = apiService;
            SettingsService = settingsService;
        }
        #region Start signature 
        public void StartSign()
        {
            Logger.Debug(SignatureServiceEvents.StartSign);
            using (Logger.BeginTimedOperation(SignatureServiceEvents.StartSign))
            {
                InProcces = true;
                Messenger.Default.Send(new ChangeIconMessage() { Icon = TrayIconsStatus.Working });
                _appRomaingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CCSIPRO");

                Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationSearchingDocument, IconType = Notifications.Wpf.NotificationType.Information, ExpTime = 300 });
                SignatureFileModel = ApiService.GetDocumentToSignature();
                if (SignatureFileModel != null)
                {
                    Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationSavingDocument, IconType = Notifications.Wpf.NotificationType.Information, ExpTime = 300 });
                    // prehodi lomku 
                    string directhoryPath = SignatureFileModel.PdfFilePath.Replace('/', '\\');
                    // prida typ suboru na konci hashu
                    string fileName = SignatureFileModel.Hash + SignatureFileModel.PdfFilePath.Substring(SignatureFileModel.PdfFilePath.LastIndexOf("."));
                    // Vytvori processname z options a vyplni paramatere {0} - filename {1} - directhoryPath
                    string processName = string.Format(SettingsService.ProcessName, fileName, directhoryPath); ;

                    /// vytvori nove zlozku 
                    if (CreateDirectory(ref directhoryPath))
                    {
                        // ulozi prijaty dokument do vytvorenej zlozky
                        if (SaveFile(directhoryPath, fileName, SignatureFileModel.File))
                        {
                            string filePath = directhoryPath + fileName;

                            Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationStarginSingSoftware, IconType = Notifications.Wpf.NotificationType.Information, ExpTime = 300 });
                            // spusti podpisovaci program 
                            if (SignFile(processName, SettingsService.ProgramPath, filePath))
                            {
                                //  Messenger.Default.Send<NotifiMessage>(new NotifiMessage() { Title = ViewModelLocator.rm.GetString("signatureTitle"), Msg = ViewModelLocator.rm.GetString("successSignDocumet"), IconType = Notifications.Wpf.NotificationType.Success });
                                Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationUploadingDocument, IconType = Notifications.Wpf.NotificationType.Information, ExpTime = 300 });

                                if (ApiService.UploadSignedDocument(SignatureFileModel.Hash, SignatureFileModel.PdfFilePath.Substring(1, SignatureFileModel.PdfFilePath.Length - 1), filePath))
                                {
                                    //                                    Messenger.Default.Send<BozpStatusPusherMessage>(new BozpStatusPusherMessage() { Status = "200" });
                                    Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationSuccessUploadedDocument, IconType = Notifications.Wpf.NotificationType.Success, ExpTime = 10 });
                                }
                                else
                                {
                                    //                                    Messenger.Default.Send<BozpStatusPusherMessage>(new BozpStatusPusherMessage() { Status = "500" });
                                    Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationFialedUploadedDocument, IconType = Notifications.Wpf.NotificationType.Error });
                                }
                            }
                            else
                            {

                                //                                Messenger.Default.Send<BozpStatusPusherMessage>(new BozpStatusPusherMessage() { Status = "408" });
                                Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationClosedDocument, IconType = Notifications.Wpf.NotificationType.Warning });
                            }
                        }
                        else
                        {
                            Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationFailedSavingDocument, IconType = Notifications.Wpf.NotificationType.Error });
                        }
                    }
                    else
                    {
                        Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationFailedSavingDocument, IconType = Notifications.Wpf.NotificationType.Error });
                    }
                }
                else
                {
                    Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationNotFoundDocument, IconType = Notifications.Wpf.NotificationType.Warning });
                    //                    Messenger.Default.Send<BozpStatusPusherMessage>(new BozpStatusPusherMessage() { Status = "404" });
                }

                Messenger.Default.Send(new ChangeIconMessage() { Icon = TrayIconsStatus.Online });
                InProcces = false;
            }
        }

        #endregion


        #region Create Directory
        private bool CreateDirectory(ref string directhoryPath)
        {
            Logger.With("directhoryPath", directhoryPath)
               .Debug(SignatureServiceEvents.CreateDirectory);
            //hash += data.link.substring(data.link.lastindexof("."));
            //  _appRomaingPath
            //var appRomaingPath = Path.Combine(LoggerInit.RoamingPath, LoggerInit.ApplicationName);
            directhoryPath = directhoryPath.Substring(1, directhoryPath.LastIndexOf("\\"));
            directhoryPath = Path.Combine(_appRomaingPath, directhoryPath);
            try
            {
                if (!Directory.Exists(directhoryPath))
                {
                    Directory.CreateDirectory(directhoryPath);
                    //  log.information("vytváram  zložku : {0}", directhorypath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.With("directhoryPath", directhoryPath)
                  .Error(ex, SignatureServiceEvents.CreateDirectoryError);
                //log.warning("nepodarilo sa vytvoriť zložku : {0} : exception {1}", directhorypath, ex.message);
                return false;
                //throw new myexception("nepodarilo sa vytvoriť zložku pre dokument");
            }
        }
        #endregion

        #region Read and Write File
        private bool SaveFile(string directhoryToSave, string fileName, byte[] file)
        {
            Logger.With("directhoryToSave", directhoryToSave)
                .With("fileName", fileName)
                .Debug(SignatureServiceEvents.SaveFile);

            try
            {
                System.IO.File.WriteAllBytes(directhoryToSave + fileName, file);
                return true;
            }
            catch (Exception ex)
            {
                Logger.With("directhoryToSave", directhoryToSave)
                    .With("fileName", fileName)
                    .Error(ex, SignatureServiceEvents.SaveFileError);
                return false;
            }
        }

        private byte[] ReadFile(string directhorypath, string hash)
        {
            try
            {
                Stream pdffile = File.OpenRead(directhorypath + hash);
                byte[] bytes = File.ReadAllBytes(directhorypath + hash);
                //uploadfile(hash, bytes, link, directhorypath);
                return bytes;
            }
            catch (Exception ex)
            {
                return null;
                //throw new myexception(ex.message);
            }
        }
        #endregion


        #region Sign
        /// <summary>
        /// Metoda na otvorenie suboru v procese a po ulozenej zmene jeho zatvorenie
        /// </summary>
        /// <returns></returns>
        public bool SignFile(string processName, string programPath, string pdfFilePath)
        {

            bool result = false;
            //Dlzka trvania podpisu
            var SignTimeout = SignatureSetting.Default.singTimeOut; ;

            var startInfo = new ProcessStartInfo(programPath, pdfFilePath);

            using (var process = Process.Start(startInfo))
            {
                var fileInfo = new FileInfo(pdfFilePath);
                var lastWrite = fileInfo.LastWriteTime;

                var counter = 0;
                var foundWindow = false;
                TimeSpan diff;

                do
                {
                    Thread.Sleep(1000);
                    fileInfo.Refresh();
                    diff = fileInfo.LastWriteTime - lastWrite;

                    counter++;
                    if (diff.TotalSeconds > 1)
                    {
                        result = true;
                        break;
                    }
                    var found = FindWindowAndClose(false, processName);
                    if (!foundWindow && found)
                    {
                        Logger.Debug(SignatureServiceEvents.SignFileWindowFound, "Sign application window found for the first time in {Iteration}. iteration.", counter);
                        foundWindow = true;
                        Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationSuccessOpenDocument, IconType = Notifications.Wpf.NotificationType.Information, ExpTime = 300 });
                    }
                    if (counter <= 4 || !foundWindow || (found))
                    {
                        continue;
                    }
                    Logger.Debug(SignatureServiceEvents.SignFileWindowClosed, "Sign application closed before timeout in {Iteration}. iteration.", counter);
                    break;
                } while (counter < SignTimeout);

                var ForceClose = true;

                if (process != null && !process.HasExited && (ForceClose || result))
                {
                    //Messenger.Default.Send(new BusyMessage(this, callerReference, Resources.StarterClosing));
                    process.CloseMainWindow();
                }
                if (ForceClose)
                {
                    //Messenger.Default.Send(new BusyMessage(this, callerReference, Resources.SignerClosing));

                    FindWindowAndClose(true, processName);
                }
                fileInfo.Refresh();
                diff = fileInfo.LastWriteTime - lastWrite;
                Log.Information("FileInfo {0} TimeDifference {1} ", fileInfo, diff.TotalSeconds);
                Logger.With("FileInfo", fileInfo).With("TimeDifference", diff.TotalSeconds).Debug(SignatureServiceEvents.SignFileAfterWait);
                result = diff.TotalSeconds > 1;
            }
            return result;
        }


        /// <summary>
        /// Metoda na na najdenie okna a zatvorenie 
        /// </summary>
        /// <param name="close"></param>
        /// <returns></returns>
        private bool FindWindowAndClose(bool close, string processName)
        {
            //FilePath = Path.GetFileName(FilePath);
            var caption = string.Format(processName, SignatureFileModel.PdfFilePath);// + " - Visual Studio Code";//string.Format(SettingsService.SignWindowCaptionFormat, FilePath);
            var windowPtr = FindWindowByCaption(IntPtr.Zero, caption);
            if (windowPtr == IntPtr.Zero)
            {
                Logger.Debug(SignatureServiceEvents.WindowNotFound, "Window not found: {Caption}", caption);

                return false;
            }
            if (!close)
            {
                Logger.Debug(SignatureServiceEvents.WindowFound, "Window found: {Caption}", caption);
                return true;
            }
            Logger.Debug(SignatureServiceEvents.WindowFoundAndClosing, "Window found and closing: {Caption}", caption);
            SendMessage(windowPtr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            return false;
        }

        /// <summary>
        /// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CLOSE = 0x0010;

        #endregion
    }
}