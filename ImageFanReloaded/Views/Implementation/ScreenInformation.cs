using Avalonia.Controls;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views.Implementation;

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
