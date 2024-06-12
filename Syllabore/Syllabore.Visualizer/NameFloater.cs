using Godot;
using Syllabore.Visualizer;
using System;
using static Godot.Control;
using System.Xml;

public partial class NameFloater : Node2D
{
    [Export]
    public int Speed = 10;

    private ITemporalController _temporalController;


    public override void _Ready()
    {
        this.GetNode<Label>("NameLabel").MouseFilter = MouseFilterEnum.Stop;
    }

    public void SetTemporalController(ITemporalController temporalController)
    {
        _temporalController = temporalController;
    }

    public void HighlightText()
    {
        this.GetNode<Label>("NameLabel").Modulate = new Color(1, 1, 0);
    }

    public void NormalizeText()
    {
        this.GetNode<Label>("NameLabel").Modulate = new Color(1, 1, 1);
    }

    public string Text { 
        get
        {
            return this.GetNode<Label>("NameLabel").Text;
        }
        set
        {
            this.GetNode<Label>("NameLabel").Text = value;
        }
    }

    public override void _Process(double delta)
    {
        if (_temporalController != null && !_temporalController.IsPaused)
        {
            if (this.Position.Y > 820) // -20
            {
                // Remove the floater once it's off screen
                this.QueueFree();
            }
            else
            {
                var adjustedSpeed = Speed;

                if (_temporalController.IsSlowed)
                {
                    adjustedSpeed = Speed / 8;
                }

                this.Position += Vector2.Down * adjustedSpeed * (float)delta;
            }
        }
    }

}
