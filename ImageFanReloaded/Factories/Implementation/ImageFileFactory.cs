using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

namespace ImageFanReloaded.Factories.Implementation;

public class ImageFileFactory : IImageFileFactory
{
	public ImageFileFactory(IImageResizer imageResizer)
	{
		_imageResizer = imageResizer;
	}

	public IImageFile GetImageFile(string fileName, string filePath, int sizeOnDiscInKilobytes)
		=> new ImageFile(_imageResizer, fileName, filePath, sizeOnDiscInKilobytes);

	#region Private

	private readonly IImageResizer _imageResizer;

	#endregion
}
