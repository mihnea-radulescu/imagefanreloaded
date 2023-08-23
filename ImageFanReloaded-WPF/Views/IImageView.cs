﻿using System;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views
{
    public interface IImageView
    {
        event EventHandler<ThumbnailChangedEventArgs> ThumbnailChanged;

		void SetImage(ImageSize screenSize, IImageFile imageFile);

        bool? ShowDialog();
    }
}
