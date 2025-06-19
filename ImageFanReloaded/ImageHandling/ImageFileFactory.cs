using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageFileFactory : IImageFileFactory
{
	public ImageFileFactory(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		IImageInfoBuilder imageInfoBuilder,
		IImageOrientationHandler imageOrientationHandler)
	{
		_globalParameters = globalParameters;
		_imageResizer = imageResizer;

		_imageInfoBuilder = imageInfoBuilder;
		_imageOrientationHandler = imageOrientationHandler;
	}

	public IImageFile GetImageFile(string fileName, string filePath, decimal sizeOnDiscInKilobytes)
		=> new ImageFile(
			_globalParameters,
			_imageResizer,
			fileName,
			filePath,
			sizeOnDiscInKilobytes,
			_imageInfoBuilder,
			_imageOrientationHandler);

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;

	private readonly IImageInfoBuilder _imageInfoBuilder;
	private readonly IImageOrientationHandler _imageOrientationHandler;

	#endregion
}
