/*
 * BSV - Team #12
 * Winter 2022 - May 20, 2022
 * Application Development 3
 * 
 * Container Farm View Model used to store a container farm.
 */

using System;
using System.Collections.ObjectModel;
using Azure.Messaging.EventHubs;
using BSV_IOT_FARM.Models;

namespace BSV_IOT_FARM.ViewModels
{
    /// <summary>
    /// This view model is used to store a container farm and communicate with the views.
    /// </summary>
    public class ContainerFarmViewModel : ViewModel
    {
        private ContainerFarm _containerFarm;

        private bool _isNameInvalid;
        private bool _isDescriptionInvalid;
        private bool _isEventHubConnectionStringInvalid;
        private bool _isDeviceIdInvalid;
        private bool _isIotHubConnectionStringInvalid;
        private bool _isEventHubNameInvalid;

        /// <summary>
        /// Constructor for the container farm view model.
        /// </summary>
        /// <param name="containerFarm"> Takes in an existing container farm object </param>
        public ContainerFarmViewModel(ContainerFarm containerFarm)
        {
            _containerFarm = containerFarm;
            _containerFarm.TelemetryHistory = new ObservableCollection<Telemetry>();
        }

        #region Properties

        /// <summary>
        /// Public property representing if the container farm is new or not.
        /// </summary>
        public bool IsNew { get; set; } = true;
            
        /// <summary>
        /// Public property representing the container farm's ID.
        /// </summary>
        public int Id => _containerFarm.Id;

        /// <summary>
        /// Public property representing the container farm object within the view model.
        /// </summary>
        public ContainerFarm ContainerFarm
        {
            get => _containerFarm;
            set
            {
                if (value == _containerFarm)
                    return;
                
                _containerFarm = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the Name of the Container Farm
        /// </summary>
        public string Name
        {
            get => _containerFarm.Name;
            set
            {
                if (value == _containerFarm.Name)
                    return;
                
                _containerFarm.Name = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the Description of the Container Farm
        /// </summary>
        public string Description
        {
            get => _containerFarm.Description;
            set
            {
                if (value == _containerFarm.Description)
                    return;
                
                _containerFarm.Description = value;          
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the event hub connection string of the Container Farm to properly communicate with the event hub
        /// </summary>
        public string EventHubConnectionString
        {
            get => _containerFarm.EventHubConnectionString;
            set
            {
                if (value == _containerFarm.EventHubConnectionString)
                    return;
                
                _containerFarm.EventHubConnectionString = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the event hub name of the Container Farm to properly connect to the event hub
        /// </summary>
        public string EventHubName
        {
            get => _containerFarm.EventHubName;
            set
            {
                if (value == _containerFarm.EventHubName)
                    return;
                
                _containerFarm.EventHubName = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the device id of the Container Farm to properly communicate with the Device Twins
        /// </summary>
        public string DeviceId
        {
            get => _containerFarm.DeviceId;
            set
            {
                if (value == _containerFarm.DeviceId)
                    return;
                
                _containerFarm.DeviceId = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the device connection string of the Container Farm to properly communicate with the Device Twins
        /// </summary>
        public string IotHubConnectionString
        {
            get => _containerFarm.IotHubConnectionString;
            set
            {
                if (value == _containerFarm.IotHubConnectionString)
                    return;
                
                _containerFarm.IotHubConnectionString = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property representing the Telemetry object.
        /// </summary>
        public Telemetry Telemetry
        {
            get => _containerFarm.Telemetry;
            set
            {
                if (value == _containerFarm.Telemetry)
                    return;
                
                _containerFarm.Telemetry = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property representing the users associated with the container farm.
        /// </summary>
        public ObservableCollection<User> Users
        {
            get => _containerFarm.Users;
            set
            {
                if (value == _containerFarm.Users)
                    return;
                
                _containerFarm.Users = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing if the user is able to refresh their data from the Event Hub.
        /// </summary>
        public bool CanRefresh
        {
            get => _containerFarm.CanRefresh;
            set
            {
                if (value == _containerFarm.CanRefresh)
                    return;

                _containerFarm.CanRefresh = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property representing if the existing thread should be killed.
        /// </summary>
        public bool ShouldKillTask
        {
            get => _containerFarm.ShouldKillTask;
            set
            {
                if (value == _containerFarm.ShouldKillTask)
                    return;
                
                _containerFarm.ShouldKillTask = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property representing the name validation for the Container Farm to be used when validating fields within the form page.
        /// </summary>
        public bool IsNameInvalid
        {
            get => _isNameInvalid;
            set
            {
                if (value == _isNameInvalid)
                    return;

                _isNameInvalid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the description validation for the Container Farm to be used when validating fields within the form page.
        /// </summary>
        public bool IsDescriptionInvalid
        {
            get => _isDescriptionInvalid;
            set
            {
                if (value == _isDescriptionInvalid)
                    return;
                
                _isDescriptionInvalid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the connection string validation for the Container Farm to be used when validating fields within the form page.
        /// </summary>
        public bool IsEventHubConnectionStringInvalid
        {
            get => _isEventHubConnectionStringInvalid;
            set
            {
                if (value == _isEventHubConnectionStringInvalid)
                    return;
                
                _isEventHubConnectionStringInvalid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the event hub name validation for the Container Farm to be used when validating fields within the form page.
        /// </summary>
        public bool IsEventHubNameInvalid
        {
            get => _isEventHubNameInvalid;
            set
            {
                if (value == _isEventHubNameInvalid)
                    return;
                
                _isEventHubNameInvalid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the device id validation for the Container Farm to be used when validating fields within the form page.
        /// </summary>
        public bool IsDeviceIdInvalid
        {
            get => _isDeviceIdInvalid;
            set
            {
                if (value == _isDeviceIdInvalid)
                    return;
                
                _isDeviceIdInvalid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the device key validation for the Container Farm to be used when validating fields within the form page.
        /// </summary>
        public bool IsIotHubConnectionStringInvalid
        {
            get => _isIotHubConnectionStringInvalid;
            set
            {
                if (value == _isIotHubConnectionStringInvalid)
                    return;
                
                _isIotHubConnectionStringInvalid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Calculated field to establish if the current Event Hub Connection is invalid.
        /// </summary>
        public bool IsConnectionInvalid
        {
            get
            {
                try
                {
                    // If the connection is invalid, then creating a new EventHubClient will throw an exception.
                    var eventHubConnection = new EventHubConnection(EventHubConnectionString, EventHubName);
                }
                catch (Exception)
                {
                    // If an exception is caught, then the connection is invalid so return true.
                    return true;
                }

                return false;
            }
        }

        public double TelemetryInterval
        {
            get => _containerFarm.TelemetryInterval;
            set
            {
                if (value == _containerFarm.TelemetryInterval)
                    return;

                _containerFarm.TelemetryInterval = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Calculated property that uses all of the validation properties to computer whether or not the form data is valid or not.
        /// </summary>
        public bool IsInvalid => IsNameInvalid || IsDescriptionInvalid || IsEventHubConnectionStringInvalid || IsEventHubNameInvalid || IsDeviceIdInvalid || IsIotHubConnectionStringInvalid;
        
        #endregion
    }
}