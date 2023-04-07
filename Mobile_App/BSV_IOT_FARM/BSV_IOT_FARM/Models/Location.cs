using Xamarin.Forms.Maps;

namespace BSV_IOT_FARM.Models
{
    public class Location
    {
        public Location(double longitude, double latitude)
        {
            Position = new Position(latitude, longitude);
        }
        
        public Position Position { get; set; }
    }
}