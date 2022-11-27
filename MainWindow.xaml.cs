using Client.Properties;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartPicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, new DateTime(DateTime.Today.Year - 6, 12, 31)));
            StartPicker.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.MaxValue));
            EndPicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, new DateTime(DateTime.Today.Year - 6, 12, 31)));
            EndPicker.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.MaxValue));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string start = $"{StartPicker.SelectedDate:yyyy-MM-dd}";
            string end = $"{EndPicker.SelectedDate:yyyy-MM-dd}";
            var index = Сurrency.SelectedIndex + 1;
            ChartView.GetData(index, start, end);
        }

        private void DatePicker_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            DateTime newDate;
            DatePicker datePickerObj = sender as DatePicker;
            if (DateTime.TryParse(e.Text, out newDate))
            {
                if (datePickerObj.BlackoutDates.Contains(newDate))
                {
                    MessageBox.Show(String.Format($"The date, {e.Text}, cannot be selected."));
                }
            }
            else
            {
                MessageBox.Show(String.Format($"The date, {e.Text}, isn't correct."));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.Save();
            base.OnClosed(e);   
        }
    }
}
