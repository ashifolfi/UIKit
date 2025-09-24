namespace UIKit.Controls;

public class Checkbox : Button
{
    public Checkbox(Widget? parent, string label) : base(parent, label)
    {
    }

    protected override void OnPaintEvent()
    {
        using var canvas = Surface.Canvas;
        
        // todo: checkbox visuals
    }
}
