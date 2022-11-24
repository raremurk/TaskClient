using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string start = $"{StartPicker.SelectedDate:yyyy-MM-dd}";
            string end = $"{EndPicker.SelectedDate:yyyy-MM-dd}";
            var index = Сurrency.SelectedIndex + 1;
            ChartView.GetData(index, start, end);
        }
    }
}
