using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

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
        }

        private async void OpenDialog()
        {
            var result = await FinishDialog.ShowAndWaitAsync(
                "WPF UI Dialog",
                "What is it like to be a scribe? Is it good? In my opinion it's not about being good or not good. If I were to say what I esteem the most in life, I would say - people. People, who gave me a helping hand when I was a mess, when I was alone. And what's interesting, the chance meetings are the ones that influence our lives. The point is that when you profess certain values, even those seemingly universal, you may not find any understanding which, let me say, which helps us to develop. I had luck, let me say, because I found it. And I'd like to thank life. I'd like to thank it - life is singing, life is dancing, life is love. Many people ask me the same question, but how do you do that? where does all your happiness come from? And i replay that it's easy, it's cherishing live, that's what makes me build machines today, and tomorrow... who knows, why not, i would dedicate myself to do some community working and i would be, wham, not least... planting .... i mean... carrots.");
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
        }
    }
}