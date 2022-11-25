using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace Client
{
    public class ChartView
    {
        private static ObservableCollection<DateTimePoint> values = new()
        {
            new DateTimePoint(new DateTime(2020, 1, 1), 2.5),
            new DateTimePoint(new DateTime(2020, 1, 2), 2.6),
            new DateTimePoint(new DateTime(2020, 1, 3), 2.7),
            new DateTimePoint(new DateTime(2020, 1, 4), 2.4),
            new DateTimePoint(new DateTime(2020, 1, 5), 2.5),
            new DateTimePoint(new DateTime(2020, 1, 6), 2.5),
            new DateTimePoint(new DateTime(2020, 1, 7), 2.6),
            new DateTimePoint(new DateTime(2020, 1, 8), 2.7),
            new DateTimePoint(new DateTime(2020, 1, 9), 2.4),
            new DateTimePoint(new DateTime(2020, 1, 10), 2.5)
        };
        private static double minValue = double.MaxValue;
        private static double maxValue = 0;

        public ISeries[] Series { get; set; } =
        {
            new ColumnSeries<DateTimePoint>
            {
                TooltipLabelFormatter = (chartPoint) => 
                    $"{new DateTime((long) chartPoint.SecondaryValue):dd MMMM yyyy}: {chartPoint.PrimaryValue}",
                Values = values
            }
            .WithConditionalPaint(new SolidColorPaint(SKColors.YellowGreen))
            .When(point => point.Model?.Value == minValue)
            .WithConditionalPaint(new SolidColorPaint(SKColors.Red))
            .When(point => point.Model?.Value == maxValue)
        };

        public static Axis[] XAxes { get; set; } =
{
            new Axis
            {
                Name = "Валюта",
                Labeler = value => new DateTime((long) value).ToString("d MMM yyyy"),
                LabelsRotation = 10,
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                MinStep = TimeSpan.FromDays(1).Ticks
            }
        };

        public static Axis[] YAxes { get; set; } = 
        { 
            new Axis
            {
                Name = "Курс"
            }
        };

        public static async void GetData(int id, string start, string end)
        {
            HttpClient httpClient = new();

            try
            {
                using var response = await httpClient.GetAsync($"https://localhost:7165/api/GetData?id={id}&start={start}&end={end}");

                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show(response.StatusCode.ToString());
                }
                else
                {
                    List<Rate> data = await response.Content.ReadFromJsonAsync<List<Rate>>();

                    values.Clear();
                    minValue = double.MaxValue;
                    maxValue = 0;
                    YAxes[0].Name = $"{data[0].ValueCurrency}";
                    XAxes[0].Name = $"{data[0].Amount} {data[0].Currency}";
                    foreach (var rate in data)
                    {
                        values.Add(new DateTimePoint(rate.Date, rate.Value));
                        if (rate.Value < minValue)
                        {
                            minValue = rate.Value;
                        }

                        if (rate.Value > maxValue)
                        {
                            maxValue = rate.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
