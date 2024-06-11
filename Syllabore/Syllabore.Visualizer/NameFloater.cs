using Godot;
using System;

public partial class NameFloater : Node2D
{
    [Export]
    public int Speed = 10;

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
        if (this.Position.Y < -20)
        {
            this.QueueFree();
        }
        else
        {
            this.Position += Vector2.Up * Speed * (float)delta;
        }
    }

}
