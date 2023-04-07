import json

class Telemetry:
    def __init__(self, telemetry_data):
        self.__set_values(telemetry_data)

    def __set_values(self, telemetry_data):
        self.Temperature = telemetry_data['Temperature']
        self.Humidity = telemetry_data['Humidity']
        self.WaterLevel = telemetry_data['WaterLevel']
        self.Moisture = telemetry_data['Moisture']
        self.Longitude = telemetry_data['Longitude']
        self.Latitude = telemetry_data['Latitude']
        self.Pitch = telemetry_data['Pitch']
        self.Roll = telemetry_data['Roll']
        self.Vibration = telemetry_data['Vibration']
        self.BuzzerIsActive = telemetry_data['BuzzerIsActive']
        self.FanIsActive = telemetry_data['FanIsActive']
        self.LightIsActive = telemetry_data['LightIsActive']
        self.DoorIsLocked = telemetry_data['DoorIsLocked']
        self.Door = telemetry_data['Door']
        self.Motion = telemetry_data['Motion']
        self.Luminosity = telemetry_data['Luminosity']
        self.Noise = telemetry_data['Noise']

    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__, sort_keys=True)
    
class Vibration:
    def __init__(self, vibration_data):
        self.__set_values(vibration_data)

    def __set_values(self, vibration_data):
        self.X = vibration_data['X']
        self.Y = vibration_data['Y']
        self.Z = vibration_data['Z']

    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__, sort_keys=True)