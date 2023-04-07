#Brookelyn Palfy
from dotenv import dotenv_values
from azure.iot.device import IoTHubDeviceClient, Message
import time
import plant_subsystem

'''
This class is used to send the plant subsystem telemetry data to the 
IoT Hub. As well, it recives and desired properties and updates the
reported properties of the device twin.
'''
class PlantStation:
    pass
    MSG_TXT = '{{"temperature": {temperature},"humidity": {humidity},"water level": {water},"moisture level": {moisture}}}'
    DEFAULT_INTERVAL = 10

    # Constructor initializes and stores telemetry data interval 
    # and plant subsystem object.
    def __init__(self):
        self.interval=self.DEFAULT_INTERVAL
        self.plant=plant_subsystem.PlantSubsystem()
        self.check_env()        

    # Method used to verify the .env file contains the correct information.
    def check_env(self):
        try:
            config = dotenv_values(".env")
            self.IOTHUB_DEVICE_CONNECTION_STRING = config.get('IOTHUB_DEVICE_CONNECTION_STRING')
        except:
            raise KeyError('Missing iot hub device connection string. Please see file .env.example.')
        if(self.IOTHUB_DEVICE_CONNECTION_STRING is None):
            raise KeyError('Missing iot hub device connection string. Please see file .env.example.')

    # Method used to collect and send telemetry data from the plant subsystem to the IoT Hub.
    def run_telemetry(self):

        self.client.connect()

        while True:
            temperature,humidity=self.plant.read_temp_and_humi()
            water=self.plant.read_water_level()
            moisture=self.plant.read_moisture_level()    

            message = Message(self.MSG_TXT.format(temperature="{:.2f}".format(temperature), humidity="{:.2f}".format(humidity),water=water,moisture=moisture))
                 
            print("Sending message: {}".format(message))
            self.client.send_message(message)
            print("Message sent")
            time.sleep(self.interval)

    # Method used to create an IoT Hub client
    def create_client(self):
        
        self.client = IoTHubDeviceClient.create_from_connection_string(self.IOTHUB_DEVICE_CONNECTION_STRING)

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
                
            except Exception:
                print("error")
        
        try:
            # Attach the twin patch handler
            self.client.on_twin_desired_properties_patch_received = twin_patch_handler
        except:
            # Clean up in the event of failure
            self.client.shutdown()
            raise

        return self.client


def main():
    station = PlantStation()
    station.create_client()
    try:
        station.run_telemetry()

    except KeyboardInterrupt:
        print("IoTHubClient stopped by user")
    finally:
        print("Shutting down IoTHubClient")
        station.client.shutdown()
    

if __name__ == '__main__':
    main()