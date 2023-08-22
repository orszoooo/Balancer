using Godot;
using System;

public class ViewSettings : PanelContainer
{
    [Signal]
    public delegate void ViewReset();
    [Signal]
    public delegate void ViewZoomIn();

    [Signal]
    public delegate void ViewZoomOut();

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
}
