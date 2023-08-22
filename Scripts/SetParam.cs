using Godot;
using System;

public class SetParam : PanelContainer
{
    [Signal]
    public delegate void SendParameters();

    public override void _Ready()
    {
        
    }

    public void OnSendButtonPressed()
    {
        EmitSignal(nameof(SendParameters));
    }
}
