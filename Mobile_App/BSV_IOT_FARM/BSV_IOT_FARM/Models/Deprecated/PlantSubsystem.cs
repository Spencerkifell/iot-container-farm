/*
 * BSV - Team #12
 * Winter 2022 - May 2, 2022
 * Application Development 3
 * 
 * Plant Subsystem class contains all components related to the plants and their condition of the iot farm.
 */

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// This class stores all data related to the plant subsystem. 
    /// Each property contains a public getter and setter to retrieve and store all data.
    /// </summary>
    public class PlantSubsystem
    {
        private double _temperature;
        private double _humidity;
        private int _moistureLevel;
        private int _waterLevel;
        private bool _fanIsActive;
        private bool _lightIsActive;

        /// <summary>
        /// Default constructor for the PlantSubsystem class.
        /// </summary>
        public PlantSubsystem()
        {
            _temperature = 0;
            _humidity = 0;
            _moistureLevel = 0;
            _waterLevel = 0;
            _fanIsActive = false;
            _lightIsActive = false;
        }

        /// <summary>
        /// Constructor for plant subsystem. Populates all backing fields.
        /// </summary>
        /// <param name="temp"> Double value for temperature. </param>
        /// <param name="humi"> Double value for humidity. </param>
        /// <param name="moisture"> Integer value for moisture level. </param>
        /// <param name="water"> Integer value for water level. </param>
        /// <param name="fan"> Boolean value for fan state. True if fan is on, false otherwise. </param>
        /// <param name="light"> Boolean value for light state. True if light is on, false otherwise. </param>
        public PlantSubsystem(double temp, double humi, int moisture, int water, bool fan, bool light)
        {
            _temperature = temp;
            _humidity = humi;
            _moistureLevel = moisture;
            _waterLevel = water;
            _fanIsActive = fan;
            _lightIsActive = light;
        }

        /// <summary>
        /// Gets and sets the double value for the temperature of the plant subsystem.
        /// </summary>
        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        /// <summary>
        /// Gets and sets the double value for the humidity of the plant subsystem.
        /// </summary>
        public double Humidity
        {
            get => _humidity;
            set => _humidity = value;
        }

        /// <summary>
        /// Gets and sets the integer value for the moisture level of the plant subsystem.
        /// </summary>
        public int MoistureLevel
        {
            get => _moistureLevel;
            set => _moistureLevel = value;
        }

        /// <summary>
        /// Gets and sets the integer value for the water level of the plant subsystem.
        /// </summary>
        public int WaterLevel
        {
            get => _waterLevel;
            set => _waterLevel = value;
        }

        /// <summary>
        /// Gets and sets the boolean value for the fan state of the plant subsystem.
        /// </summary>
        public bool FanIsActive
        {
            get => _fanIsActive;
            set => _fanIsActive = value;
        }

        /// <summary>
        /// Gets and sets the boolean value for the light state of the plant subsystem.
        /// </summary>
        public bool LightIsActive
        {
            get => _lightIsActive;
            set => _lightIsActive = value;
        }

        /// <summary>
        ///  Creates a string representation of the plant subsytem.
        /// </summary>
        /// <returns>String representation of the plant subsystem.</returns>
        public override string ToString()
        {
            return string.Format("Temperature: {0}\n" +"Humidity: {1}\n" +
                "Moisture Level: {2}\n" + "Water Level: {3}\n" +
                "Fan State: {4}\n" + "Light State: {5}", 
                Temperature, Humidity, MoistureLevel, WaterLevel, 
                FanIsActive ? "ON" : "OFF", LightIsActive ? "ON":"OFF");
        }
    }
}
