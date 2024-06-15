using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls;

public interface IImageView
{
	IGlobalParameters? GlobalParameters { get; set; }
	IScreenInformation? ScreenInformation { get; set; }

	event EventHandler<ImageViewClosingEventArgs>? ViewClosing;
	event EventHandler<ImageChangedEventArgs>? ImageChanged;

	void SetImage(IImageFile imageFile, bool recursiveFolderAccess);

	void Show();
    Task ShowDialog(IMainView owner);
}
