using Godot;
using Syllabore;
using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Syllabore.Visualizer;
using static Godot.Control;

public partial class MainVisualizer : Node2D, ITemporalController
{
    private static readonly string DefaultVowels = "ae";
    private static readonly string DefaultLeadingConsonants = "str";
    private static readonly string DefaultTrailingConsonants = "lmn";

    private SyllableGenerator _syllableGenerator;
    private NameGenerator _nameGenerator;
    private PackedScene _nameFloaterScene;

    private Random _random = new Random();
    private int _xLeftPadding = 300; // Make room for the menu stuff
    private int _xRightPadding = 100; // Ensure names fit

    public bool IsSlowed { get; set; }
    public bool IsPaused { get; set; }

    public override void _Ready()
    {
        _syllableGenerator = new SyllableGenerator()
            .WithVowels(DefaultVowels)
            .WithLeadingConsonants(DefaultLeadingConsonants)
            .WithTrailingConsonants(DefaultTrailingConsonants);

        _nameGenerator = new NameGenerator(_syllableGenerator).UsingSyllableCount(2, 3);
        _nameFloaterScene = GD.Load<PackedScene>("res://NameFloater.tscn");

        this.RefreshEditorTextValues();
    }

    public void RefreshEditorTextValues()
    {

        var vowels = string.Join(string.Empty, _syllableGenerator.Vowels.Select(x => x.Value));
        var leadingConsonants = string.Join(string.Empty, _syllableGenerator.LeadingConsonants.Select(x => x.Value));
        var trailingConsonants = string.Join(string.Empty, _syllableGenerator.TrailingConsonants.Select(x => x.Value));

        this.GetNode<TextEdit>("Editor/Vowels").Text = vowels;
        this.GetNode<TextEdit>("Editor/LeadingConsonants").Text = leadingConsonants;
        this.GetNode<TextEdit>("Editor/TrailingConsonants").Text = trailingConsonants;

        this.RefreshShiftingLabels();

    }

    public void RefreshSyllableGenerator()
    {
        var vowels = this.GetNode<TextEdit>("Editor/Vowels").Text;
        var leadingConsonants = this.GetNode<TextEdit>("Editor/LeadingConsonants").Text;
        var trailingConsonants = this.GetNode<TextEdit>("Editor/TrailingConsonants").Text;

        _syllableGenerator = new SyllableGenerator()
            .WithVowels(vowels)
            .WithLeadingConsonants(leadingConsonants)
            .WithTrailingConsonants(trailingConsonants);

        _nameGenerator = new NameGenerator(_syllableGenerator).UsingSyllableCount(2, 3);

        this.RefreshShiftingLabels();
    }

    public void RefreshShiftingLabels()
    {
        this.GetNode<ShiftingLabel>("Editor/VowelLabel").Values = _syllableGenerator.Vowels.Select(x => x.Value).ToList();
        this.GetNode<ShiftingLabel>("Editor/LeadingConsonantLabel").Values = _syllableGenerator.LeadingConsonants.Select(x => x.Value).ToList();
        this.GetNode<ShiftingLabel>("Editor/TrailingConsonantLabel").Values = _syllableGenerator.TrailingConsonants.Select(x => x.Value).ToList();
    }

    public override void _Process(double delta)
    {

    }

    public void PauseFloaters()
    {
        this.IsPaused = true;
        this.IsSlowed = false;
    }

    public void SlowDownFloaters()
    {
        this.IsPaused = false;
        this.IsSlowed = true;
    }

    public void ResumeFloaters()
    {
        this.IsSlowed = false;
        this.IsPaused = false;
    }

    public void GenerateName()
    {
        if (!this.IsPaused && !this.IsSlowed && _syllableGenerator.Vowels.Count > 0)
        {
            var windowSize = GetViewportRect().Size;

            var xRoundTo = 20;
            var yRoundTo = 20;

            var xPosition = _xLeftPadding + _random.Next((int)windowSize.X - (_xLeftPadding + _xRightPadding));
            xPosition = (xPosition / xRoundTo) * xRoundTo; // Round to nearest 50

            //var yPosition = (int)windowSize.Y;
            var yPosition = -20; // Start off screen
            yPosition = (yPosition / yRoundTo) * yRoundTo;

            var instance = (NameFloater)_nameFloaterScene.Instantiate();

            instance.SetTemporalController(this);

            instance.Text = _nameGenerator.Next();

            instance.Speed = 100; // Base speed only; the instance will adjust itself based on the TemporalController's state
            instance.Position = new Vector2(xPosition, yPosition);

            //connect instance label to the the MainVisualizer's PauseFloater method
            var nameLabel = instance.GetNode<Label>("NameLabel");
            nameLabel.MouseEntered += this.SlowDownFloaters;
            nameLabel.MouseExited += this.ResumeFloaters;

            this.GetTree().CurrentScene.AddChild(instance);
        }
    }

}
