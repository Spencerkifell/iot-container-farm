/*
 * BSV - Team #12
 * Winter 2022 - May 2, 2022
 * Application Development 3
 * 
 * Plant Repository is used to store the plant subsystem.
 */

using System;
using BSV_IOT_FARM.Models;

namespace BSV_IOT_FARM.Repos.Deprecated
{
    /// <summary>
    /// Store plant subsystem.
    /// </summary>
    public class PlantRepo
    {
        private PlantSubsystem _plantSubsystem;
        /// <summary>
        /// Default constructor, create dummy data for plant subsystem.
        /// </summary>
        public PlantRepo()
        {
            _plantSubsystem = new PlantSubsystem(25, 30, 234, 23, false, true);
        }

        /// <summary>
        /// Gets and sets the plant subsystem.
        /// </summary>
        public PlantSubsystem PlantSystem
        {
            get => _plantSubsystem;
            set => _plantSubsystem = value;
        }
        
        /// <summary>
        /// Creates a new PlantSubsystem with dummy data.
        /// </summary>
        /// <returns> A new Plant Subsystem populated with dummy data. </returns>
        public static PlantSubsystem GenerateDummyData()
        {
            var randomGenerator = new Random();

            return new PlantSubsystem()
            {
                Temperature = randomGenerator.Next(20, 30),
                Humidity = randomGenerator.Next(0, 100),
                MoistureLevel = randomGenerator.Next(0, 100),
                WaterLevel = randomGenerator.Next(0, 100),
                LightIsActive = randomGenerator.Next() > (int.MaxValue / 2),
                FanIsActive = randomGenerator.Next() > (int.MaxValue / 2)
            };
        }
    }
}
