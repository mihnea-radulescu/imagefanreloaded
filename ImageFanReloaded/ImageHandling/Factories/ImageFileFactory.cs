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
		IThumbnailCacheOptions thumbnailCacheOptions,
		IFileSizeEngine fileSizeEngine,
		IImageFileContentLogic directImageFileContentLogic,
		IImageFileContentLogic cachedImageFileContentLogic)
	{
		_globalParameters = globalParameters;

		_imageResizer = imageResizer;
		_fileSizeEngine = fileSizeEngine;

		_directImageFileContentLogic = directImageFileContentLogic;
		_cachedImageFileContentLogic = cachedImageFileContentLogic;

		_activeImageFileContentLogic =
			thumbnailCacheOptions.EnableThumbnailCaching
				? cachedImageFileContentLogic
				: directImageFileContentLogic;
	}

	public void EnableThumbnailCaching()
	{
		_activeImageFileContentLogic = _cachedImageFileContentLogic;
	}

	public void DisableThumbnailCaching()
	{
		_activeImageFileContentLogic = _directImageFileContentLogic;
	}

	public IImageFile GetImageFile(ImageFileData imageFileData)
		=> new ImageFile(
			_globalParameters,
			_imageResizer,
			_fileSizeEngine,
			_activeImageFileContentLogic,
			imageFileData);

	private readonly IGlobalParameters _globalParameters;

	private readonly IImageResizer _imageResizer;
	private readonly IFileSizeEngine _fileSizeEngine;

	private readonly IImageFileContentLogic _directImageFileContentLogic;
	private readonly IImageFileContentLogic _cachedImageFileContentLogic;

	private IImageFileContentLogic _activeImageFileContentLogic;
}
