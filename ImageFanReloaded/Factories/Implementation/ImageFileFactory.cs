using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

namespace ImageFanReloaded.Factories.Implementation;

public class ImageFileFactory
	: IImageFileFactory
{
	public ImageFileFactory(IImageResizer imageResizer)
	{
		_imageResizer = imageResizer;
	}

	public IImageFile GetImageFile(string filePath)
		=> new ImageFile(_imageResizer, filePath);

	#region Private

	private readonly IImageResizer _imageResizer;

	#endregion
}
