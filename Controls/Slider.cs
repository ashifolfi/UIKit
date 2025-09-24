using SkiaSharp;
using UIKit.Styles;

namespace UIKit.Controls;

public class Slider : Widget
{
    private const float THUMB_SIZE = 12.0f;
    private const float HALF_SIZE = THUMB_SIZE / 2;
    private const float THUMB_PADDING = 6.0f;
    private const float CONTROL_PADDING = 6.0f;
    
    public float Value
    {
        get => m_Value;
        set
        {
            m_Value = Math.Clamp(value, m_MinValue, m_MaxValue);
            OnValueChanged?.Invoke(this, new ValueChangedEventArgs()
            {
                Value = m_Value
            });
        }
    }

    private float m_Value = 0.5f;
    private float m_MaxValue = 1.0f;
    private float m_MinValue = 0.0f;
    
    private bool m_IsHeld = false;
    
    public EventHandler? OnValueChanged;
    
    public Slider(Widget? parent) : base(parent)
    {
    }

    protected override void OnPaintEvent()
    {
        using var canvas = Surface.Canvas;
        
        canvas.DrawRect(
            0, 0, Size.X, Size.Y,
            new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 2
            }
        );
        
        var vertCenter = Size.Y / 2;
        
        var percentage = (m_Value - m_MinValue) / (m_MaxValue - m_MinValue);
        var thumbPos = (percentage * (Size.X - (THUMB_SIZE + CONTROL_PADDING * 2))) + (HALF_SIZE + CONTROL_PADDING);
        
        canvas.DrawCircle(
            thumbPos, vertCenter,
            HALF_SIZE, new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = UIKitApplication.Style.GetStyleColor(UIKitStyle.StyleColor.Text),
            }
        );
        
        var lEnd = thumbPos - (HALF_SIZE + (THUMB_PADDING / 2));
        var rStart = thumbPos + (HALF_SIZE + (THUMB_PADDING / 2));

        if (lEnd >= HALF_SIZE + CONTROL_PADDING)
        {
            canvas.DrawLine(
                HALF_SIZE + CONTROL_PADDING, vertCenter,
                lEnd, vertCenter,
                new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2,
                    Color = UIKitApplication.Style.GetStyleColor(UIKitStyle.StyleColor.Text)
                }
            );
        }

        if (rStart < Size.X - (HALF_SIZE + CONTROL_PADDING))
        {
            canvas.DrawLine(
                rStart, vertCenter,
                Size.X - (HALF_SIZE + CONTROL_PADDING), vertCenter,
                new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2,
                    Color = UIKitApplication.Style.GetStyleColor(UIKitStyle.StyleColor.Border)
                }
            );
        }
    }

    protected override bool OnMouseButtonEvent(MbEventData data)
    {
        var vertCenter = Size.Y / 2;
        var percentage = (m_Value - m_MinValue) / (m_MaxValue - m_MinValue);
        var thumbPos = (percentage * (Size.X - (THUMB_SIZE + CONTROL_PADDING * 2))) + (HALF_SIZE + CONTROL_PADDING);
        thumbPos += GlobalPosition.X;
        vertCenter += GlobalPosition.Y;
        
        if (
            (data.X >= thumbPos - HALF_SIZE && data.X <= thumbPos + HALF_SIZE)
            && (data.Y >= vertCenter - HALF_SIZE && data.Y <= vertCenter + HALF_SIZE)
            )
        {
            m_IsHeld = data.Pressed;
            return true;
        }

        if (!data.Pressed) m_IsHeld = false;

        return false;
    }

    protected override bool OnMouseMotionEvent(MmEventData data)
    {
        var offsetX = data.X - (GlobalPosition.X + HALF_SIZE + CONTROL_PADDING);
        
        if (m_IsHeld && data.Dx != 0.0f)
        {
            Value = (offsetX / (Size.X - (THUMB_SIZE + CONTROL_PADDING * 2))) * (m_MaxValue - m_MinValue) + m_MinValue;
            return true;
        }
        
        return base.OnMouseMotionEvent(data);
    }
}