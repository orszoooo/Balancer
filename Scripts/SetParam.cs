using Godot;
using System;

public class SetParam : PanelContainer
{
    [Signal]
    public delegate void SendParameters();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void OnSendButtonPressed()
    {
        EmitSignal(nameof(SendParameters));
    }
}
