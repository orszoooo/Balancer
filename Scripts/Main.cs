using Godot;
using System.IO.Ports;
using System.Globalization;

public partial class Main : Spatial
{
    [Signal] //Handshaking
    public delegate void Connected();

    [Signal]
    public delegate void Disconnected();

    private Balancer _balancer;
    private static bool _continue;
    private OptionButton _portList;
    private Camera _camera;
    private const int _RESISTIVE_PANEL_LAYER = 2;
    private bool _showReceivedMessages = false;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");  //To use . instead of , as decimal separator
        _balancer = new Balancer();
        
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
            if(_balancer.SerialCom.IsOpen)
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
} 
