using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views;

public interface IImageView
{
    event EventHandler<ThumbnailChangedEventArgs>? ThumbnailChanged;

	IScreenInformation? ScreenInformation { get; set; }

	void SetImage(IImageFile imageFile);

    Task ShowDialog(Window owner);
}
