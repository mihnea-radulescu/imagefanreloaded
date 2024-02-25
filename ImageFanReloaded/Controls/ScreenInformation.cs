using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Controls;

public class ScreenInformation : IScreenInformation
{
	public ScreenInformation(Window mainWindow)
    {
		_mainWindow = mainWindow;
	}

    public ImageSize GetScreenSize()
	{
		var screenBounds = _mainWindow.Screens.ScreenFromWindow(_mainWindow)!.Bounds;
		var screenSize = new ImageSize(screenBounds.Width, screenBounds.Height);

		return screenSize;
	}

	#region Private

	private readonly Window _mainWindow;

	#endregion
}
