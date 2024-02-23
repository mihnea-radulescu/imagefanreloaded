using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Global;

namespace ImageFanReloaded.Controls;

public class ImageViewFactory : IImageViewFactory
{
	public ImageViewFactory(
		IGlobalParameters globalParameters,
		IScreenInformation screenInformation)
	{
		_globalParameters = globalParameters;
		_screenInformation = screenInformation;
	}

	public IImageView GetImageView()
	{
		IImageView imageView = new ImageWindow();
		
		imageView.ScreenInformation = _screenInformation;
		imageView.GlobalParameters = _globalParameters;

		return imageView;
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IScreenInformation _screenInformation;

	#endregion
}
