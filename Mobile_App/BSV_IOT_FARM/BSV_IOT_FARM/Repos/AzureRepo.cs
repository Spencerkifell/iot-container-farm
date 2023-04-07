/*
 * BSV - Team #12
 * Winter 2022 - May 20, 2022
 * Application Development III
 *
 * Azure Repository Class - Meant to be used as a repository for all Azure related data.
 */

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Consumer;
using BSV_IOT_FARM.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace BSV_IOT_FARM.Repos
{
    /// <summary>
    /// Class meant to be used as a repository for all Azure related data.
    /// </summary>
    public class AzureRepo
    {
        private const string INTERVAL_KEY = "telemetryInterval";
        private const string LIGHT_KEY = "lightState";
        private const string FAN_KEY = "fanState";
        private const string BUZZER_KEY = "buzzerState";
        private const string DOOR_LOCK_KEY = "doorLockState";

        /// <summary>
        /// Method that creates a connection to the Azure Event Hub and starts a consumer to read the partitions for the latest telemetry messages. Ideally, this should be run in a background thread.
        /// </summary>
        /// <param name="containerFarm"> Takes in an existing container farm to access connection values and modify telemetry data. </param>
        /// <exception cref="ArgumentException"> Will throw a custom argument exception dependent on the error thrown (Cancellation Token/Network Connectivity/Credentials/Other) </exception>
        public async Task RetrieveTelemetryData(ContainerFarm containerFarm)
        {
            const string CONSUMER_GROUP = EventHubConsumerClient.DefaultConsumerGroupName;

            // Assure that the connection string and associated Event Hub name aren't empty.
            if (containerFarm.EventHubName == null || containerFarm.EventHubConnectionString == null)
                throw new ArgumentException("Please make sure that your container farm has an associated Event Hub connection string and name.");

            // Create the consumer
            var consumer = new EventHubConsumerClient(CONSUMER_GROUP, containerFarm.EventHubConnectionString, containerFarm.EventHubName);
            
            try
            {
                containerFarm.CanRefresh = false;
                // We will create a cancellation token so that we can notify the user that data was not able to be retrieved within a prolonged amount of time.
                using var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

                // To get the latest set of data, we use EventPosition.Latest, this way we only retrieve the latest message rather than 
                var eventPosition = EventPosition.Latest;

                // Since the dataset may be large, we should look through all the partitions for data being sent back. 
                // This logic is that each device uses a different hub rather than different devices on a hub, that way it can be set up by anyone.
                foreach (var partition in await consumer.GetPartitionIdsAsync(cancellationTokenSource.Token))
                {
                    await foreach (var partitionEvent in consumer.ReadEventsFromPartitionAsync(partition, eventPosition, cancellationTokenSource.Token))
                    {
                        if (containerFarm.ShouldKillTask)
                        {
                            containerFarm.CanRefresh = true;
                            return;
                        }

                        // This will get us the raw data payload as a string that we will 
                        var data = partitionEvent.Data.EventBody.ToString();

                        // Now we want to parse the data which comes in as a string json object. 
                        var telemetryData = JsonConvert.DeserializeObject<Telemetry>(data);
                        

                        // Everytime we are able to retrieve data, we will extend the life time of the cancellation token so that we can still retrieve data.
                        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

                        containerFarm.TelemetryHistory.Add(telemetryData);
                        containerFarm.Telemetry = telemetryData;
                    }
                }
            }
            // Exception that will be thrown when the cancellation token expires.
            catch (TaskCanceledException)
            {
                throw new ArgumentException("Took too long to get events");
            }
            // Exception that will be thrown when there are any network issues.
            catch (SocketException)
            {
                throw new ArgumentException("Network connection is no longer available. Please check your internet connection and try again.");
            }
            // Exception that will be thrown if another issues occurs. 
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
            finally
            {
                // Since the task has ended, we can now allow the user to refresh the data.
                containerFarm.CanRefresh = true;
                await consumer.CloseAsync();
            }
        }

        /// <summary>
        /// Enum to represent the different actuator states.
        /// </summary>
        public enum DeviceTwinState
        {
            LightOn,
            LightOff,
            FanOn,
            FanOff,
            BuzzerOn,
            BuzzerOff,
            DoorOpen,
            DoorClose
        }
        private Dictionary<DeviceTwinState, string> actuatorPatches = new Dictionary<DeviceTwinState, string>
        {
            { DeviceTwinState.LightOn, "lightState:\'on\'" },
            { DeviceTwinState.LightOff, "lightState:\'off\'" },
            { DeviceTwinState.FanOn, "fanState:\'on\'" },
            { DeviceTwinState.FanOff, "fanState:\'off\'" },
            { DeviceTwinState.BuzzerOn, "buzzerState:\'on\'" },
            { DeviceTwinState.BuzzerOff, "buzzerState:\'off\'" },
            { DeviceTwinState.DoorOpen, "doorLockState:\'open\'" },
            { DeviceTwinState.DoorClose, "doorLockState:\'close\'" }
        };

        /// <summary>
        /// Retrieves the actuator states from the device twin and sets the container farm properties accordingly.
        /// </summary>
        /// <param name="containerFarm">Takes in an existing container farm to access connection values.</param>
        /// <returns>Task for asyncronous operation.</returns>
        public async Task RetrieveReportedProperties(ContainerFarm containerFarm)
        {
            try
            {
                RegistryManager registryManager = RegistryManager.CreateFromConnectionString(containerFarm.IotHubConnectionString);
                Twin twin = await registryManager.GetTwinAsync(containerFarm.DeviceId);

                //Set actuator data
                
                if(twin.Properties.Reported.Contains(LIGHT_KEY))
                    containerFarm.Telemetry.LightIsActive = twin.Properties.Reported[LIGHT_KEY] == "on";
                if (twin.Properties.Reported.Contains(FAN_KEY))
                    containerFarm.Telemetry.FanIsActive = twin.Properties.Reported[FAN_KEY] == "on";
                if (twin.Properties.Reported.Contains(BUZZER_KEY))
                    containerFarm.Telemetry.BuzzerIsActive = twin.Properties.Reported[BUZZER_KEY] == "on";
                if (twin.Properties.Reported.Contains(DOOR_LOCK_KEY))
                    containerFarm.Telemetry.DoorIsLocked = twin.Properties.Reported[DOOR_LOCK_KEY] == "close";
                
                //set interval data
                if (twin.Properties.Reported.Contains(INTERVAL_KEY))
                    containerFarm.TelemetryInterval = twin.Properties.Reported[INTERVAL_KEY];
            }
            catch (SocketException)
            {
                throw new ArgumentException("Cannot retrieve device twin. Please check your internet connection and try again.");
            }
            catch (Exception e)
            {
                throw new ArgumentException("Cannot retrieve device twin. Please verify that your connection string and device ID are valid.");
            }
        }

        /// <summary>
        /// Sets the desired property with the given device twin state.
        /// </summary>
        /// <param name="containerFarm">Takes in an existing container farm to access connection values</param>
        /// <param name="state">Actuator state to be modified.</param>
        /// <returns>Task for asyncronous operation.</returns>
        public async Task PatchActuatorDeviceTwin(ContainerFarm containerFarm, DeviceTwinState state)
        {
            try
            {
                var registryManager = RegistryManager.CreateFromConnectionString(containerFarm.IotHubConnectionString);
                var twin = await registryManager.GetTwinAsync(containerFarm.DeviceId);

                string actuatorData = actuatorPatches[state];
                string patch = @"{properties:{desired:{" + actuatorData + "}}}";

                await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot patch device twin, check device connection string and device id. "+e.Message);
            }
        }

        /// <summary>
        /// Set the desired property for the telemetry interval.
        /// </summary>
        /// <param name="containerFarm">Takes in an existing container farm to access connection values</param>
        /// <param name="interval">Double interval to be set.</param>
        /// <returns>Task for asyncronous operation.</returns>
        public async Task PatchTelemetryInterval(ContainerFarm containerFarm, double interval)
        {
            try
            {
                var registryManager = RegistryManager.CreateFromConnectionString(containerFarm.IotHubConnectionString);
                var twin = await registryManager.GetTwinAsync(containerFarm.DeviceId);

                string patch =
                    @"{properties: {desired: {" + INTERVAL_KEY + ":" + interval + "}}}";

                await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot patch device twin, check device connection string and device id. "+e.Message);
            }
        }
    }
}