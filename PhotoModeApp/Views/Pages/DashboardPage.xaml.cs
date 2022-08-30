using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Windows.Foundation.Collections;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace PhotoModeApp.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>
    {
        private readonly IDialogControl _dialogControl;

        public ViewModels.DashboardViewModel ViewModel
        {
            get;
        }

        public DashboardPage(ViewModels.DashboardViewModel viewModel, IDialogService dialogService)
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

        public void Setup()
        {
            if (!Helpers.Config.GetPath().Equals(string.Empty))
            {
                PathAction.IsEnabled = true;
                PathAction.Content = Helpers.Config.GetPath();
            }
        }

        private void PathAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Helpers.Config.WritePath(dialog.SelectedPath);
                PathAction.Content = Helpers.Config.GetPath();
            }
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process converterProcess = new Process();

            converterProcess.StartInfo.RedirectStandardOutput = true;
            converterProcess.StartInfo.UseShellExecute = false;
            converterProcess.StartInfo.CreateNoWindow = true;

            converterProcess.StartInfo.FileName = "ragephoto-extract.exe";

            foreach (var image in Directory.GetFiles(Helpers.Config.GetPath(), "*.*", SearchOption.AllDirectories))
            {
                converterProcess.StartInfo.Arguments = image + " " + image + ".jpg";
                converterProcess.Start();
                await converterProcess.WaitForExitAsync();
            }

            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("Photos successfully converted! 🎉")
                .AddButton(new ToastButton()
                    .SetContent("Show")
                    .AddArgument("action", "reply")
                    .SetBackgroundActivation())
                .Show();
        }

    }
}