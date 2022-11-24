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
        private static ObservableCollection<DateTimePoint> values = new();
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
            .WithConditionalPaint(new SolidColorPaint(SKColors.Green.WithAlpha(120)))
            .When(point => point.Model?.Value == minValue)
            .WithConditionalPaint(new SolidColorPaint(SKColors.Red.WithAlpha(120)))
            .When(point => point.Model?.Value == maxValue)
        };

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Labeler = value => new DateTime((long) value).ToString("dd.MM.yyyy"),
                LabelsRotation = 15,
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                MinStep = TimeSpan.FromDays(1).Ticks
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
                    foreach (var rate in data)
                    {
                        values.Add(new DateTimePoint(rate.Date, rate.Price));
                        if (rate.Price < minValue)
                        {
                            minValue = rate.Price;
                        }

                        if (rate.Price > maxValue)
                        {
                            maxValue = rate.Price;
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
