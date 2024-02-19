using System;
using ImageFanReloaded.Views;

namespace ImageFanReloaded.CommonTypes.CustomEventArgs;

public class ThumbnailChangedEventArgs : EventArgs
{
    public ThumbnailChangedEventArgs(IImageView imageView, int increment)
    {
        ImageView = imageView;
        Increment = increment;
    }

    public IImageView ImageView { get; }
    public int Increment { get; }
}
