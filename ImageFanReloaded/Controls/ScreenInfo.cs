using Avalonia;
using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Controls;

public class ScreenInfo : IScreenInfo
{
	public ImageSize GetScaledScreenSize(object currentWindowObject)
	{
		var currentWindow = (Window)currentWindowObject;
		var currentScreen = currentWindow.Screens.ScreenFromWindow(currentWindow)!;

		var windowClientSize = currentWindow.ClientSize;
		var screenBounds = currentScreen.Bounds;
		var screenScaling = currentScreen.Scaling;

		int screenWidth;
		int screenHeight;

		var shouldSwapDimensions = ShouldSwapDimensions(screenBounds, windowClientSize);
		if (shouldSwapDimensions)
		{
			screenWidth = screenBounds.Height;
			screenHeight = screenBounds.Width;
		}
		else
		{
			screenWidth = screenBounds.Width;
			screenHeight = screenBounds.Height;
		}

		var scaledScreenWidth = (int)(screenWidth / screenScaling);
		var scaledScreenHeight = (int)(screenHeight / screenScaling);

		var scaledScreenSize = new ImageSize(scaledScreenWidth, scaledScreenHeight);
		return scaledScreenSize;
	}

	public ImageSize GetHalfScaledScreenSize(object currentWindowObject)
	{
		var scaledScreenSize = GetScaledScreenSize(currentWindowObject);

		var halfScaledScreenSize = new ImageSize(scaledScreenSize.Width / 2, scaledScreenSize.Height / 2);
		return halfScaledScreenSize;
	}

	#region Private

	private static bool ShouldSwapDimensions(PixelRect screenBounds, Size windowClientSize)
	{
		var areLandscapeDimensions = screenBounds.Width > screenBounds.Height;
		var isPortraitOrientation = windowClientSize.Width < windowClientSize.Height;

		var shouldSwapDimensions = areLandscapeDimensions && isPortraitOrientation;
		return shouldSwapDimensions;
	}

	#endregion
}
