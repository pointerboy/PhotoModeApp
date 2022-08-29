using Wpf.Ui.Common.Interfaces;

namespace PhotoModeApp.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>
    {
        public ViewModels.DashboardViewModel ViewModel
        {
            get;
        }

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        public void Setup()
        {
            if (!Helpers.Config.GetPath().Equals(string.Empty))
            {
                PathAction.IsEnabled = true;
                PathAction.Content = Helpers.Config.GetPath();
            }
        }
    }
}