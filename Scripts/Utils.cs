using Godot;
using System.Globalization;

public partial class Main : Spatial
{
    // Parses input from serial port into List of doubles that is used to update UI elements
    public void ParseInput(string inputParam)
    {
        if(_showReceivedMessages)
            GD.Print("RECEIVED: " + inputParam);

        string[] splitParam = inputParam.Split(' ');
        bool syncActive = false;

        for(int i = 0; i < splitParam.Length; i++)
        {
            if(splitParam[i] == "SYNC" || syncActive)
            {
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
        if(double.TryParse(numberString,NumberStyles.Number, CultureInfo.InvariantCulture, out double result))
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
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Xset_HBoxContainer/Xset_LineEdit").Text 
            = _balancer.XSetpoint.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Yset_HBoxContainer/Yset_LineEdit").Text 
            = _balancer.YSetpoint.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Xpos_HBoxContainer/Xpos_LineEdit").Text 
            = _balancer.XCurrentPosition.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Ypos_HBoxContainer/Ypos_LineEdit").Text 
            = _balancer.YCurrentPosition.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Err1_HBoxContainer/Err1_LineEdit").Text 
            = _balancer.PIDRegulator1.Error.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Y1_HBoxContainer/Y1_LineEdit").Text 
            = _balancer.PIDRegulator1.RegulatorOutput.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/P1_HBoxContainer/P1_LineEdit").Text 
            = _balancer.PIDRegulator1.P.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/I1_HBoxContainer/I1_LineEdit").Text 
            = _balancer.PIDRegulator1.I.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/D1_HBoxContainer/D1_LineEdit").Text 
            = _balancer.PIDRegulator1.D.ToString(CultureInfo.InvariantCulture);
            
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Err2_HBoxContainer/Err2_LineEdit").Text 
            = _balancer.PIDRegulator2.Error.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/Y2_HBoxContainer/Y2_LineEdit").Text 
            = _balancer.PIDRegulator2.RegulatorOutput.ToString(CultureInfo.InvariantCulture);

        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/P2_HBoxContainer/P2_LineEdit").Text 
            = _balancer.PIDRegulator2.P.ToString(CultureInfo.InvariantCulture);
        
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/I2_HBoxContainer/I2_LineEdit").Text 
            = _balancer.PIDRegulator2.I.ToString(CultureInfo.InvariantCulture);
        
        GetNode<LineEdit>("UI/DataReadout/DataReadout_VBoxContainer/D2_HBoxContainer/D2_LineEdit").Text 
            = _balancer.PIDRegulator2.D.ToString(CultureInfo.InvariantCulture);
    }

    private void UpdateBall()
    {
        GetNode<Spatial>("Balancer3D/Ball").GlobalTranslation = new Vector3(
            (float)_balancer.XCurrentPosition/100.0f - ((_balancer.xPanelResolution/100.0f)/2.0f),
             0.0f, 
            (float)_balancer.YCurrentPosition/100.0f - ((_balancer.yPanelResolution/100.0f)/2.0f)
        );
    }
}