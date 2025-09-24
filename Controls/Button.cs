using System.Numerics;
using SkiaSharp;
using UIKit.Styles;

namespace UIKit.Controls;

public class Button : Widget
{
    public enum ButtonState
    {
        Normal,
        Hovered,
        Pressed
    }
    
    public string Text = "";
    public ButtonState State = ButtonState.Normal;
    public EventHandler? OnPressed;
    public bool ToggleMode = false;
    public bool Pressed = false;

    public Button(Widget? parent) : base(parent)
    {
    }

    public Button(Widget? parent, string text) : base(parent)
    {
        Text = text;
    }
    
    protected override bool OnMouseButtonEvent(MbEventData data)
    {
        switch (data)
        {
            case { Button: MbEventData.LeftButton, Pressed: true }:
                if (!ToggleMode) Pressed = true;
                return true;
            case { Button: MbEventData.LeftButton, Pressed: false }:
                if (ToggleMode)
                {
                    Pressed = !Pressed;
                }
                else
                {
                    Pressed = false;
                }
                OnPressed?.Invoke(this, new ButtonChangedEventArgs() { Pressed = Pressed });
                return true;
        }

        return false;
    }

    protected override void OnMouseEnterEvent()
    {
        IsHovered = true;
    }
    
    protected override void OnMouseExitEvent()
    {
        IsHovered = false;
    }
    
    protected override void OnUpdateEvent()
    {
        State = Pressed ? ButtonState.Pressed : IsHovered ? ButtonState.Hovered : ButtonState.Normal;
    }

    protected override void OnPaintEvent()
    {
        if (Surface == null)
            throw new NullReferenceException("Surface cannot be null");
        using var canvas = Surface.Canvas;
        
        switch (State)
        {
            case ButtonState.Pressed:
                UIKitApplication.Style.DrawControl(
                    canvas,
                    UIKitStyle.ControlType.Button,
                    UIKitStyle.StyleState.Pressed,
                    this
                );
                break;
            case ButtonState.Hovered:
                UIKitApplication.Style.DrawControl(
                    canvas,
                    UIKitStyle.ControlType.Button,
                    UIKitStyle.StyleState.Hovered,
                    this
                );
                break;
            case ButtonState.Normal:
            default:
                UIKitApplication.Style.DrawControl(
                    canvas,
                    UIKitStyle.ControlType.Button,
                    UIKitStyle.StyleState.Normal,
                    this
                );
                break;
        }
    }
}
