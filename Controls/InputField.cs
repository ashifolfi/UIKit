using System.Numerics;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using SkiaSharp;
using UIKit.Styles;

namespace UIKit.Controls;

public class InputField : Widget, IFocusable
{
    public string Placeholder = "";
    public string Text = "";
    public bool IsFocused { get; set; }

    public EventHandler<string>? OnTextChanged;
    public EventHandler<string>? OnSubmit;
    
    private int m_FieldPos;

    public InputField(Widget? parent) : base(parent)
    {
    }

    protected override bool OnMouseButtonEvent(MbEventData data)
    {
        switch (data)
        {
            case { Button: MbEventData.LeftButton, Pressed: true }:
                // fuck ass line of code
                ((IFocusable)this).Focus();
                return true;
        }

        return false;
    }

    protected override bool OnKeyboardEvent(KbEventData data)
    {
        if (data.Pressed)
        {
            switch (data.Key)
            {
                case KeyCode.KBackspace or KeyCode.KKPBackspace:
                {
                    if (Text.Length > 0)
                    {
                        var postText = Text[m_FieldPos..];
                        Text = Text.Remove(m_FieldPos - 1) + postText;
                        m_FieldPos--;
                    }

                    break;
                }
                case KeyCode.KDelete:
                {
                    if (Text.Length > 0 && m_FieldPos != Text.Length)
                    {
                        var postText = Text[(m_FieldPos + 1)..];
                        Text = Text.Remove(m_FieldPos) + postText;
                    }

                    break;
                }
                case KeyCode.KLeft:
                    if (m_FieldPos > 0)
                        m_FieldPos--;
                    break;
                case KeyCode.KRight:
                    if (m_FieldPos < Text.Length)
                        m_FieldPos++;
                    break;
            }
        }
        return true;
    }

    protected override bool OnTextInputEvent(string text)
    {
        Text = Text.Insert(m_FieldPos, text);
        m_FieldPos += text.Length;
        return true;
    }

    protected override void OnPaintEvent()
    {
        using var canvas = Surface.Canvas;

        using var painter = new SKPaint();
        painter.Color = UIKitApplication.Style.GetStyleColor(UIKitStyle.StyleColor.Text);
        painter.IsStroke = false;
        painter.IsAntialias = true;

        UIKitApplication.Style.DrawPrimitive(canvas, UIKitStyle.PrimitiveType.FieldFrame, UIKitStyle.StyleState.Normal, this);

        // todo: metrics for padding and whatnot in styles
        var font = UIKitApplication.Style.DefaultTypeface.ToFont(14.0f);
        canvas.DrawText(Text == string.Empty ? Placeholder : Text,
            13, (Size.Y / 2) + ((font.Metrics.CapHeight - font.Metrics.Descent) / 2), font,
            painter
        );
        
        // draw text cursor
        // todo: blink cursor
        if (IsFocused)
        {
            Span<ushort> glyphs = stackalloc ushort[m_FieldPos];
            font.GetGlyphs(Text.AsSpan(0, m_FieldPos), glyphs);
            var width = font.MeasureText(glyphs, painter);
            
            canvas.DrawText("|", 
                13 + width,
                (Size.Y / 2) + ((font.Metrics.CapHeight - font.Metrics.Descent) / 2),
                SKTextAlign.Left,
                font,
                painter
            );
        }
    }

    // we don't implement OnFocusEvent as we don't do anything special when focused
}
