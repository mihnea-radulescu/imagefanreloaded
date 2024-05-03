using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Controls;

public class ScreenInformation : IScreenInformation
{
	public ScreenInformation(Window currentWindow)
    {
		_currentWindow = currentWindow;
	}

    public ImageSize GetScreenSize()
	{
		var screenBounds = _currentWindow.Screens.ScreenFromWindow(_currentWindow)!.Bounds;
		var screenSize = new ImageSize(screenBounds.Width, screenBounds.Height);

		return screenSize;
	}

	#region Private

	private readonly Window _currentWindow;

	#endregion
}
