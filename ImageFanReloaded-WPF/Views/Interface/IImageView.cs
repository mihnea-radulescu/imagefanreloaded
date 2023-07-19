using System;

using ImageFanReloadedWPF.CommonTypes.CommonEventArgs;
using ImageFanReloadedWPF.CommonTypes.ImageHandling.Interface;

namespace ImageFanReloadedWPF.Views.Interface
{
    public interface IImageView
    {
        event EventHandler<ThumbnailChangedEventArgs> ThumbnailChanged;
        
        void SetImage(IImageFile imageFile);

        bool? ShowDialog();
    }
}
