from dotenv import dotenv_values
from azure.iot.device import IoTHubDeviceClient, Message
import time
import security_subsystem

class SecurityStation:
    # Constants
    MSG_TXT = '{{"noise level": {noise}, "luminosity level": {luminosity}, "motion detection": {motion}, "door state": {door}}}'
    DEFAULT_INTERVAL = 10

    # Constructor initializes and stores telemetry data interval 
    # and security subsystem object.
    def __init__(self):
        self.interval=SecurityStation.DEFAULT_INTERVAL
        self.security=security_subsystem.SecuritySubSystem()
        self.check_env()    
    
    #Checks if the value in the .env file are valid
    def check_env(self):
        try:
            config = dotenv_values(".env")
            self.IOTHUB_DEVICE_CONNECTION_STRING = config.get('IOTHUB_DEVICE_CONNECTION_STRING')
        except:
            raise KeyError('Missing iot hub device connection string. Please see file .env.example.')
        if(self.IOTHUB_DEVICE_CONNECTION_STRING is None):
            raise KeyError('Missing iot hub device connection string. Please see file .env.example.')
    
    #Collects and sends data from the security subsytem to the IoT Hub 
    def run_telemetry(self,client):
        
        client.connect()
        
        while True:
            noise=self.security.read_noise_level()
            luminosity=self.security.read_luminosity_level()
            motion=self.security.read_motion_state()
            door=self.security.read_door_state()
            message = Message(SecurityStation.MSG_TXT.format(noise=noise,luminosity=luminosity,motion=motion,door=door))

            message.content_encoding = "UTF-8"
            message.content_type = "application/json"
            print("Sending message: {}".format(message))
            client.send_message(message)
            print("Message sent")
            time.sleep(self.interval)
    
    #Create an IoT Hub client
    def create_client(self):
        
        self.client = IoTHubDeviceClient.create_from_connection_string(self.IOTHUB_DEVICE_CONNECTION_STRING)

        # Patch reported properties and update interval
        def twin_patch_handler(twin_patch):
            print("Twin patch received:")
            try:
                for key in twin_patch:
                    if(str(key)[0]!='$'):                        
                        if(key=='telemetryInterval'):
                            self.client.patch_twin_reported_properties({key:twin_patch[key]})
                            print({key:twin_patch[key]})
                            self.interval=twin_patch[key]
                
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
    station = SecurityStation()
    client = station.create_client()
    try:
        station.run_telemetry(client)
    except KeyboardInterrupt:
        print("IoTHubClient stopped by user")
    finally:
        print("Shutting down IoTHubClient")
        station.client.shutdown()


if __name__ == '__main__':
    main()