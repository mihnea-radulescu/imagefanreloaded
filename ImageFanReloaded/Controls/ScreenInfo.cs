using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Controls;

public class ScreenInfo : IScreenInfo
{
	public ImageSize GetScaledScreenSize(object currentWindowObject)
	{
		var currentWindow = (Window)currentWindowObject;
		var currentScreen = currentWindow.Screens.ScreenFromWindow(
			currentWindow)!;

		var screenBounds = currentScreen.Bounds;
		var screenScaling = currentScreen.Scaling;

		var scaledScreenWidth = (int)(screenBounds.Width / screenScaling);
		var scaledScreenHeight = (int)(screenBounds.Height / screenScaling);

		var scaledScreenSize = new ImageSize(
			scaledScreenWidth, scaledScreenHeight);
		return scaledScreenSize;
	}

	public ImageSize GetHalfScaledScreenSize(object currentWindowObject)
	{
		var scaledScreenSize = GetScaledScreenSize(currentWindowObject);

		var halfScaledScreenSize = new ImageSize(
			scaledScreenSize.Width / 2, scaledScreenSize.Height / 2);
		return halfScaledScreenSize;
	}
}
