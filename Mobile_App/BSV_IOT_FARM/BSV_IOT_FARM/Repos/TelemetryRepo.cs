/*
 * BSV - Team #12
 * Winder 2022 - May 20, 2022
 * Application Development III
 *
 * Telemetry Repository Class - Repository to retrieve and generate Telemetry data.
 */

using System;
using BSV_IOT_FARM.Models;

namespace BSV_IOT_FARM.Repos
{
    /// <summary>
    /// Repository to retrieve and generate Telemetry data
    /// </summary>
    public static class TelemetryRepo
    {
        /// <summary>
        /// Static method to generate random telemetry data.
        /// </summary>
        /// <returns> Returns random telemetry data. </returns>
        public static Telemetry GenerateTelemetry()
        {
            var randomGenerator = new Random();

            return new Telemetry()
            {
                // Geo Location Telemetry Data
                Latitude = Math.Round(randomGenerator.NextDouble() * (200 - -200) + -200, 6),
                Longitude = Math.Round(randomGenerator.NextDouble() * (200 - -200) + -200, 6),
                Pitch = Math.Round(randomGenerator.NextDouble() * (100 - -100) + -100, 2),
                Roll = Math.Round(randomGenerator.NextDouble() * (100 - -100) + -100, 2),
                Vibration = new Vibration(Math.Round(randomGenerator.NextDouble() * (100 - -100) + -100, 2), Math.Round(randomGenerator.NextDouble() * (100 - -100) + -100, 2), Math.Round(randomGenerator.NextDouble() * (100 - -100) + -100, 2)),
                
                // Plant Telemetry Data
                Temperature = randomGenerator.Next(20, 30),
                Humidity = randomGenerator.Next(0, 100),
                Moisture = randomGenerator.Next(0, 100),
                WaterLevel = randomGenerator.Next(0, 100),
                LightIsActive = randomGenerator.Next() > (int.MaxValue / 2),
                FanIsActive = randomGenerator.Next() > (int.MaxValue / 2),
                
                // Security Telemetry Data
                Noise = randomGenerator.Next(20, 30),   
                Luminosity = randomGenerator.Next(20, 30),
                Motion = randomGenerator.Next() > (int.MaxValue / 2),
                Door = randomGenerator.Next() > (int.MaxValue / 2),
                DoorIsLocked = randomGenerator.Next() > (int.MaxValue / 2),

                // Shared Telemetry Data (Between Security and Geo Location)
                BuzzerIsActive = randomGenerator.Next() > (int.MaxValue / 2)
            };
        }
    }
}