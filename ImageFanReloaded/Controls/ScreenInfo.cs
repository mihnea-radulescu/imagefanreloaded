using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Controls;

public class ScreenInfo : IScreenInfo
{
	public ScreenInfo(Window currentWindow)
	{
		_currentWindow = currentWindow;
	}

	public ImageSize GetScaledScreenSize()
	{
		var currentScreen = _currentWindow.Screens.ScreenFromWindow(_currentWindow)!;

		var screenBounds = currentScreen.Bounds;
		var screenScaling = currentScreen.Scaling;

		var scaledScreenWidth = (int)((double)screenBounds.Width / screenScaling);
		var scaledScreenHeight = (int)((double)screenBounds.Height / screenScaling);

		var scaledScreenSize = new ImageSize(scaledScreenWidth, scaledScreenHeight);

		return scaledScreenSize;
	}

	#region Private

	private readonly Window _currentWindow;

	#endregion
}
