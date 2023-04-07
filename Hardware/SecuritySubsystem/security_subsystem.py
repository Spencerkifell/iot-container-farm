import argparse
from grove.adc import ADC
from gpiozero import Button,AngularServo
import seeed_python_reterminal.core as rt
from grove.grove_mini_pir_motion_sensor import GroveMiniPIRMotionSensor
from gpiozero.pins.pigpio import PiGPIOFactory

#Constants 
DOOR_PIN = 22
MOTION_PIN = 24
LOCK_PIN = 12
NOISE_PIN = 2
OPEN_ANGLE = 0
CLOSE_ANGLE = 180

class SecuritySubSystem:
    #Constructor that initializes and stores:
    # - ADC device reader (Luminosity, Buzzer, Noise Sensor)
    # - Motion Sensor
    # - Door Sensor
    # - Door Lock (Servo)
    def __init__(self):
        #Luminosity and Buzzer
        self.adc = ADC()
        #Door Sensor 
        self.door = Button(DOOR_PIN)
        #Motion Sensor
        self.motion_detected = None
        self.motionSensor = GroveMiniPIRMotionSensor(MOTION_PIN)
        self.motionSensor.on_event = self._handle_event
        #Door Lock
        self.lock = AngularServo(LOCK_PIN,0,OPEN_ANGLE,CLOSE_ANGLE,pin_factory= PiGPIOFactory())
    
    #Return value of the noise sensor 
    def read_noise_level(self):
        return self.adc.read(NOISE_PIN)
    
    #Return value of the luminosity level
    def read_luminosity_level(self):
        return round(rt.illuminance)
    
    #Checks the state of the buzzer on the reterminal (on/off)
    def read_buzzer_state(self):
        return rt.buzzer
    
    #Sets the buzzer state on the reterminal(on/off)
    def set_buzzer_state(self,state):
        if(state=='on'):
            rt.buzzer = True
        elif(state=='off'):
            rt.buzzer = False
    
    #Checks the state of the "door" by seeing if the magnet are linked, which imitates a closed door
    def read_door_state(self):
        return self.door.is_pressed

    #Checks if a motion has been detected by the sensor
    def read_motion_state(self):
        return self.motion_detected
        
    def _handle_event(self, pin, value):
        self.motion_detected = True if value == 1 else False

    #Sets the state of the door lock (servo motor), (Open/Closed)
    def set_door_lock_state(self,state):
        if(state == "close"):
            self.lock.max()
        elif(state == "open"):
            self.lock.min()
            
     #Checks the state of the door lock (servo motor), (Open/Closed)
    def read_door_lock_state(self):
        if(self.lock.angle == self.lock.max_angle):
            return True
        elif(self.lock.angle == self.lock.min_angle):
            return False

def main():

    parser = argparse.ArgumentParser()

    # Define arguments
    parser.add_argument("--noise",const=True, default=False,nargs='?')
    parser.add_argument("--lumi",const=True, default=False,nargs='?')
    parser.add_argument("--buzzer",const=True, default=False,nargs='?')
    parser.add_argument("--set_buzzer", type=str)
    parser.add_argument("--door",const=True, default=False,nargs='?')
    parser.add_argument("--motion",const=True, default=False,nargs='?')
    parser.add_argument("--set_lock", type=str)
    parser.add_argument("--lock",const=True, default=False,nargs='?')
    # Parse arguments
    args = parser.parse_args()
    security= SecuritySubSystem()

    # Call the correct methods based on the arguments provided
    if(args._get_args().count!=0):
        if(args.noise):
            print("Noise Level: "+str(security.read_noise_level()))
    
        if(args.lumi):
            luminosity = security.read_luminosity_level()
            print("Luminosity Level: "+ str(luminosity))

        if(args.buzzer):
            print("Buzzer State: " + ("ON" if security.read_buzzer_state() else "OFF"))

        if(args.set_buzzer):
            security.set_buzzer_state(args.set_buzzer)
            print("Buzzer State: " + ("ON" if security.read_buzzer_state() else "OFF"))

        if(args.door):
            print("Door State: " + ("Closed" if security.read_door_state() else "Open"))
        
        if(args.motion):
            security.read_motion_state()
            print(IS_DETECTED)

        if(args.set_lock):
            security.set_door_lock_state(args.set_lock)
            print("Door Lock State: " + ("Locked" if security.read_door_lock_state() else "Unlock"))
       
        if(args.lock):
            print("Door Lock State: " + ("Locked" if security.read_door_lock_state() else "Unlock"))
            
            
            


    

if __name__ == '__main__':
    main()     