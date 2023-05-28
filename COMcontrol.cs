using Godot;
using System;

public class COMcontrol : PanelContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal]
    public delegate void ConnectToCOM();

    [Signal]
    public delegate void DisconnectFromCOM(); 
    
    [Signal]
    public delegate void RefreshPortList();
    [Signal]
    public delegate void Start();
    [Signal]
    public delegate void Stop();

    bool _connected; //0 - disconected 1 - connected
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void OnConnectButtonPressed()
    {
        if(!_connected)
        {
            EmitSignal(nameof(ConnectToCOM));
        }
        else
        {
            EmitSignal(nameof(DisconnectFromCOM));
        }
        
    }

    public void OnRefreshButtonPressed()
    {
        EmitSignal(nameof(RefreshPortList));
    }

    public void OnStartButtonPressed()
    {
        EmitSignal(nameof(Start));
    }

    public void OnStopButtonPressed()
    {
        EmitSignal(nameof(Stop));
    }

    public void OnConnected()
    {
        _connected = true;
        GetNode<Button>("COMsettings_VBoxContainer/Connect_Button").Text = "Disconnect";
    }

    public void OnDisconnected()
    {
        _connected = false;
        GetNode<Button>("COMsettings_VBoxContainer/Connect_Button").Text = "Connect";
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
