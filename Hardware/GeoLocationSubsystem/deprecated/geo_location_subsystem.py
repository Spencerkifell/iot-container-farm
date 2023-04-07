from math import atan2, pi, sqrt
import time
import serial
import pynmea2
import argparse
import seeed_python_reterminal.core as rt
import seeed_python_reterminal.acceleration as rt_accel

class GeoLocationSubSystem:
    def __init__(self):
        """ Constructor that initializes the GPS module and the reTerminal built-in accelerometer """
        # Initializes the GPS module using the default serial port of the base hat (UART)
        self.serial = serial.Serial('/dev/ttyAMA0', 9600, timeout=1)
        self.device = rt.get_acceleration_device()

    def get_gps_location(self):
        """ Returns longitude and latiude values based off of the GPS module """
        longitude, latitude = self.__read_gps_data()
        longitude = self.__check_negative_values(longitude)
        latitude = self.__check_negative_values(latitude)
        return longitude, latitude

    def __check_negative_values(self, value):
        """ Checks if the value is negative, parses to a negative value if it is and returns the propper value"""
        if len(value.split(".")) > 1 and str(value)[0] == "0":
            value = -abs(float(value))
        return float(value) / 100


    def __read_gps_data(self):
        """ Reads the GPS data from the serial port and returns the longitude and latitude """
        longitude, latitude = [None, None]
        while (longitude is None or latitude is None):
            try:
                line = self.serial.readline().decode('utf-8')
                while len(line) > 0:
                    if (self.__parse_gps_data(line) is not None):
                        longitude, latitude = self.__parse_gps_data(line)
                    break
            except UnicodeDecodeError:
                self.serial.reset_input_buffer()
                self.serial.flush()
        return longitude, latitude

    def __parse_gps_data(self, line):
        """ Parses the GPS data and returns the longitude and latitude properties from the GPS payload """
        try:
            gps_data = pynmea2.parse(line.rstrip())
            return gps_data.lon, gps_data.lat
        except Exception:
            pass

    def get_angles(self):
        """ Returns the pitch and roll angles of the device's accelerometer """
        x,y,z = self.__get_raw_acceleration_data()
        pitch = 180 * atan2(x, sqrt(y**2 + z**2)) / pi
        roll = 180 * atan2(y, sqrt(x**2 + z**2)) / pi
        return pitch, roll
        

    def __get_raw_acceleration_data(self):
        """ Returns the raw acceleration data from the accelerometer """
        x,y,z = [None, None, None]
        while (x is None or y is None or z is None):
            # Only reads one at a time, otherwise we are reading the same data over and over again
            event = self.device.read_one()
            if (event is not None):
                accelEvent = rt_accel.AccelerationEvent(event)
                if accelEvent.name == rt_accel.AccelerationName.X:
                    x = accelEvent.value
                elif accelEvent.name == rt_accel.AccelerationName.Y:
                    y = accelEvent.value
                elif accelEvent.name == rt_accel.AccelerationName.Z:
                    z = accelEvent.value
        return x,y,z

    # TODO: Need to research how to calculate vibration levels.
    def get_vibration_levels(self):
        """ Returns the vibration levels of the device """
        pass

    def get_buzzer_state(self):
        """ Returns the state of the buzzer """
        return "ON" if rt.buzzer else "OFF"

    def set_buzzer_state(self, value):
        """ Sets the state of the buzzer """
        if value == "ON":
            rt.buzzer = True
        elif value == "OFF":
            rt.buzzer = False

def main():
    parser = argparse.ArgumentParser()

    # Defined arguments
    parser.add_argument("--gps",const=True, default=False, nargs='?', help="Defines if we should collect GPS data")
    parser.add_argument("--angles", const=True, default=False, nargs='?', help="Defines if we should collect accelerometer data")
    parser.add_argument("--buzzer", const=True, default=False, nargs='?', help="Defines if we should collect buzzer data")
    parser.add_argument("--vibration", const=True, default=False, nargs='?', help="Defines if we should collect vibration data")
    parser.add_argument("--set_buzzer", type=str, default=None, help="The user can set the buzzer state")

    # Parse the arguments
    args = parser.parse_args()

    # Initialize the geo location subsystem
    geo_location_subsystem = GeoLocationSubSystem()
    
    if (args.gps):
        longitude, latitude = geo_location_subsystem.get_gps_location()
        print(f"Longitude: {longitude}, Latitude: {latitude}")
    
    if (args.angles):
        pitch, roll = geo_location_subsystem.get_angles()
        print(f"Pitch: {pitch}, Roll: {roll}")

    if (args.buzzer):
        print(f"Buzzer state: {geo_location_subsystem.get_buzzer_state()}")

    if (args.vibration):
        print(f"Vibration levels: {geo_location_subsystem.get_vibration_levels()}")

    if (args.set_buzzer):
        geo_location_subsystem.set_buzzer_state(args.set_buzzer)
        print(f"Buzzer state: {geo_location_subsystem.get_buzzer_state()}")

if __name__ == '__main__':
    main()