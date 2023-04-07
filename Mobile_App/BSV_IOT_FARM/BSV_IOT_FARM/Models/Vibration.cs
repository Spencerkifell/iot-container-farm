/*
 * BSV - Team #12
 * Winder 2022 - May 20, 2022
 * Application Development III
 *
 * Vibration Class - Represents a vibration object to be contained in a Telemetry object.
 */

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// This class represents a vibration object to be contained in a Telemetry object.
    /// </summary>
    public class Vibration
    {
        private double _x;
        private double _y;
        private double _z;
        
        /// <summary>
        /// Default constructor for the vibration class. Sets the values to default values.
        /// </summary>
        public Vibration()
        {
            X = double.NaN;
            Y = double.NaN;
            Z = double.NaN;
        }
        
        /// <summary>
        /// Constructor for the vibration class that sets the values to the given values.
        /// </summary>
        /// <param name="x"> Represents X Axis Vibration </param>
        /// <param name="y"> Represents Y Axis Vibration </param>
        /// <param name="z"> Represents Z Axis Vibration </param>
        public Vibration(double x, double y, double z)
        {
            _x = x;
            _x = y;
            _z = z;
        }
        
        /// <summary>
        /// Public property for the X Axis Vibration.
        /// </summary>
        public double X
        {
            get => _x;
            set => _x = value;
        }
        
        /// <summary>
        /// Public property for the Y Axis Vibration.
        /// </summary>
        public double Y
        {
            get => _y;
            set => _y = value;
        }
        
        /// <summary>
        /// Public property for the Z Axis Vibration.
        /// </summary>
        public double Z
        {
            get => _z;
            set => _z = value;
        }
        
        /// <summary>
        /// Override for the ToString method to return string representation of the vibration class.
        /// </summary>
        /// <returns> Returns string representation of the vibration class. </returns>
        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }
}