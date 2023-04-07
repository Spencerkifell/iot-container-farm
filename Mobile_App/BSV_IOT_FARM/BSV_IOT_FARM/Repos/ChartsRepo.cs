/*
 * BSV - Team #12
 * Winter 2022 - May 20, 2022
 * Application Development III
 *
 * Chart Repository Class - Meant to be used as a repository for all chart related data.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using BSV_IOT_FARM.Models;
using Microcharts;
using SkiaSharp;

namespace BSV_IOT_FARM.Repos
{
    /// <summary>
    /// Class meant to be used as a repository for all chart related data.
    /// </summary>
    public static class ChartsRepo
    {
        /// <summary>
        /// Method that takes in telemetry history and the given property that we want to receive a chart for.
        /// Iterates through the history and returns the values that are not null and belong to the filtered property.
        /// </summary>
        /// <param name="history"> Takes in a collection of telemetry history. </param>
        /// <param name="property"> Takes in the property within the list we want to filter on. </param>
        /// <returns> Returns a transformed collection containing the filtered values. </returns>
        private static IEnumerable<dynamic> TransformDataset(List<Telemetry> history, string property)
        {
            // Dynamic so that it can apply to any property within the Telemetry object...
            var propertyTelemetry = new List<dynamic>();
            
            foreach (var telemetry in history)
            {
                var value = telemetry.GetType().GetProperty(property)?.GetValue(telemetry);
                
                // Since the values are nullable, we don't want to use any value that are potentially null.
                if (value == null)
                    continue;
                
                propertyTelemetry.Add(value);
            }

            return propertyTelemetry;
        }
        
        /// <summary>
        /// Generates a chart for the given property based on telemetry history.
        /// </summary>
        /// <param name="history"> A raw collection of Telemetry data. </param>
        /// <param name="property"> The property we want to chart based off. </param>
        /// <param name="amountOfDataPoints"> The amount of data points we want to generate. </param>
        /// <returns> Returns a line chart </returns>
        /// <exception cref="ArgumentException"> If there aren't any data points, an error is thrown to alert the user. </exception>
        public static LineChart GetLineChart(List<Telemetry> history, string property, int amountOfDataPoints)
        {
            // If the received property doesn't exist on a Telemetry object then an exception is thrown.
            if (typeof(Telemetry).GetProperties().All(p => p.Name != property))
                throw new ArgumentException("Property does not exist within Telemetry class");

            // Retrieve the transformed dataset that contains non null and associated property values.
            var transformedDataset = TransformDataset(history, property).ToList();
            
            if (transformedDataset.Count == 0)
                throw new ArgumentException($"No data points found for {property}");
                
            var lineChart = new LineChart();
            var chartEntries = new List<ChartEntry>();

            // If there are more data points than the amount present in the dataset, then the amount of data points is reduced.
            if (amountOfDataPoints > transformedDataset.Count())
                amountOfDataPoints = transformedDataset.Count();
            
            // Iterate through the data points starting from amount of data points to the end of the dataset.
            for (int i = transformedDataset.Count() - amountOfDataPoints, count = 1; i < transformedDataset.Count(); i++, count++)
            {
                var chartEntry = new ChartEntry((float) transformedDataset[i])
                {
                    Label = count.ToString(),
                    ValueLabel = transformedDataset[i].ToString(),
                    Color = SKColor.Parse("#23a2f9")
                };
                chartEntries.Add(chartEntry);
            }
            
            lineChart.Entries = chartEntries;
            lineChart.LabelTextSize = 20;
            // Added a min/max value to the chart to ensure that the values are always visible.
            lineChart.MaxValue = (float) transformedDataset.Max() + (float) (Math.Abs(transformedDataset.Max()) / 4);
            lineChart.MinValue = (float) transformedDataset.Min() - (float) (Math.Abs(transformedDataset.Min()) / 4);
            lineChart.ValueLabelOrientation = Orientation.Horizontal;
            lineChart.BackgroundColor = SKColor.Parse("#fbfbfb");
            
            return lineChart;
        }
    }
}