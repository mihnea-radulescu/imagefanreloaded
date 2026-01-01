using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Controls;

public class ScreenInfo : IScreenInfo
{
	public ImageSize GetScaledScreenSize(object currentWindowObject)
	{
		var currentWindow = (Window)currentWindowObject;
		var currentScreen = currentWindow.Screens.ScreenFromWindow(currentWindow)!;

		var screenBounds = currentScreen.Bounds;
		var screenOrientation = currentScreen.CurrentOrientation;
		var screenScaling = currentScreen.Scaling;

		int screenWidth;
		int screenHeight;

		var shouldSwapDimensions = ShouldSwapDimensions(screenBounds, screenOrientation);
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

	private static bool ShouldSwapDimensions(PixelRect screenBounds, ScreenOrientation screenOrientation)
	{
		var areLandscapeDimensions = screenBounds.Width > screenBounds.Height;
		var isPortraitOrientation =
			screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitFlipped;

		var shouldSwapDimensions = areLandscapeDimensions && isPortraitOrientation;
		return shouldSwapDimensions;
	}

	#endregion
}
