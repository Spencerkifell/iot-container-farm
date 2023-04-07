/*
 * BSV - Team #12
 * Winter 2022 - May 2, 2022
 * Application Developpment 3
 * 
 * Security Repository is used to store the security subsystem.
 */

using System;
using BSV_IOT_FARM.Models;

namespace BSV_IOT_FARM.Repos.Deprecated
{
    /// <summary>
    /// Strore security subsystem.
    /// </summary>
    class SecurityRepo
    {
        private SecuritySubsystem _securitySubsystem;
        /// <summary>
        /// Default constructor, create dummy data for security subsystem.
        /// </summary>
        public SecurityRepo()
        {
            _securitySubsystem = new SecuritySubsystem(150, 250, false, false, true, false);
        }
        /// <summary>
        /// Gets and sets the security subsystem.
        /// </summary>
        public SecuritySubsystem SecuritySubsystem
        {
            get => _securitySubsystem;
            set => _securitySubsystem = value;
        }
        /// <summary>
        /// Creates a new SecuritySubsystem with dummy data.
        /// </summary>
        /// <returns> A new Security Subsystem populated with dummy data. </returns>
        public static SecuritySubsystem GenerateDummyData()
        {
            var randomGenerator = new Random();

            return new SecuritySubsystem()
            {
                Noise = randomGenerator.Next(20, 30),
                Luminosity = randomGenerator.Next(20, 30),
                Motion = randomGenerator.Next() > (int.MaxValue / 2),
                Lock = randomGenerator.Next() > (int.MaxValue / 2),
                Door = randomGenerator.Next() > (int.MaxValue / 2),
                Buzzer = randomGenerator.Next() > (int.MaxValue / 2)
            };
        }
    }
}
