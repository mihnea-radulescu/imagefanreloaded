using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

public class ImageInfoViewFactory : IImageInfoViewFactory
{
	public ImageInfoViewFactory(
		IGlobalParameters globalParameters,
		IImageInfoBuilder imageInfoBuilder)
	{
		_globalParameters = globalParameters;
		_imageInfoBuilder = imageInfoBuilder;
	}

	public IImageInfoView GetImageInfoView(IImageFile imageFile)
	{
		IImageInfoView imageInfoView = new ImageInfoWindow();

		imageInfoView.GlobalParameters = _globalParameters;
		imageInfoView.ImageInfoBuilder = _imageInfoBuilder;

		imageInfoView.ImageFile = imageFile;

		return imageInfoView;
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageInfoBuilder _imageInfoBuilder;

	#endregion
}
