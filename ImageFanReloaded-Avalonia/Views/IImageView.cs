using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views;

public interface IImageView
{
    event EventHandler<ThumbnailChangedEventArgs> ThumbnailChanged;

	void SetImage(IImageFile imageFile);

    Task ShowDialog(Window owner);
}
