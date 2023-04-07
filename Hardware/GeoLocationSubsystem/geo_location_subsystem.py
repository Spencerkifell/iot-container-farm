# __init__.py files weren't working for relative imports, so we had to import sys as a work around.
import sys
sys.path.append("../")
import time
import serial
import pynmea2
import argparse
import seeed_python_reterminal.core as rt
import seeed_python_reterminal.acceleration as rt_accel
import threading
from math import atan2, pi, sqrt
from TelemetryHelper import Vibration

class GeoLocationSubSystem:
    def __init__(self):
        """ Constructor that initializes the GPS module and the reTerminal built-in accelerometer """
        # Initializes the GPS module using the default serial port of the base hat (UART)
        self.serial = serial.Serial('/dev/ttyAMA0', 9600, timeout=1)
        self.device = rt.get_acceleration_device()
        self._latitude = None 
        self._longitude = None
        # Considering converting acceleration data (RAW/VIBRATION) to a dictionary/array.
        self.x = None
        self.y = None
        self.z = None
        # Added values for vibration levels
        self.previous_x = None
        self.previous_y = None
        self.previous_z = None
        # Added during milestone #3 since the subsystem module is instatiated in the geo_location_station.py without arguments. 
        self.read_gps_data()
        self.read_acceleration_data()

    @property
    def latitude(self):
        return self._latitude

    @latitude.setter
    def latitude(self, value):
        self._latitude = self.__check_negative_values(value)

    @property
    def longitude(self):
        return self._longitude

    @longitude.setter
    def longitude(self, value):
        self._longitude = self.__check_negative_values(value)

    @property
    def pitch(self):
        if self.x is not None and self.y is not None and self.z is not None:
            return 180 * atan2(self.x, sqrt(self.y**2 + self.z**2)) / pi
        else:
            return None

    @property
    def roll(self):
        if self.x is not None and self.y is not None and self.z is not None:
            return 180 * atan2(self.y, sqrt(self.x**2 + self.z**2)) / pi
        else:
            return None
        
    @property
    def buzzer(self):
        """ Returns the state of the buzzer """
        return rt.buzzer

    @buzzer.setter
    def buzzer(self, value):
        """ Sets the state of the buzzer (Values should be either ON or OFF) """
        # Ignores case sensitivity for input value
        if value.upper() == "ON":
            rt.buzzer = True
        elif value.upper() == "OFF":
            rt.buzzer = False
        else:
            print("Invalid buzzer state")

    @property
    def vibration(self):
        if (
            self.previous_x is not None and
            self.previous_y is not None and
            self.previous_z is not None and
            self.x is not None and
            self.y is not None and
            self.z is not None
        ):
            return Vibration({"X": self.x - self.previous_x, "Y": self.y - self.previous_y, "Z": self.z - self.previous_z})
        else:
            return None;

    def read_gps_data(self):
        """ Starts a thread that reads the GPS data and updates the longitude and latitude properties """
        threading.Thread(target=self.__read_raw_gps_data, daemon=True).start()

    def __read_raw_gps_data(self):
        """ Reads the raw GPS data and updates the longitude and latitude properties. Should be utilized in a seperate thread. """
        while True:
            try:
                line = self.serial.readline().decode('utf-8')
                while len(line) > 0:
                    if (self.__parse_gps_data(line) is not None):
                        self.longitude, self.latitude = self.__parse_gps_data(line)
                    break
            except UnicodeDecodeError:
                self.serial.reset_input_buffer()
                self.serial.flush()
            except Exception:
                # This exception is thrown when the resource is temporarily unavailable (BlockingIOError/SerialException)
                pass

    def __check_negative_values(self, value):
        """ Checks if the value is negative, parses to a negative value if it is and returns the propper value"""
        try:
            if len(str(value).split(".")[0]) > 1 and str(value)[0] == "0":
                value = -abs(float(value))
            return float(value) / 100
        except Exception:
            return None

    def __parse_gps_data(self, line):
        """ Parses the GPS data and returns the longitude and latitude properties from the GPS payload """
        try:
            gps_data = pynmea2.parse(line.rstrip())
            return gps_data.lon, gps_data.lat
        except Exception:
            pass

    def read_acceleration_data(self):
        """ Starts a thread that reads the acceleration data and updates the x, y and z properties """
        threading.Thread(target=self.__read_raw_acceleration_data, daemon=True).start()

    def __read_raw_acceleration_data(self):
        """ Reads the raw acceleration data and updates the x, y and z properties. Should be utilized in a seperate thread. """
        while True:
            try:
                for event in self.device.read_loop():
                    accelEvent = rt_accel.AccelerationEvent(event)
                    if (accelEvent.name != None):
                        if accelEvent.name == rt_accel.AccelerationName.X:
                            self.previous_x = self.x
                            self.x = accelEvent.value
                        elif accelEvent.name == rt_accel.AccelerationName.Y:
                            self.previous_y = self.y
                            self.y = accelEvent.value
                        elif accelEvent.name == rt_accel.AccelerationName.Z:
                            self.previous_z = self.z
                            self.z = accelEvent.value
            except Exception:
                # This exception is thrown when the resource is temporarily unavailable (BlockingIOError/SerialException)
                pass

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
    
    # The following aren't in the main loop because they need to be initialized before the main loop.
    if (args.gps):
        geo_location_subsystem.read_gps_data()

    if (args.angles or args.vibration):
        geo_location_subsystem.read_acceleration_data()

    if (args.set_buzzer):
        geo_location_subsystem.buzzer = args.set_buzzer

    try:
        while True:
            # We only display this data every second to reduce the amount of data being printed, 
            # however the values are still being collected since we are using a seperate thread
            print("\n** Geo Location Data **")
            
            if (args.gps):
                if (geo_location_subsystem.longitude is None and geo_location_subsystem.latitude is None):
                    print("\nWaiting for GPS data...")
                else:
                    print("\nGPS Data:")
                    print(f"\tLongitude: {geo_location_subsystem.longitude}, Latitude: {geo_location_subsystem.latitude}") 
            
            if (args.angles):
                if (geo_location_subsystem.x is None and geo_location_subsystem.y is None and geo_location_subsystem.z is None):
                    print("\nWaiting for accelerometer data...")
                else:
                    print("\nAccerlerometer Data:")
                    print(f"\tX: {geo_location_subsystem.x}, Y: {geo_location_subsystem.y}, Z: {geo_location_subsystem.z}")
                    print(f"\tPitch: {geo_location_subsystem.pitch}, Roll: {geo_location_subsystem.roll}")

            if (args.buzzer):
                print("\nBuzzer Data:")
                print(f"\tBuzzer state: {geo_location_subsystem.buzzer}")

            if (args.vibration):
                print("\nVibration Data:")
                if geo_location_subsystem.vibration is None:
                    print("\nWaiting for accelerometer/vibration data...")
                else:
                    print(f"\tVibration levels: {geo_location_subsystem.vibration.toJSON()}")
            
            time.sleep(1)
    except KeyboardInterrupt:
        print("\nKeyboard Interrupt - Exiting Program...")

if __name__ == '__main__':
    main()