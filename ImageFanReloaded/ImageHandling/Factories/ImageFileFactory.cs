using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling.Factories;

public class ImageFileFactory : IImageFileFactory
{
	public ImageFileFactory(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		IFileSizeEngine fileSizeEngine,
		IImageFileContentLogic imageFileContentLogic)
	{
		_globalParameters = globalParameters;

		_imageResizer = imageResizer;
		_fileSizeEngine = fileSizeEngine;
		_imageFileContentLogic = imageFileContentLogic;
	}

	public IImageFile GetImageFile(ImageFileData imageFileData)
		=> new ImageFile(
			_globalParameters,
			_imageResizer,
			_fileSizeEngine,
			_imageFileContentLogic,
			imageFileData);

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IImageFileContentLogic _imageFileContentLogic;
}
