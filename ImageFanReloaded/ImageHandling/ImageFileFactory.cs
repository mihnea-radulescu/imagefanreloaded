using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageFileFactory : IImageFileFactory
{
	public ImageFileFactory(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		IImageOrientationHandler imageOrientationHandler)
	{
		_globalParameters = globalParameters;
		_imageResizer = imageResizer;

		_imageOrientationHandler = imageOrientationHandler;
	}

	public IImageFile GetImageFile(ImageFileData imageFileData)
		=> new ImageFile(
			_globalParameters,
			_imageResizer,
			imageFileData,
			_imageOrientationHandler);

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;

	private readonly IImageOrientationHandler _imageOrientationHandler;

	#endregion
}
