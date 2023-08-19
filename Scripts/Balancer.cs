using System.IO.Ports;

public class Balancer
{
    //Resistive panel resolution
    private readonly int _xPanelResolution = 337;
    private readonly int _yPanelResolution = 269;


    //Are they realy needed?
    public int xPanelResolution { get { return _xPanelResolution; }}
    public int yPanelResolution { get { return _yPanelResolution; }} 

    //Current position
    public int XCurrentPosition {get; set;}
    public int YCurrentPosition {get; set;}

    //Desired position of ball 
    public int XSetpoint {get; set;}
    public int YSetpoint {get; set;}

    //Regulators
    public PID PIDRegulator1 { get; }
    public PID PIDRegulator2 { get; }

    //Communication
    public SerialPort SerialCom {get;}

    public Balancer()
    {
        PIDRegulator1 = new PID(0.0,0.0,0.0);
        PIDRegulator2 = new PID(0.0,0.0,0.0);
        SerialCom = new SerialPort();
    }
}


