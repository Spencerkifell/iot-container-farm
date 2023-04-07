/*
 * BSV - Team #12
 * Winter 2022 - May 2, 2022
 * Application Developpment 3
 * 
 * Security Subsystem class contains all components related to the security and their functionality in the iot farm.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// This class stores all data related to the security subsystem. 
    /// Each property contains a public getter and setter to retrieve and store all data.
    /// </summary>
    public class SecuritySubsystem
    {
        private int _noiseLevel;
        private int _luminosityLevel;
        private bool _motionIsDetected;
        private bool _doorIsLocked;
        private bool _doorIsClosed;
        private bool _buzzerIsActive;
        /// <summary>
        /// Default constructor for the SecuritySubsystem class.
        /// </summary>
        public SecuritySubsystem()
        {
            _noiseLevel = 0;
            _luminosityLevel = 0;
            _motionIsDetected = false;
            _doorIsLocked = false;
            _doorIsClosed = false;
            _buzzerIsActive = false;
        }
        /// <summary>
        /// Constructor for the security subsystem. Populates all backing fields
        /// </summary>
        /// <param name="noise">Integer value for noise level. </param> 
        /// <param name="luminosity">Integer value for luminosity level. </param>
        /// <param name="motion">Boolean value for motion detection. </param>
        /// <param name="locked">Boolean value for lock state. </param>
        /// <param name="door">Boolean value for door state.</param>
        /// <param name="buzzer">Boolean value for buzzer state.</param>
        public SecuritySubsystem(int noise, int luminosity, bool motion, bool locked, bool door, bool buzzer)
        {
            _noiseLevel = noise;
            _luminosityLevel = luminosity;
            _motionIsDetected = motion;
            _doorIsLocked = locked;
            _doorIsClosed = door;
            _buzzerIsActive = buzzer;
        }
        /// <summary>
        /// Gets and sets the double value for the noise level of the security subsystem.
        /// </summary>
        public int Noise
        {
            get => _noiseLevel;
            set => _noiseLevel = value;
        }
        /// <summary>
        /// Gets and sets the double value for the luminosty level of the security subsystem.
        /// </summary>
        public int Luminosity
        {
            get => _luminosityLevel;
            set => _luminosityLevel = value;
        }
        /// <summary>
        /// Gets and sets the boolean value for the motion detection of the security subsystem.
        /// </summary>
        public bool Motion
        {
            get => _motionIsDetected;
            set => _motionIsDetected = value;
        }
        /// <summary>
        /// Gets and sets the boolean value for the lock state of the security subsystem.
        /// </summary>
        public bool Lock
        {
            get => _doorIsLocked;
            set => _doorIsLocked = value;
        }
        /// <summary>
        /// Gets and sets the boolean value for the door state of the security subsystem.
        /// </summary>
        public bool Door
        {
            get => _doorIsClosed;
            set => _doorIsClosed = value;
        }
        /// <summary>
        /// Gets and sets the boolean value for the buzzer state of the security subsystem.
        /// </summary>
        public bool Buzzer
        {
            get => _buzzerIsActive;
            set => _buzzerIsActive = value;
        }
        /// <summary>
        ///  Creates a string representation of the security subsytem.
        /// </summary>
        /// <returns>String representation of the security subsystem.</returns>
        public override string ToString()
        {
            return $"Security Subsystem:\n\tNoise Level: {_noiseLevel}\n\tLuminosity Level: {_luminosityLevel}\n\tMotion: {_motionIsDetected}\n\tDoor Lock: {_doorIsLocked}\n\tDoor State: {_doorIsClosed}\n\tBuzzer State: {_buzzerIsActive}";
        }
    }
}
