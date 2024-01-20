using System;

namespace ImageFanReloaded.CommonTypes.CustomEventArgs;

public class ThumbnailChangedEventArgs
    : EventArgs
{
    public ThumbnailChangedEventArgs(int increment)
    {
        Increment = increment;
    }

    public int Increment { get; }
}
