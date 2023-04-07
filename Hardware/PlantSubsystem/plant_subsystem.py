from grove.grove_temperature_humidity_aht20 import GroveTemperatureHumidityAHT20
from gpiozero import LED
from . import chainable_rgb_direct
from grove.adc import ADC
import argparse
import time

'''
This class is used to control the hardware related to the plat subsystem.
'''
class PlantSubsystem:
    
    # Constructor initializes and stores the tempertaure sensor, 
    # the fan, the adc device reader and the leds
    def __init__(self):
    
        pin=0x38
        bus=4
        self.sensor=GroveTemperatureHumidityAHT20(pin,bus)

        self.fan=LED(5)
        self.adc = ADC()

        self.num_led=2
        self.leds=chainable_rgb_direct.rgb_led(self.num_led)
  
    # Reads and returns the temperature and humidity respectively.
    def read_temp_and_humi(self):
        return GroveTemperatureHumidityAHT20.read(self.sensor) 

    # Reads and returns the fan state. 
    # Returns true is fan is on, false otherwise
    def read_fan_state(self):
        return self.fan.is_active

    # Sets the state of the fan. 
    # Arguments: string state ('on' or 'off')
    def set_fan_state(self,state):
        if(state=='on'):
           self.fan.on()
        elif(state=='off'):
            self.fan.off() 
        time.sleep(1)

    # Reads and returns the light state.
    # Returns true if the light is on, false otherwise.
    def read_light_state(self):
        for x in range(self.num_led):
            if(self.leds.r_all[x]==0 and self.leds.g_all[x]==0 and self.leds.b_all[x]==0):
                return False
        return True

    # Sets the state of the light
    # Arguments: string state ('on' or 'off')
    def set_light_state(self,state):
        if(state=='on'):
            for x in range(self.num_led):
                self.leds.setOneLED(127,127,127,x)
        elif(state=='off'):
            for x in range(self.num_led):
                self.leds.setOneLED(0,0,0,x)

    # Reads and returns the moisture level.
    def read_moisture_level(self):
        return self.adc.read(0)

    # Reads and returns the water level.
    def read_water_level(self):
        return self.adc.read(5)


def main():

    parser = argparse.ArgumentParser()

    # Define arguments
    parser.add_argument("--temp_humi",const=True, default=False,nargs='?')
    parser.add_argument("--water",const=True, default=False,nargs='?')
    parser.add_argument("--moisture",const=True, default=False,nargs='?')
    parser.add_argument("--fan",const=True, default=False,nargs='?')
    parser.add_argument("--set_fan", type=str)
    parser.add_argument("--light",const=True, default=False,nargs='?')
    parser.add_argument("--set_light", type=str)

    # Parse arguments
    args = parser.parse_args()
    plant= PlantSubsystem()

    # Call the correct methods based on the arguments provided
    if(args._get_args().count!=0):
        if(args.temp_humi):
            temp,humi=plant.read_temp_and_humi()
            print("Temperature: "+"{:.2f}".format(temp)+"\nHumidity: "+"{:.2f}".format(humi))
    
        if(args.water):
            print("Water level: "+str(plant.read_water_level()))

        if(args.moisture):
            print("Moisture: "+str(plant.read_moisture_level()))

        if(args.fan):
            print("Fan State: "+("ON" if plant.read_fan_state() else "OFF"))

        if(args.set_fan):
            plant.set_fan_state(args.set_fan)
            print("Fan State: "+("ON" if plant.read_fan_state() else "OFF"))

        if(args.light):
            print("Light State: "+("ON" if plant.read_light_state() else "OFF"))

        if(args.set_light):
            plant.set_light_state(args.set_light)
            print("Light State: "+("ON" if plant.read_light_state() else "OFF"))

    

if __name__ == '__main__':
    main()