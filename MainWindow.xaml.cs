using System.Windows;

namespace BinLine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.page1.DataContext = viewModel.page1;
            //viewModel.page1.PropertyChanged += Model_PropertyChanged;
        }

        //private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    Page1ViewModel model = sender as Page1ViewModel;
        //    if (e.PropertyName == "Xline1")
        //    {
        //        this.page1.textblock1.ItemsSource = null;
        //        this.page1.textblock1.ItemsSource = model.Xline1;
        //    }
        //    if (e.PropertyName == "Yline1")
        //    {
        //        this.page1.textblock2.ItemsSource = null;
        //        this.page1.textblock2.ItemsSource = model.Yline1;
        //    }
        //}

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenFile();
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveAsImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Line1_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Line(0,viewModel.Xfdata(SimalateData.Text),viewModel.Simulate(SimalateData.Text,viewModel.Base(SimalateData.Text)));
        }

        private void RandomNosie_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Line(1, viewModel.Xfdata(SimalateData.Text), viewModel.Simulate(SimalateData.Text, viewModel.Base(SimalateData.Text)));
        }

        private void arPLS_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Line(2, viewModel.Xfdata(SimalateData.Text), viewModel.Simulate(SimalateData.Text, viewModel.Base(SimalateData.Text)));
        }

        private void Dada_Click(object sender, RoutedEventArgs e)
        {
            viewModel.S = SimalateData.Text;
        }

        private void Poly_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Poly(viewModel.Xfdata(SimalateData.Text), viewModel.Simulate(SimalateData.Text, viewModel.Base(SimalateData.Text)));
        }

        private void Realdata_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Realdata(viewModel.Xdata,viewModel.Ydata);
        }

        private void page1_Loaded()
        {

        }
    }
}
