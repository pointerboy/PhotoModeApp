using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Common.Interfaces;

namespace PhotoModeApp.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {
        public ViewModels.SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            this.Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            VersionText.Text += Assembly.GetExecutingAssembly().GetName().Version;

            ThemeSelectionDropDown.ItemsSource = new List<string> { "Light", "Dark" };

            switch (Wpf.Ui.Appearance.Theme.GetAppTheme())
            {
                case Wpf.Ui.Appearance.ThemeType.Light:
                    ThemeSelectionDropDown.Text = "Light";
                    break;
                case Wpf.Ui.Appearance.ThemeType.Dark:
                    ThemeSelectionDropDown.Text = "Dark";
                    break;
            }

            ThemeSelectionDropDown.IsEditable = false;

            ThemeSelectionDropDown.SelectionChanged += ThemeSelectionDropDown_DataContextChanged;
        }

        private void ThemeSelectionDropDown_DataContextChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (e.AddedItems[0].ToString())
            {
                case "Light":
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
                    break;
                case "Dark":
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
                    break;
            }
        }
    }
}