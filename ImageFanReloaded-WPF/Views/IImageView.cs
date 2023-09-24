using System;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views
{
    public interface IImageView
    {
        event EventHandler<ThumbnailChangedEventArgs> ThumbnailChanged;

		IScreenInformation ScreenInformation { get; set; }

		void SetImage(IImageFile imageFile);

        bool? ShowDialog();
    }
}
