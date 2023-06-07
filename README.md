# Balancer
The aim of this project is to develop application, in C# and Godot, that will cooperate with Balancer - device holding ball in the set point on the resistive plate with use of servos. This application communicates through serial port with microcontroller(currently Arduino Uno) then displays data such as current ball position or PID controllers parameters. Also user can manipulate position of the ball by the set point.

# How it looks in action
![image](https://github.com/orszoooo/Balancer/assets/117857476/1654b0bc-f00e-4513-a300-94cfa4f39d9b)

# Current functionality
- Connection via serial port. GUI allows to choose the port, data transfer speed, start and end the connection
- GUI panel for displaying current process parameters and entering changes in parameters
- 3D model of balancer and ball with top view
- 3D view configuration (distance of the camera from the ball and balancer)
- Support for setting the ball position with a mouse click
- Support for application's window resizing

# List of commands - communication between balancer and computer
Coming out of the app:
- Start – start the ball balancing process
- Stop – stop the balancing process
- Sync – request to send current parameters
- XY {x} {y} – sending a new set point, e.g. XY 123 65. Coordinates are given in millimeters. Their range is the size of the working area of the resistive board, that is, for x from 0 to 337 and for y from 0 to 269.  
- PIDn {P} {I} {D} – sending parameters to PID controllers, where n is the controller number 

# To be done in the near future
- Full implementation of the serial commands (receiving and sending parameters)
- Balancer object(OOP programming) that stores all parameters of balancer for better organization of the code
- Arduino app to test all commands and simulate ball position
- Smooth ball movement animation
- Marker of the current set position
- Sliders for precise parameter setting
