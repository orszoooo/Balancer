using Godot;
using System.IO.Ports;
using System.Collections.Generic;
using System.Globalization;

public class Main : Spatial
{
    //337x269

    [Signal] //Handshaking
    public delegate void Connected();

    [Signal]
    public delegate void Disconnected();

    private static SerialPort _serialPort;
    private static bool _continue;
    private OptionButton _portList;
    private List<double> _paramList; 
    private Camera _camera;
    private const int _RESISTIVE_PANEL_LAYER = 2;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _paramList = new List<double>();
        _serialPort = new SerialPort();
        _portList = GetNode<OptionButton>("UI/DataCOM_VBoxContainer/COMControl/COMsettings_VBoxContainer/Port_HBoxContainer/Port_OptionButton");

        foreach (string s in SerialPort.GetPortNames())
        {
            _portList.AddItem(s);
        }  

        _camera = GetNode<Camera>("Balancer3D/CameraPivot/Camera");

        _continue = false;
    }

    public override void _Input(InputEvent @event)
    {
        //Registering left mouse click and getting the postion for the new set point  
        if(@event is InputEventMouseButton eventMouseButton 
            && eventMouseButton.Pressed &&
            (ButtonList)eventMouseButton.ButtonIndex == ButtonList.Left)
        {
            Vector3 origin = _camera.ProjectRayOrigin(eventMouseButton.Position);
            Vector3 direction = _camera.ProjectRayNormal(eventMouseButton.Position);
            Vector3 end = origin + direction * 1000;

            PhysicsDirectSpaceState state = GetWorld().DirectSpaceState;
            var intersection = state.IntersectRay(origin, end, 
                new Godot.Collections.Array {}, _RESISTIVE_PANEL_LAYER); //intersections dictionary

            if(intersection.Count > 0){
                //panel hit!
                Vector3 mousePosition = (Vector3)intersection["position"];
                int xCorrected = ((int)((mousePosition.x + 3.37f/2.0f) * 100.0f));
                int zCorrected = ((int)((mousePosition.z + 2.69f/2.0f) * 100.0f));

                GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/Xpos_HBoxContainer/Xpos_LineEdit").Text = xCorrected.ToString();
                GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/Ypos_HBoxContainer/Ypos_LineEdit").Text = zCorrected.ToString();
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(_continue)
        {
            if(_serialPort != null)
            {
                if(_serialPort.BytesToRead > 0)
                {
                    ParseInput(_serialPort.ReadTo("\n")); //Reads characters up to new line. If there is no new line character in the port's buffer, the content of the buffer is cleared.
                    UpdateUI();
                    UpdateBall();
                }
            }
        }
    }



    // Parses input from serial port into List of doubles that is used to update UI elements
    public void ParseInput(string inputParam)
    {
        string[] splitParam = inputParam.Split(' ');
        _paramList.Clear();

        foreach (string s in splitParam)
        {
            if(double.TryParse(s.Remove(0, s.IndexOf(':') + 1),
                NumberStyles.Number, CultureInfo.InvariantCulture, out double result)){
                _paramList.Add(result);
            }
            else{
                _paramList.Add(0.0);
            }
        }
    }
    
    public void UpdateUI()
    {
        if(_paramList.Count == 12)
        {
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Xpos_HBoxContainer/Xpos_LineEdit").Text = _paramList[0].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Ypos_HBoxContainer/Ypos_LineEdit").Text = _paramList[1].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Err1_HBoxContainer/Err1_LineEdit").Text = _paramList[2].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Y1_HBoxContainer/Y1_LineEdit").Text = _paramList[3].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/P1_HBoxContainer/P1_LineEdit").Text = _paramList[4].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/I1_HBoxContainer/I1_LineEdit").Text = _paramList[5].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/D1_HBoxContainer/D1_LineEdit").Text = _paramList[6].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Err2_HBoxContainer/Err2_LineEdit").Text = _paramList[7].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Y2_HBoxContainer/Y2_LineEdit").Text = _paramList[8].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/P2_HBoxContainer/P2_LineEdit").Text = _paramList[9].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/I2_HBoxContainer/I2_LineEdit").Text = _paramList[10].ToString();
            GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/D2_HBoxContainer/D2_LineEdit").Text = _paramList[11].ToString();
        }
    }

    public void UpdateBall()
    {
        GetNode<Spatial>("Balancer3D/Ball").GlobalTranslation = new Vector3((float)_paramList[0]/100.0f - (3.37f/2.0f), 0.0f, ((float)_paramList[1]/100.0f - (2.69f/2.0f)));
    }

    public void Connect()
    {   
        int selectedItem = _portList.GetSelectedId();
        if(selectedItem != -1)
        {
            _serialPort.PortName = _portList.GetItemText(selectedItem);
            _serialPort.BaudRate = GetNode<LineEdit>("UI/DataCOM_VBoxContainer/COMControl/COMsettings_VBoxContainer/BaudRate_HBoxContainer/BaudRate_LineEdit").Text.ToInt();
            _serialPort.Open(); 
            _continue = true;
            EmitSignal(nameof(Connected));
            GD.Print("SerialPort open on: " + _serialPort.PortName);
            _serialPort.WriteLine("SYNC");
            GD.Print("Sent SYNC request");
        }
        else
        {
            GetNode<AcceptDialog>("UI/PortNotSelectedAlert").Show();
            GetNode<Button>("UI/DataCOM_VBoxContainer/COMControl/COMsettings_VBoxContainer/Connect_Button").Text = "Connect";
        }   
    }

    public void Start()
    {
        if(_serialPort.IsOpen)
        {
            _serialPort.WriteLine("START");
            GD.Print("Sent START command");
        }
    }

    public void Stop()
    {
        if(_serialPort.IsOpen)
        {
            _serialPort.WriteLine("STOP");
            GD.Print("Sent STOP command");
        }
    }
    public void Disconnect()
    {
        if(_serialPort.IsOpen)
        {
            _continue = false;
            _serialPort.Close();
            EmitSignal(nameof(Disconnected));
            GD.Print("SP closed");
        }
    }

    public void RefreshPortList()
    {
        _portList = GetNode<OptionButton>("UI/DataCOM_VBoxContainer/COMControl/COMsettings_VBoxContainer/Port_HBoxContainer/Port_OptionButton");
        _portList.Clear();
        foreach (string s in SerialPort.GetPortNames())
        {
            _portList.AddItem(s);
        }  

    }
} 
