using ImageFanReloaded.Views;
using ImageFanReloaded.Views.Implementation;

namespace ImageFanReloaded.Factories.Implementation;

public class ImageViewFactory : IImageViewFactory
{
	public ImageViewFactory(IScreenInformation screenInformation)
	{
		_screenInformation = screenInformation;
	}

	public IImageView GetImageView()
	{
		var imageWindow = new ImageWindow
		{
			ScreenInformation = _screenInformation
		};

		return imageWindow;
	}

	#region Private

	private readonly IScreenInformation _screenInformation;

	#endregion
}
