/*
 * BSV - Team #12
 * Winter 2022 - May 2, 2022
 * Application Development III
 */

using System;
using BSV_IOT_FARM.Models;

namespace BSV_IOT_FARM.Repos.Deprecated
{
    /// <summary>
    /// Repository to retrieve and generate data for the Geo Location Subsystem
    /// </summary>
    public class GeoLocationRepo
    {
        private GeoLocationSubsystem _geoLocationSubsystem;
        
        /// <summary>
        /// Default constructor for the GeoLocationRepo, initializes the GeoLocationSubsystem with dummy data.
        /// </summary>
        public GeoLocationRepo()
        {
            _geoLocationSubsystem = GenerateDummyData();
        }
        
        /// <summary>
        /// Property for the GeoLocationSubsystem
        /// </summary>
        public GeoLocationSubsystem GeoLocationSubsystem
        {
            get => _geoLocationSubsystem;
            set => _geoLocationSubsystem = value;
        }

        /// <summary>
        /// Creates a new GeoLocationSubsystem with dummy data.
        /// </summary>
        /// <returns> A new Geo Location Subsystem populated with dummy data. </returns>
        public static GeoLocationSubsystem GenerateDummyData()
        {
            var randomGenerator = new Random();

            return new GeoLocationSubsystem()
            {
                Latitude = Math.Round(randomGenerator.NextDouble() * (200 - -200) + -200, 6),
                Longitude = Math.Round(randomGenerator.NextDouble() * (200 - -200) + -200, 6),
                Pitch = Math.Round(randomGenerator.NextDouble() * (200 - -200) + -200, 2),
                Roll = Math.Round(randomGenerator.NextDouble() * (200 - -200) + -200, 2),
                Vibration = Math.Round(randomGenerator.NextDouble() * (200 - -200) + -200, 2),
                BuzzerIsActive = randomGenerator.Next() > (int.MaxValue / 2)
            };
        }
    }
}