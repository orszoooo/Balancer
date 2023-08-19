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

    private Balancer _balancer;
    private static bool _continue;
    private OptionButton _portList;
    //private List<double> _paramList; 
    private Camera _camera;
    private const int _RESISTIVE_PANEL_LAYER = 2;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _balancer = new Balancer();
        //_paramList = new List<double>();
        
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
                int xCorrected = ((int)((mousePosition.x + (_balancer.xPanelResolution/100.0f)/2.0f) * 100.0f));
                int zCorrected = ((int)((mousePosition.z + (_balancer.yPanelResolution/100.0f)/2.0f) * 100.0f));

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
            if(_balancer.SerialCom != null)
            {
                if(_balancer.SerialCom.BytesToRead > 0)
                {
                    ParseInput(_balancer.SerialCom.ReadTo("\n")); //Reads characters up to new line. If there is no new line character in the port's buffer, the content of the buffer is cleared.
                    UpdateUI();
                    UpdateBall();
                }
            }
        }
    }



    // Parses input from serial port into List of doubles that is used to update UI elements
    public void ParseInput(string inputParam)
    {
        GD.Print("RECEIVED: " + inputParam);
        string[] splitParam = inputParam.Split(' ');
      // _paramList.Clear();
        bool syncActive = false;

        for(int i = 0; i < splitParam.Length; i++)
        {
            if(splitParam[i] == "SYNC" || syncActive)
            {
                GD.Print("Received SYNC");
                syncActive = true;
                syncActive = SYNCProcedure(splitParam, i, ref syncActive);
            } 
            else
                UpdateBalancerState(splitParam, i);
        }
    }

    private bool SYNCProcedure(string[] splitParam, int i, ref bool syncActive)
    {
        //Parameter SYNC procedure
        if (splitParam[i] == "XY")
        {
            _balancer.XSetpoint = (int)ReceivedStringToNumber(splitParam[i + 1]);
            _balancer.YSetpoint = (int)ReceivedStringToNumber(splitParam[i + 2]);
        }

        if (splitParam[i] == "PID1")
        {
            _balancer.PIDRegulator1.P = ReceivedStringToNumber(splitParam[i + 1]);
            _balancer.PIDRegulator1.I = ReceivedStringToNumber(splitParam[i + 2]);
            _balancer.PIDRegulator1.D = ReceivedStringToNumber(splitParam[i + 3]);
        }

        if (splitParam[i] == "PID2")
        {
            _balancer.PIDRegulator2.P = ReceivedStringToNumber(splitParam[i + 1]);
            _balancer.PIDRegulator2.I = ReceivedStringToNumber(splitParam[i + 2]);
            _balancer.PIDRegulator2.D = ReceivedStringToNumber(splitParam[i + 3]);
            syncActive = false;
        }

        return syncActive;
    }


    private void UpdateBalancerState(string[] splitParam, int i)
    {
        GD.Print("Updating Balancer State");
        if (splitParam[i] == "XY")
        {
            _balancer.XCurrentPosition = (int)ReceivedStringToNumber(splitParam[i + 1]);
            _balancer.YCurrentPosition = (int)ReceivedStringToNumber(splitParam[i + 2]);
        }

        if (splitParam[i] == "PID1")
        {
            _balancer.PIDRegulator1.Error = ReceivedStringToNumber(splitParam[i + 1]);
            _balancer.PIDRegulator1.RegulatorOutput = ReceivedStringToNumber(splitParam[i + 2]);
        }

        if (splitParam[i] == "PID2")
        {
            _balancer.PIDRegulator2.Error = ReceivedStringToNumber(splitParam[i + 1]);
            _balancer.PIDRegulator2.RegulatorOutput = ReceivedStringToNumber(splitParam[i + 2]);
        }
    }

    private double ReceivedStringToNumber(string numberString)
    {
        if(double.TryParse(numberString.Remove(0, numberString.IndexOf(':') + 1),NumberStyles.Number, CultureInfo.InvariantCulture, out double result))
        {
            return result;
        }
        else
        {
            return 0.0;
        }
    }

    private void UpdateUI()
    {
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Xpos_HBoxContainer/Xpos_LineEdit").Text 
            = _balancer.XCurrentPosition.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Ypos_HBoxContainer/Ypos_LineEdit").Text 
            = _balancer.YCurrentPosition.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Err1_HBoxContainer/Err1_LineEdit").Text 
            = _balancer.PIDRegulator1.Error.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Y1_HBoxContainer/Y1_LineEdit").Text 
            = _balancer.PIDRegulator1.RegulatorOutput.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/P1_HBoxContainer/P1_LineEdit").Text 
            = _balancer.PIDRegulator1.P.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/I1_HBoxContainer/I1_LineEdit").Text 
            = _balancer.PIDRegulator1.I.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/D1_HBoxContainer/D1_LineEdit").Text 
            = _balancer.PIDRegulator1.D.ToString();
            
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Err2_HBoxContainer/Err2_LineEdit").Text 
            = _balancer.PIDRegulator2.Error.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Y2_HBoxContainer/Y2_LineEdit").Text 
            = _balancer.PIDRegulator2.RegulatorOutput.ToString();

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/P2_HBoxContainer/P2_LineEdit").Text 
            = _balancer.PIDRegulator2.P.ToString();
        
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/I2_HBoxContainer/I2_LineEdit").Text 
            = _balancer.PIDRegulator2.I.ToString();
        
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/D2_HBoxContainer/D2_LineEdit").Text 
            = _balancer.PIDRegulator2.D.ToString();
    }

    private void UpdateBall()
    {
        GetNode<Spatial>("Balancer3D/Ball").GlobalTranslation = new Vector3(
            (float)_balancer.XCurrentPosition/100.0f - ((_balancer.xPanelResolution/100.0f)/2.0f),
             0.0f, 
            ((float)_balancer.YCurrentPosition/100.0f - ((_balancer.xPanelResolution/100.0f)/2.0f))
        );
    }

    private void Connect()
    {   
        int selectedItem = _portList.GetSelectedId();
        if(selectedItem != -1)
        {
            _balancer.SerialCom.PortName = _portList.GetItemText(selectedItem);
            _balancer.SerialCom.BaudRate = GetNode<LineEdit>("UI/DataCOM_VBoxContainer/COMControl/COMsettings_VBoxContainer/BaudRate_HBoxContainer/BaudRate_LineEdit").Text.ToInt();
            _balancer.SerialCom.Open(); 
            _continue = true;
            EmitSignal(nameof(Connected));
            GD.Print("SerialPort open on: " + _balancer.SerialCom.PortName);
            _balancer.SerialCom.WriteLine("SYNC");
            GD.Print("Sent SYNC request");
        }
        else
        {
            GetNode<AcceptDialog>("UI/PortNotSelectedAlert").Show();
            GetNode<Button>("UI/DataCOM_VBoxContainer/COMControl/COMsettings_VBoxContainer/Connect_Button").Text = "Connect";
        }   
    }

    private void Start()
    {
        if(_balancer.SerialCom.IsOpen)
        {
            _balancer.SerialCom.WriteLine("START");
            GD.Print("Sent START command");
        }
    }

    private void Stop()
    {
        if(_balancer.SerialCom.IsOpen)
        {
            _balancer.SerialCom.WriteLine("STOP");
            GD.Print("Sent STOP command");
        }
    }
    private void Disconnect()
    {
        if(_balancer.SerialCom.IsOpen)
        {
            _continue = false;
            _balancer.SerialCom.Close();
            EmitSignal(nameof(Disconnected));
            GD.Print("SP closed");
        }
    }

    private void RefreshPortList()
    {
        _portList = GetNode<OptionButton>("UI/DataCOM_VBoxContainer/COMControl/COMsettings_VBoxContainer/Port_HBoxContainer/Port_OptionButton");
        _portList.Clear();
        foreach (string s in SerialPort.GetPortNames())
        {
            _portList.AddItem(s);
        }  

    }
} 
