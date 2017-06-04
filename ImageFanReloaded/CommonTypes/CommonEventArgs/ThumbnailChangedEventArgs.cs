using System;

namespace ImageFanReloaded.CommonTypes.CommonEventArgs
{
    public class ThumbnailChangedEventArgs
        : EventArgs
    {
        public ThumbnailChangedEventArgs(int increment)
        {
            Increment = increment;
        }

        public int Increment { get; private set; }
    }
}
