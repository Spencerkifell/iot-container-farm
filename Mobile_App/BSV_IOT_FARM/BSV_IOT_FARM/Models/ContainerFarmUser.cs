/*
 * BSV - Team #12
 * Winder 2022 - May 20, 2022
 * Application Development III
 *
 * This class is used to represent the many to many relationship between users and a container farm.
 */

using SQLiteNetExtensions.Attributes;

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// This class is used to represent the many to many relationship between users and a container farm.
    /// </summary>
    public class ContainerFarmUser
    {
        [ForeignKey(typeof(User))]
        public int UserId { get; set; }
        
        [ForeignKey(typeof(ContainerFarm))]
        public int ContainerFarmId { get; set; }
    }
}