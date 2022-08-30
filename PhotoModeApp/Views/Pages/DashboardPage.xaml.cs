using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation.Collections;
using Wpf.Ui.Common.Interfaces;

namespace PhotoModeApp.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>
    {
        private enum PROCESSING_STATUS
        {
            Done = 999
        };

        public ViewModels.DashboardViewModel ViewModel
        {
            get;
        }

        private int totalNumberOfFiles;

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            PathAction.Content = Helpers.Config.GetPath();


            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                ValueSet userInput = toastArgs.UserInput;

                Application.Current.Dispatcher.Invoke(delegate
                {
                    Process.Start("explorer.exe", Helpers.Config.GetPath());
                });
            };
        }

        public async Task SetupAsync()
        {
            await Task.Run(() =>
            {
                if (!Helpers.Config.GetPath().Equals(string.Empty))
                {
                    PathAction.IsEnabled = true;
                    PathAction.Content = Helpers.Config.GetPath();
                }
            });
        }

        private async void PathAction_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                await Task.Run(() => Helpers.Config.WritePath(dialog.SelectedPath));
                PathAction.Content = dialog.SelectedPath;
            }
        }


        private async Task<int> ProcessPicturesAsync(IProgress<int> progress)
        {
            int processCount = await Task.Run<int>(() =>
            {
                int tempCount = 0;

                DirectoryInfo di = Directory.CreateDirectory(Helpers.Config.GetPath() + "/original");

                Process converterProcess = new Process();

                converterProcess.StartInfo.RedirectStandardOutput = true;
                converterProcess.StartInfo.UseShellExecute = false;
                converterProcess.StartInfo.CreateNoWindow = true;

                converterProcess.StartInfo.FileName = "ragephoto-extract.exe";

                foreach (var image in Directory.GetFiles(Helpers.Config.GetPath(), "PRDR3*", SearchOption.AllDirectories))
                {
                    converterProcess.StartInfo.Arguments = image + " " + image + ".jpg";
                    converterProcess.Start();

                    converterProcess.WaitForExit();

                    var finalPath = String.Format("{0}\\original\\{1}", Helpers.Config.GetPath(),
                        Path.GetFileName(image));

                    File.Move(image, finalPath);

                    tempCount++;
                    if (progress != null) progress.Report((tempCount));

                }

                return tempCount;
            });

            if (processCount == totalNumberOfFiles) return (int)PROCESSING_STATUS.Done;
            return processCount;

        }

        private async void ConvertButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            totalNumberOfFiles = Helpers.Win32Files.GetFileCount(Helpers.Config.GetPath(), true);

            if (totalNumberOfFiles < 1)
            {
                new ToastContentBuilder()
                  .AddArgument("action", "viewConversation")
                  .AddArgument("conversationId", 9813)
                  .AddText("Could not find any photos.")
                  .AddButton(new ToastButton()
                      .SetContent("Show")
                      .SetBackgroundActivation())
                  .Show();
                return;
            }

            ConvertButton.IsEnabled = false;
            ProgressText.Visibility = Visibility.Visible;

            int proc = await ProcessPicturesAsync(new Progress<int>(percent => UpdateProgressUI(percent)));

            if (proc == (int)PROCESSING_STATUS.Done)
            {
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("Photos successfully converted! 🎉")
                    .AddButton(new ToastButton()
                        .SetContent("Show")
                        .AddArgument("action", "reply")
                        .SetBackgroundActivation())
                    .Show();

                ConvertButton.IsEnabled = true;
            }
        }

        private void UpdateProgressUI(int value)
        {
            ProgressText.Text = String.Format("Procesing: {0}/{1}", value, totalNumberOfFiles);
        }
    }
}