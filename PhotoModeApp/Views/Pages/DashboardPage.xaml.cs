using Microsoft.Toolkit.Uwp.Notifications;
using PhotoModeApp.Helpers;
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

        private int filesSkipped = 0;

        private bool isAnyFilesSkipped;


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

                DirectoryInfo di = Directory.CreateDirectory(Helpers.Config.GetPath() + "/ConvertedImages");

                foreach (var image in Directory.GetFiles(Helpers.Config.GetPath(), "PRDR3*", SearchOption.TopDirectoryOnly))
                {
                    RagePhoto.Convert(image);

                    string convertedFileName = image + ".jpg";

                    var finalPath = String.Format("{0}\\ConvertedImages\\{1}", Helpers.Config.GetPath(),
                        Path.GetFileName(convertedFileName));
                    try
                    {
                        File.Move(convertedFileName, finalPath);
                    }
                    catch (IOException)
                    {
                        File.Delete(convertedFileName);
                        isAnyFilesSkipped = true;
                        filesSkipped++;
                    }

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
            if (Helpers.Config.GetPath().Length < 1) return;

            totalNumberOfFiles = Helpers.Win32Files.GetFileCount(Helpers.Config.GetPath(), false);

            if (totalNumberOfFiles < 1)
            {
                StatusLabel.Text = "Could not find any photos.";
                return;
            }

            ConvertButton.IsEnabled = false;
            StatusLabel.Visibility = Visibility.Visible;

            int proc = await ProcessPicturesAsync(new Progress<int>(percent => UpdateProgressUI(percent)));

            if (proc == (int)PROCESSING_STATUS.Done)
            {
                if (!isAnyFilesSkipped)
                {
                    StatusLabel.Text = "Photos successfully converted! 🎉";
                    ConvertButton.IsEnabled = true;

                    new ToastContentBuilder()
                        .AddArgument("action", "viewConversation")
                        .AddArgument("conversationId", 9813)
                        .AddText("Photos successfully converted! 🎉")
                        .AddButton(new ToastButton()
                        .SetContent("Show")
                        .SetBackgroundActivation())
                        .Show();
                    return;
                }

                if (filesSkipped == totalNumberOfFiles)
                {
                    StatusLabel.Text = $"No photos were converted.\nAll file(s) were skipped because their converted versions of the same name\nalready exist in the ConvertedImages folder.";
                    ConvertButton.IsEnabled = true;
                    return;
                }

                StatusLabel.Text = $"Photos successfully converted! 🎉\n{filesSkipped}/{totalNumberOfFiles} file(s) were skipped because their converted versions of the same\nname already exist in the ConvertedImages folder.";
                ConvertButton.IsEnabled = true;
                return;
            }
        }

        private void UpdateProgressUI(int value)
        {
            StatusLabel.Text = String.Format("Procesing: {0}/{1}", value, totalNumberOfFiles);
        }
    }
}