using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IImageView
{
	IGlobalParameters? GlobalParameters { get; set; }

	IScreenInformation? ScreenInformation { get; set; }
	ITabOptions? TabOptions { get; set; }

	bool IsStandaloneView { get; set; }

	event EventHandler<ImageViewClosingEventArgs>? ViewClosing;
	event EventHandler<ImageChangedEventArgs>? ImageChanged;

	bool CanAdvanceToDesignatedImage { get; set; }

	Task<bool> CanStartSlideshowFromContentTabItem();
	Task StartSlideshowFromContentTabItem();

	Task SetImage(IImageFile imageFile);

	void Show();
	Task ShowDialog(IMainView owner);
}
