using System;

namespace ImageFanReloadedWPF.CommonTypes.CommonEventArgs
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
