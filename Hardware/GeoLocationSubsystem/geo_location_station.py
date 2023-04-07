import time
import geo_location_subsystem as gls
from azure.iot.device import IoTHubDeviceClient, Message
from dotenv import dotenv_values

class GeoLocationStation:
    """ This class is used to send the geo-location subsystem telemetry data to the IoT Hub. It also receives the desired properties and updates the reported properties of the device twin. """
    DEFAULT_INTERVAL = 10.0

    def __init__(self, connection_string: str, interval: float = DEFAULT_INTERVAL):
        """ Constructor that instantiates the GeoLocationStation class. It takes in the connection string (str) and the interval (float) as parameters. """
        self.connection_string = connection_string
        self.interval = interval
        self.client = self.create_client()
        self.geo_location_subsystem = gls.GeoLocationSubSystem()


    def __parse_float(self, value):
        """ Private method meant to be only used internally to parse a value into a float. If the value is not a float, it will return None. """
        try:
            return float(value)
        except ValueError:
            return None

    def create_client(self):
        """ Method that creates the client that will be used to send the telemetry data to the IoT Hub. """
        client = IoTHubDeviceClient.create_from_connection_string(self.connection_string)

        # Define behavior for receiviing twin desired property patches
        def twin_patch_handler(twin_patch):
            print("Twin patch received:")
            # Update the repoted properties with the values of the desire property (params as JSON dict)
            for property in twin_patch:
                # Check for the interval property and update the interval
                if (property == 'telemetryInterval'):
                    # Validate the interval to assure the value is a positive numeric value
                    parsed_interval = self.__parse_float(twin_patch[property])
                    if parsed_interval is not None and parsed_interval > 0:
                        self.interval = parsed_interval
                        print(f"Telemetry interval updated to -> {self.interval}")

        try:
            # Set the handlers on the client
            client.on_twin_desired_properties_patch_received = twin_patch_handler
        except:
            # Clean up in the event of failure
            client.shutdown()
        
        return client

    def read_values(self):
        """ Returns the values of the geo-location subsystem. """
        return self.geo_location_subsystem.longitude, self.geo_location_subsystem.latitude, self.geo_location_subsystem.pitch, self.geo_location_subsystem.roll, self.geo_location_subsystem.vibration, self.geo_location_subsystem.buzzer

    def run(self):
        """ Method used to collect and send telemetry data from the geo-location subsystem to the IoT Hub. """
        JSON_PAYLOAD = '{{"longitude": {longitude}, "latitude": {latitude}, "pitch": {pitch}, "roll": {roll}, vibration: {vibration}, "buzzer": {buzzer}}}'
        self.client.connect()

        while True:
            longitude, latitude, pitch, roll, vibration, buzzer = self.read_values()
            formatted_payload = JSON_PAYLOAD.format(longitude=longitude, latitude=latitude, pitch=pitch, roll=roll, vibration=vibration, buzzer=buzzer)
            
            # Create / Initialize the message with the formatted JSON payload.
            message = Message(formatted_payload)

            # Send the message to the IoT Hub.
            print(f"Sending message: {message}")
            self.client.send_message(message)
            time.sleep(self.interval)

def main():
    connection_string = get_env_values()
    station = GeoLocationStation(connection_string)
    try:
        station.run()
    except KeyboardInterrupt:
        print("Geo-Location Station interuptted by user.")
    finally:
        station.client.shutdown()

def get_env_values():
    config = dotenv_values(".env")
    IOTHUB_DEVICE_CONNECTION_STRING = config.get('IOTHUB_DEVICE_CONNECTION_STRING')
    if (IOTHUB_DEVICE_CONNECTION_STRING is None):
        raise KeyError("Missing IOTHUB_DEVICE_CONNECTION_STRING. Please see file .env.example.")
    return IOTHUB_DEVICE_CONNECTION_STRING

if __name__ == "__main__":
    main()

