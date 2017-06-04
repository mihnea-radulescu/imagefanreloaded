using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using System;

namespace ImageFanReloaded.Views.Interface
{
    public interface IImageView
    {
        event EventHandler<ThumbnailChangedEventArgs> ThumbnailChanged;
        
        void SetImage(IImageFile imageFile);

        bool? ShowDialog();
    }
}
