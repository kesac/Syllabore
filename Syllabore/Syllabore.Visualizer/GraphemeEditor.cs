using Godot;
using System;

public partial class GraphemeEditor : Control
{
    public event Action GraphemeEditorChanged;

    protected virtual void OnGraphemeEditorChanged()
    {
        GraphemeEditorChanged?.Invoke();
    }

    protected virtual void OnVSliderChanged(float value)
    {
        this.UpdatePercentageLabel(value);
        GraphemeEditorChanged?.Invoke();
    }

    public string LabelText
    {
        get
        {
            return this.GetNode<Label>("LeftPanel/Label").Text;
        }
        set
        {
            this.GetNode<Label>("LeftPanel/Label").Text = value;
        }
    }

    public string Text
    {
        get
        {
            return this.GetNode<TextEdit>("LeftPanel/TextEdit").Text;
        }
        set
        {

            this.GetNode<TextEdit>("LeftPanel/TextEdit").Text = value;
        }
    }

    public double Probability
    {
        get
        {
            var vSlider = this.GetNode<VSlider>("RightPanel/VSlider");
            return vSlider.Value / vSlider.MaxValue; // Normalize to [0,1]
        }
        set
        {
            var vSlider = this.GetNode<VSlider>("RightPanel/VSlider");
            this.GetNode<VSlider>("RightPanel/VSlider").Value = value * vSlider.MaxValue; // Normalize to [0,100]
            this.UpdatePercentageLabel(value * vSlider.MaxValue);
        }
    }

    private void UpdatePercentageLabel(double value)
    {
        this.GetNode<Label>("RightPanel/PercentageLabel").Text = $"{(int)value}%";
    }
}
