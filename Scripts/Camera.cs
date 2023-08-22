using Godot;
using System;

public class Camera : Godot.Camera
{
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

}
