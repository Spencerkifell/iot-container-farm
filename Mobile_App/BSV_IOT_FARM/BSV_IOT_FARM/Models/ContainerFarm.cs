/*
 * BSV - Team #12
 * Winder 2022 - May 20, 2022
 * Application Development III
 *
 * Container Farm Class - Represents a container farm that encapsulates all the sub-systems.
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace BSV_IOT_FARM.Models
{
    /// <summary>
    /// This class is used to store container farms and each of their subsystems data.
    /// </summary>
    public class ContainerFarm
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string EventHubConnectionString { get; set; }
        public string EventHubName { get; set; }
        public string DeviceId { get; set; }
        public string IotHubConnectionString { get; set; }
        public double TelemetryInterval { get; set; }

        [ManyToMany(typeof(ContainerFarmUser), CascadeOperations = CascadeOperation.CascadeDelete)]
        public ObservableCollection<User> Users { get; set; }
        
        // Values that are not stored in the database...
        
        [Ignore]
        public Telemetry Telemetry { get; set; }
        
        [Ignore]
        public ObservableCollection<Telemetry> TelemetryHistory { get; set; }

        [Ignore] public bool CanRefresh { get; set; }

        [Ignore] public bool ShouldKillTask { get; set; }

        /// <summary>
        /// Default constructor for the container farm.
        /// </summary>
        public ContainerFarm()
        {
            Name = string.Empty;
            Description = string.Empty;
            EventHubConnectionString = string.Empty;
            EventHubName = string.Empty;
            DeviceId = string.Empty;
            IotHubConnectionString = string.Empty;
            Users = new ObservableCollection<User>();
            Telemetry = new Telemetry();
            TelemetryHistory = new ObservableCollection<Telemetry>();
            CanRefresh = true;
            ShouldKillTask = false;
        }
        
        /// <summary>
        /// Constructor for the container farm, takes in a name, description and telemetry.
        /// </summary>
        /// <param name="name"> Takes in the string value representing the name of the container. </param>
        /// <param name="description"> Takes in the string value representing the description of the container. </param>
        /// <param name="eventHubConnectionString"> Takes in the connection string of the event hub to read partition data. </param>
        /// <param name="eventHubName"> Takes in the name of the Azure Event Hub to process data. </param>
        /// <param name="deviceId"> Takes in the device id of the device in order to communicate with Device Twins. </param>
        /// <param name="iotHubConnectionString"> Takes in the connection string of the IoT Hub to communicate with Device Twins. </param>
        /// <param name="users"> Takes in the list of users that are allowed to access the container. </param>
        /// <param name="telemetry"> Takes in the telemetry object to store the telemetry data. </param>
        /// <param name="canRefresh"> Takes in a boolean to be utilized when checking if the user can run the thread to dynamically update their data. </param>
        /// <param name="shouldKillTask"> To be utilized in the Azure Repo (Background thead) to see if the thread needs to be killed. </param>
        public ContainerFarm(string name, string description, string eventHubConnectionString, string eventHubName, string deviceId, string iotHubConnectionString, ObservableCollection<User> users, Telemetry telemetry, bool canRefresh, bool shouldKillTask)
        {
            Name = name;
            Description = description;
            EventHubConnectionString = eventHubConnectionString;
            EventHubName = eventHubName;
            DeviceId = deviceId;
            IotHubConnectionString = iotHubConnectionString;
            Users = users;
            Telemetry = telemetry;
            CanRefresh = canRefresh;
            ShouldKillTask = shouldKillTask;
        }

        /// <summary>
        /// Constructor that takes in an existing container farm in order to duplicate its values.
        /// </summary>
        /// <param name="containerFarm"> Takes in an existing container farm</param>
        public ContainerFarm(ContainerFarm containerFarm)
        {
            Id = containerFarm.Id;
            Name = containerFarm.Name;
            Description = containerFarm.Description;
            EventHubConnectionString = containerFarm.EventHubConnectionString;
            EventHubName = containerFarm.EventHubName;
            DeviceId = containerFarm.DeviceId;
            IotHubConnectionString = containerFarm.IotHubConnectionString;
            Users = containerFarm.Users;
            Telemetry = containerFarm.Telemetry;
            CanRefresh = containerFarm.CanRefresh;
            ShouldKillTask = containerFarm.ShouldKillTask;
        }
    }
}