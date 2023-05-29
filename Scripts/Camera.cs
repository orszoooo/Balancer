using Godot;
using System;

public class Camera : Godot.Camera
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void OnViewReset()
    {
        this.Size = 3.0f;
    }

    public void OnViewZoomIn()
    {
        this.Size -= 0.25f;
    }

    public void OnViewZoomOut()
    {
        this.Size += 0.25f;
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
