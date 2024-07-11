using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageFileFactory : IImageFileFactory
{
	public ImageFileFactory(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer)
	{
		_globalParameters = globalParameters;
		_imageResizer = imageResizer;
	}

	public IImageFile GetImageFile(string fileName, string filePath, decimal sizeOnDiscInKilobytes)
		=> new ImageFile(_globalParameters, _imageResizer, fileName, filePath, sizeOnDiscInKilobytes);

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;

	#endregion
}
