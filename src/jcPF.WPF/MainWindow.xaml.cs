using System.Windows;

using jcPF.WPF.ViewModels;

namespace jcPF.WPF
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            viewModel.LoadData();

            MouseDown += MainWindow_MouseDown;
        }

        private void MainWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void BtnStartMonitoring_OnClick(object sender, RoutedEventArgs e)
        {
            var result = await viewModel.RunScan();
        }
    }
}