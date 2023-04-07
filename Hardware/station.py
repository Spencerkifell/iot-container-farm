from dotenv import dotenv_values
from azure.iot.device import IoTHubDeviceClient
import time
from PlantSubsystem import plant_subsystem
from SecuritySubsystem import security_subsystem
from GeoLocationSubsystem import geo_location_subsystem
from TelemetryHelper import Telemetry

class Station:
    """ This class is used to send telemetry data to the IoT Hub based off of all the subsystems and is also meant to handle Device Twins. """
    DEFAULT_INTERVAL = 10

    # Constructor initializes and stores telemetry data interval and all subsystems.
    def __init__(self, iot_device_connection_string):
        self.interval=self.DEFAULT_INTERVAL
        self.iot_device_connection_string = iot_device_connection_string
        self.client = self.create_client()
        self.plant=plant_subsystem.PlantSubsystem()
        self.geoLocation=geo_location_subsystem.GeoLocationSubSystem()
        self.security=security_subsystem.SecuritySubSystem()

    def read_geo_location_values(self):
        """ Method used to return the current values of the geo-location subsystem. """
        return self.geoLocation.longitude, self.geoLocation.latitude, self.geoLocation.pitch, self.geoLocation.roll, self.geoLocation.vibration, self.geoLocation.buzzer

    def read_plant_values(self):
        """ Method used to return the current values of the plant subsystem. """
        temp,humi=self.plant.read_temp_and_humi()
        return temp,humi,self.plant.read_water_level(),self.plant.read_moisture_level(),self.plant.read_fan_state(),self.plant.read_light_state()

    def read_security_values(self):
        """ Method used to return the current values of the security subsystem. """
        return self.security.read_door_lock_state(),self.security.read_door_state(),self.security.read_motion_state(),self.security.read_luminosity_level(),self.security.read_noise_level()

    def run(self):
        """ Method used to collect and send telemetry data from all subsystems to the IoT Hub. """
        self.client.connect()

        while True:
            # Get plant data
            temperature,humidity,water,moisture,fan,light=self.read_plant_values() 

            # Get security data
            doorLocked,door,motion,luminosity,noise = self.read_security_values()

            # Get geo location data
            longitude, latitude, pitch, roll, vibration, buzzer = self.read_geo_location_values()

            # Creates a serializable telemetry object taking in the values from all subsystems.
            telemetryData = Telemetry({
                "Temperature": temperature,
                "Humidity": humidity,
                "WaterLevel": water,
                "Moisture": moisture,
                "Longitude": longitude,
                "Latitude": latitude,
                "Pitch": pitch,
                "Roll": roll,
                "Vibration": vibration,
                "BuzzerIsActive": buzzer,
                "FanIsActive": fan,
                "LightIsActive": light,
                "DoorIsLocked": doorLocked,
                "Door": door,
                "Motion": motion,
                "Luminosity": luminosity,
                "Noise": noise
            })

            # Create the payload using the serialized telemetry object.
            payload = str(telemetryData.toJSON())

            print("Sending message: {}".format(payload))
            self.client.send_message(payload)
            print("Message sent")
            time.sleep(self.interval)

    # Method used to create an IoT Hub client
    def create_client(self):
        self.client = IoTHubDeviceClient.create_from_connection_string(self.iot_device_connection_string)

        # Patch repoted properties and update interval
        def twin_patch_handler(twin_patch):
            print("Twin patch received:")
            try:
                for key in twin_patch:
                    if(str(key)[0]!='$'):
                        if(key=='telemetryInterval'):
                            self.interval=twin_patch[key]
                            self.client.patch_twin_reported_properties({key:twin_patch[key]})
                            print({key:twin_patch[key]})
                        elif(key=='buzzerState'):
                            self.geoLocation.buzzer=twin_patch[key]
                            self.client.patch_twin_reported_properties({key:twin_patch[key]})
                            print({key:twin_patch[key]})
                        elif(key=='lightState'):
                            self.plant.set_light_state(twin_patch[key])
                            self.client.patch_twin_reported_properties({key:twin_patch[key]})
                            print({key:twin_patch[key]})
                        elif(key=='fanState'):
                            self.plant.set_fan_state(twin_patch[key])
                            self.client.patch_twin_reported_properties({key:twin_patch[key]})
                            print({key:twin_patch[key]})
                        elif(key=='doorLockState'):
                            self.security.set_door_lock_state(twin_patch[key])
                            self.client.patch_twin_reported_properties({key:twin_patch[key]})
                            print({key:twin_patch[key]})
                
            except Exception:
                print("An error occurred while patching the twin.")
        
        try:
            # Attach the twin patch handler
            self.client.on_twin_desired_properties_patch_received = twin_patch_handler
        except:
            # Clean up in the event of failure
            self.client.shutdown()
            raise

        return self.client


def main():
    iot_device_connection_string = get_env_values() 
    station = Station(iot_device_connection_string)
    try:
        station.run()
    except KeyboardInterrupt:
        print("IoTHubClient / Station interuptted by user")
    finally:
        print("Shutting down IoTHubClient")
        station.client.shutdown()
        
def get_env_values():
    """ Method used to verify the .env file contains the correct information. """
    config = dotenv_values(".env")
    IOTHUB_DEVICE_CONNECTION_STRING = config.get('IOTHUB_DEVICE_CONNECTION_STRING')
    if (IOTHUB_DEVICE_CONNECTION_STRING is None):
        raise KeyError("Missing IOTHUB_DEVICE_CONNECTION_STRING. Please see file .env.example.")
    return IOTHUB_DEVICE_CONNECTION_STRING        

if __name__ == '__main__':
    main()