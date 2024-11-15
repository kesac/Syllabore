using Godot;
using System;
using System.Collections.Generic;

public partial class ShiftingLabel : Label
{
    private Random _random;

    // Strings to shift through
    public List<string> Values;

    public override void _Ready()
    {
        _random = new Random();
        this.Values = new List<string>();
        this.GetNode<Timer>("Timer").Start();
    }

    // Randomly select a value from the list
    // and set the value of this label
    public void Shift()
    {
        if (this.Values.Count > 0)
        {
            var index = _random.Next(0, this.Values.Count);
            this.Text = this.Values[index].ToUpper();
        }
    }

}
