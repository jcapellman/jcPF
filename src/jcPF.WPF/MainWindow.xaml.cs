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
        }
    }
}