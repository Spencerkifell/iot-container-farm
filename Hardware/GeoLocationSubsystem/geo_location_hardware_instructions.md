# Geo-Location Subsystem

### Hardware Setup
- You will need to plug in the [Grove - GPS (Air530)](https://wiki.seeedstudio.com/Grove-GPS-Air530/) into the UART (Serial Port) of the Grove Base Hat.

### Class Instructions
- How to initiate GeoLocationSubsystem Class
`geo_location_subsystem = GeoLocationSubsystem()`
- How to consistantly collect GPS data
    - When caling `geo_location_subsystem.read_gps_data` it will start a thread to read raw GPS data that consistantly updates the Longitude and Latitude properties.
- How to consistantly collect acceleration data
    - When calling `geo_location_subsystem.read_acceleration_data` it will start a thread to read raw acceleration data that consistantly updates the X,Y,Z values.

### Class Properties
- Latitude
    - Property that takes in and parses the raw GPS latitude value.
- Longitude
    - Property that takes in and parses the raw GPS longitude value.
- X
    - Property to store X value computed from collecting acceleration data
- Y
    - Property to store X value computed from collecting acceleration data
- Z
    - Property to store Y value computed from collecting acceleration data
- Pitch
    - Calculated get-only property that computes pitch using the X,Y,Z values.
    - Returns None if any of the X,Y or Z values are None.
- Roll
    - Calculated get-only property that computes roll using the X,Y,Z values.
    - Returns None if any of the X,Y or Z values are None.
- Vibration
    - Calculated get-only property that computes relative vibration using acceleration data (X,Y,Z)
- Buzzer
    - Property that can be used to set the state of the buzzer and read the state of the buzzer. (ON/OFF)
