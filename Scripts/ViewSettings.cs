using Godot;
using System;

public class ViewSettings : PanelContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal]
    public delegate void ViewReset();
    [Signal]
    public delegate void ViewZoomIn();

    [Signal]
    public delegate void ViewZoomOut();



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void OnResetButtonPressed()
    {
        EmitSignal(nameof(ViewReset));
    }

    public void OnZoomInButtonPressed()
    {
        EmitSignal(nameof(ViewZoomIn));
    }

    public void OnZoomOutButtonPressed()
    {
        EmitSignal(nameof(ViewZoomOut));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
