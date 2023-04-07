/*
 * BSV - Team #12
 * Winder 2022 - May 20, 2022
 * Application Development III
 *
 * User Class - Represents the user table in the database.
 */

using System.Collections.ObjectModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// Enum for the different type of user roles.
    /// </summary>
    public enum UserRole
    {
        FarmTechnician,
        FleetManager
    }
    
    /// <summary>
    /// This class is used to store user information.
    /// </summary>
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public int Role { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        [Ignore]
        public string RoleString => Role == (int) UserRole.FleetManager ? "Fleet Manager" : "Farm Technician";

        [ManyToMany(typeof(ContainerFarmUser), CascadeOperations = CascadeOperation.CascadeDelete)]
        public ObservableCollection<ContainerFarm> ContainerFarms { get; set; }

        /// <summary>
        /// Default constructor that initializes the user to a default state.
        /// </summary>
        public User()
        {
            Role = (int) UserRole.FarmTechnician;
            FirstName = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ContainerFarms = new ObservableCollection<ContainerFarm>();
        }
        
        /// <summary>
        /// Constructor that takes in the user's data and creates a new user with the given data.
        /// </summary>
        /// <param name="role"> Value representing the user's role. </param>
        /// <param name="firstName"> Value representing the user's first name. </param>
        /// <param name="username"> Value representing the user's username. </param>
        /// <param name="password"> Value representing the user's password. </param>
        /// <param name="containerFarms"> Value representing the user's list of container farms. </param>
        public User(int role, string firstName, string username, string password, ObservableCollection<ContainerFarm> containerFarms)
        {
            Role = role;
            FirstName = firstName;
            Username = username;
            Password = password;
            ContainerFarms = containerFarms;
        }

        /// <summary>
        /// Constructor that takes in an existing user and creates a new user with the same data.
        /// </summary>
        /// <param name="user"> Takes in an existing user object. </param>
        public User(User user)
        {
            Role = user.Role;
            FirstName = user.FirstName;
            Username = user.Username;
            Password = user.Password;
            ContainerFarms = user.ContainerFarms;
        }
    }
}