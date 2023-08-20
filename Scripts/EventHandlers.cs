using Godot;
using System.IO.Ports;

public partial class Main : Spatial
{
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

    private void OnSendParameters()
    {
        GD.Print("Send button clicked");

        if(_balancer.SerialCom.IsOpen)
        {
            SendSetPoint();
            SendPID();
        }
    }

    private void SendSetPoint()
    {
        //Set point XY
        int newSetPointX = (int)ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/Xpos_HBoxContainer/Xpos_LineEdit").Text);

        int newSetPointY = (int)ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/Ypos_HBoxContainer/Ypos_LineEdit").Text);

        if (newSetPointX != _balancer.XSetpoint || newSetPointY != _balancer.YSetpoint)
        {
            if (newSetPointX > _balancer.xPanelResolution)
                newSetPointX = 0;

            if (newSetPointY > _balancer.yPanelResolution)
                newSetPointY = 0;

            _balancer.SerialCom.WriteLine($"XY {newSetPointX} {newSetPointY}");
        }
    }

    private void SendPID()
    {
        //PID 1
        double newP1 = ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/P1_HBoxContainer/P1_LineEdit").Text);

        double newI1 = ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/I1_HBoxContainer/I1_LineEdit").Text);

        double newD1 = ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/D1_HBoxContainer/D1_LineEdit").Text);

        if (newP1 != _balancer.PIDRegulator1.P || newI1 != _balancer.PIDRegulator1.I || newD1 != _balancer.PIDRegulator1.D)
        {
            if (newP1 < 0.0)
                newP1 = 0.0;

            if (newI1 < 0.0)
                newI1 = 0.0;

            if(newD1 < 0.0)
                newD1 = 0.0;

            _balancer.SerialCom.WriteLine($"PID1 {newP1} {newI1} {newD1}");
        }

        //PID 2
        double newP2 = ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/P2_HBoxContainer/P2_LineEdit").Text);

        double newI2 = ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/I2_HBoxContainer/I2_LineEdit").Text);

        double newD2 = ReceivedStringToNumber(
            GetNode<LineEdit>("UI/DataCOM_VBoxContainer/SetParam/SetParam_VBoxContainer/D2_HBoxContainer/D2_LineEdit").Text);

        if (newP2 != _balancer.PIDRegulator2.P || newI2 != _balancer.PIDRegulator2.I || newD2 != _balancer.PIDRegulator2.D)
        {
            if (newP2 < 0.0)
                newP2 = 0.0;

            if (newI2 < 0.0)
                newI2 = 0.0;

            if(newD2 < 0.0)
                newD2 = 0.0;

            _balancer.SerialCom.WriteLine($"PID2 {newP2} {newI2} {newD2}");
        }
    }
}