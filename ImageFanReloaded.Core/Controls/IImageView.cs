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
	
	event EventHandler<ImageChangedEventArgs>? ImageChanged;

	void SetImage(IImageFile imageFile);

    Task ShowDialog(IMainView owner);
    void Show();
}
