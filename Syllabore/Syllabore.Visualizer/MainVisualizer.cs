using Godot;
using Syllabore;
using System;

public partial class MainVisualizer : Node2D
{
    private NameGenerator _nameGenerator;
    private PackedScene _nameFloaterScene;

    private int _speed = 100;
    private Random _random = new Random();
    private int _xPadding = 100;

    public override void _Ready()
    {
        _nameGenerator = new NameGenerator("ae","strl");
        _nameFloaterScene = GD.Load<PackedScene>("res://NameFloater.tscn");
    }

    public override void _Process(double delta)
    {

    }

    public void GenerateName()
    {

        var windowSize = GetViewportRect().Size;

        var xRoundTo = 50;
        var yRoundTo = 20;

        var xPosition = _xPadding + _random.Next((int)windowSize.X - (2 * _xPadding));
        xPosition = (xPosition / xRoundTo) * xRoundTo; // Round to nearest 50

        var yPosition = (int)windowSize.Y;
        yPosition = (yPosition / yRoundTo) * yRoundTo;

        var instance = (NameFloater)_nameFloaterScene.Instantiate();

        instance.Text = _nameGenerator.Next();
        instance.Speed = 100;
        instance.Position = new Vector2(xPosition, yPosition);

        this.GetTree().CurrentScene.AddChild(instance);
    }

}
