/*
 * BSV - Team #12
 * Winder 2022 - May 20, 2022
 * Application Development III
 *
 * Telemetry Class - Represents telemetry data for a specific container farm.
 */

using Newtonsoft.Json;

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// This class represents telemetry data for a specific container farm.
    /// </summary>
    public class Telemetry
    {
        private double? _latitude;
        private double? _longitude;
        private double? _pitch;
        private double? _roll;
        private Vibration _vibration;
        private bool? _buzzerIsActive;
        private double? _temperature;
        private double? _humidity;
        private int? _moisture;
        private int? _waterLevel;
        private bool? _fanIsActive;
        private bool? _lightIsActive;
        private int? _noise;
        private int? _luminosity;
        private bool? _motion;       
        private bool? _door;
        private bool? _doorIsLocked;

        /// <summary>
        /// Default constructor for the Telemetry class. Initializes all properties to their default values.
        /// </summary>
        public Telemetry()
        {
            _latitude = 0;
            _longitude = 0;
            _pitch = 0;
            _roll = 0;
            _vibration = new Vibration();
            _buzzerIsActive = false;
            _temperature = 0;
            _humidity = 0;
            _moisture = 0;
            _waterLevel = 0;
            _fanIsActive = false;
            _lightIsActive = false;
            _noise = 0;
            _luminosity = 0;
            _motion = false;
            _doorIsLocked = false;
            _door = false;
        }

        /// <summary>
        /// Constructor for the Telemetry class. Initializes all properties to the values passed in.
        /// </summary>
        /// <param name="latitude"> Double representing the latitude value </param>
        /// <param name="longitude"> Double representing the longitude value </param>
        /// <param name="pitch"> Double representing the pitch angle value </param>
        /// <param name="roll"> Double representing the roll angle value </param>
        /// <param name="vibration"> Vibration object representing the vibration value </param>
        /// <param name="buzzerIsActive"> Boolean value for the state of the buzzer. True if buzzer is on, false otherwise. </param>
        /// <param name="temperature"> Double value for temperature. </param>
        /// <param name="humidity"> Double value for humidity. </param>
        /// <param name="moisture"> Integer value for moisture level. </param>
        /// <param name="waterLevel"> Integer value for water level. </param>
        /// <param name="fanIsActive"> Boolean value for fan state. True if fan is on, false otherwise. </param>
        /// <param name="lightIsActive"> Boolean value for light state. True if light is on, false otherwise. </param>
        /// <param name="noise"> Integer value for noise level. </param>
        /// <param name="luminosity"> Integer value for luminosity level. </param>
        /// <param name="motion"> Boolean value for motion detection. </param>
        /// <param name="doorIsLocked"> Boolean value for lock state. </param>
        /// <param name="door"> Boolean value for door state. </param>
        public Telemetry(double latitude, double longitude, double pitch, double roll, Vibration vibration,
            bool buzzerIsActive, double temperature, double humidity, int moisture, int waterLevel,
            bool fanIsActive, bool lightIsActive, int noise, int luminosity, bool motion,
            bool doorIsLocked, bool door)
        {
            _latitude = latitude;
            _longitude = longitude;
            _pitch = pitch;
            _roll = roll;
            _vibration = vibration;
            _buzzerIsActive = buzzerIsActive;
            _temperature = temperature;
            _humidity = humidity;
            _moisture = moisture;
            _waterLevel = waterLevel;
            _fanIsActive = fanIsActive;
            _lightIsActive = lightIsActive;
            _noise = noise;
            _luminosity = luminosity;
            _motion = motion;
            _door = door;
            _doorIsLocked = doorIsLocked;
        }

        /// <summary>
        /// Constructor that takes in an existing container telemetry object in order to duplicate its values.
        /// </summary>
        /// <param name="telemetry"> Takes in an existing telemetry object. </param>
        public Telemetry(Telemetry telemetry)
        {
            Latitude = telemetry.Latitude;
            Longitude = telemetry.Longitude;
            Pitch = telemetry.Pitch;
            Roll = telemetry.Roll;
            Vibration = telemetry.Vibration;
            BuzzerIsActive = telemetry.BuzzerIsActive;
            Temperature = telemetry.Temperature;
            Humidity = telemetry.Humidity;
            Moisture = telemetry.Moisture;
            WaterLevel = telemetry.WaterLevel;
            FanIsActive = telemetry.FanIsActive;
            LightIsActive = telemetry.LightIsActive;
            Noise = telemetry.Noise;
            Luminosity = telemetry.Luminosity;
            Motion = telemetry.Motion;
            Door = telemetry.Door;
            DoorIsLocked = telemetry.DoorIsLocked;
        }
        
        /// <summary>
        /// Public property for latitude.
        /// </summary>
        public double? Latitude
        {
            get => _latitude;
            set => _latitude = value;
        }
        
        /// <summary>
        /// public property for longitude.
        /// </summary>
        public double? Longitude
        {
            get => _longitude;
            set => _longitude = value;
        }
        
        /// <summary>
        /// Public property for pitch.
        /// </summary>
        public double? Pitch
        {
            get => _pitch;
            set => _pitch = value;
        }
        
        /// <summary>
        /// Public property for roll.
        /// </summary>
        public double? Roll
        {
            get => _roll;
            set => _roll = value;
        }
        
        /// <summary>
        /// Public property for vibration.
        /// </summary>
        public Vibration Vibration
        {
            get => _vibration;
            set => _vibration = value;
        }
        
        public bool? BuzzerIsActive
        {
            get => _buzzerIsActive;
            set => _buzzerIsActive = value;
        }
        
        /// <summary>
        /// Public property for temperature.
        /// </summary>
        public double? Temperature
        {
            get => _temperature;
            set => _temperature = value;
        }
        
        /// <summary>
        /// Public property for humidity.
        /// </summary>
        public double? Humidity
        {
            get => _humidity;
            set => _humidity = value;
        }
        
        /// <summary>
        /// Public property for moisture level.
        /// </summary>
        public int? Moisture
        {
            get => _moisture;
            set => _moisture = value;
        }
        
        /// <summary>
        /// Public property for water level.
        /// </summary>
        public int? WaterLevel
        {
            get => _waterLevel;
            set => _waterLevel = value;
        }
        
        /// <summary>
        /// Public property for fan is active.
        /// </summary>
        public bool? FanIsActive
        {
            get => _fanIsActive;
            set => _fanIsActive = value;
        }
        
        /// <summary>
        /// Public property for light is active.
        /// </summary>
        public bool? LightIsActive
        {
            get => _lightIsActive;
            set => _lightIsActive = value;
        }
        
        /// <summary>
        /// Public property for noise level.
        /// </summary>
        public int? Noise
        {
            get => _noise;
            set => _noise = value;
        }
        
        /// <summary>
        /// Public property for light level.
        /// </summary>
        public int? Luminosity
        {
            get => _luminosity;
            set => _luminosity = value;
        }
        
        /// <summary>
        /// Public property for motion detection.
        /// </summary>
        public bool? Motion
        {
            get => _motion;
            set => _motion = value;
        }
        
        /// <summary>
        /// Public property for door lock.
        /// </summary>
        public bool? DoorIsLocked
        {
            get => _doorIsLocked;
            set => _doorIsLocked = value;
        }
        
        /// <summary>
        /// Public property for door status.
        /// </summary>
        public bool? Door
        {
            get => _door;
            set => _door = value;
        }
        
        /// <summary>
        /// Method to serialize the object.
        /// </summary>
        /// <returns> Serialized JSON representing the instance of this object. </returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        /// <summary>
        /// Method to deserialize a string into an instance of this object.
        /// </summary>
        /// <param name="json"> Takes in a string in JSON format. </param>
        /// <returns> Returns an object of type Telemetry </returns>
        public static Telemetry FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Telemetry>(json);
        }
        
        /// <summary>
        /// Override of the ToString method to represent the Telemetry object as a string.
        /// </summary>
        /// <returns> String representation of the Telemetry class. </returns>
        public override string ToString()
        {
            return $"Telemetry Data:\n\tLatitude: {Latitude}\n\tLongitude: {Longitude}\n\tPitch: {Pitch}\n\tRoll: {Roll}\n\tVibration: {Vibration}\n\tBuzzerIsActive: {BuzzerIsActive}\n\tTemperature: {Temperature}\n\tHumidity: {Humidity}\n\tMoistureLevel: {Moisture}\n\tWaterLevel: {WaterLevel}\n\tFanIsActive: {FanIsActive}\n\tLightIsActive: {LightIsActive}\n\tNoiseLevel: {Noise}\n\tLuminosityLevel: {Luminosity}\n\tMotionIsDetected: {Motion}\n\tDoorIsLocked: {DoorIsLocked}\n\tDoorIsClosed: {Door}";
        }
    }
}