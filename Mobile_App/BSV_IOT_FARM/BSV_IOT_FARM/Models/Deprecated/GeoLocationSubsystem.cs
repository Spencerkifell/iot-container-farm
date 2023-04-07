/*
 * BSV - Team #12
 * Winder 2022 - May 2, 2022
 * Application Development III
 *
 * GeoLocation Subsystem class contains all fields and methods related to the GeoLocation subsystem.
 */

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// This class stores all data related to the geolocation subsystem. 
    /// Each property contains a public getter and setter to retrieve and store all data.
    /// </summary>
    public class GeoLocationSubsystem
    {
        private double _latitude;
        private double _longitude;
        private double _pitch;
        private double _roll;
        private double _vibration;
        private bool _buzzerIsActive;

        /// <summary>
        /// Default constructor for GeoLocationSubsystem. Initializes all fields to default values.
        /// </summary>
        public GeoLocationSubsystem()
        {
            _latitude = 0;
            _longitude = 0;
            _pitch = 0;
            _roll = 0;
            _vibration = 0;
            _buzzerIsActive = false;
        }
        
        /// <summary>
        /// Constructor for the GeoLocationSubsystem. Populates all of the backing fields.
        /// </summary>
        /// <param name="latitude"> Double representing the latitude value </param>
        /// <param name="longitude"> Double representing the longitude value </param>
        /// <param name="pitch"> Double representing the pitch angle value </param>
        /// <param name="roll"> Double representing the roll angle value </param>
        /// <param name="vibration"> Double representing the vibration value </param>
        /// <param name="buzzerIsActive"> Boolean value for the state of the buzzer. True if buzzer is on, false otherwise. </param>
        public GeoLocationSubsystem(double latitude, double longitude, double pitch, double roll, double vibration, bool buzzerIsActive)
        {
            _latitude = latitude;
            _longitude = longitude;
            _pitch = pitch;
            _roll = roll;
            _vibration = vibration;
            _buzzerIsActive = buzzerIsActive;
        }
        
        /// <summary>
        /// Property for the latitude value.
        /// </summary>
        public double Latitude
        {
            get => _latitude;
            set => _latitude = value;
        }
        
        /// <summary>
        /// Property for the longitude value.
        /// </summary>
        public double Longitude
        {
            get => _longitude;
            set => _longitude = value;
        }
        
        /// <summary>
        /// Property for the pitch angle value.
        /// </summary>
        public double Pitch
        {
            get => _pitch;
            set => _pitch = value;
        }
        
        /// <summary>
        /// Property for the roll angle value.
        /// </summary>
        public double Roll
        {
            get => _roll;
            set => _roll = value;
        }
        
        /// <summary>
        /// Property for the vibration value.
        /// </summary>
        public double Vibration
        {
            get => _vibration;
            set => _vibration = value;
        }
        
        /// <summary>
        /// Property representing the state of the buzzer. 
        /// </summary>
        public bool BuzzerIsActive
        {
            get => _buzzerIsActive;
            set => _buzzerIsActive = value;
        }
        
        /// <summary>
        /// Override of the ToString method to represent the GeoLocationSubsystem object as a string.
        /// </summary>
        /// <returns> String representation of the Geo Location Subsystem </returns>
        public override string ToString()
        {
            return $"Geo Location Subsystem:\n\tLatitude: {_latitude}\n\tLongitude: {_longitude}\n\tPitch: {_pitch}\n\tRoll: {_roll}\n\tVibration: {_vibration}\n\tBuzzer: {_buzzerIsActive}";
        }
    }
}