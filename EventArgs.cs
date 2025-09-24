namespace UIKit;

public class ValueChangedEventArgs : EventArgs
{
    public float Value { get; set; }
}

public class ButtonChangedEventArgs : EventArgs
{
    public bool Pressed { get; set; }
}