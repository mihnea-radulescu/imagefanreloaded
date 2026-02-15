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
		IImageFileContentLogic imageFileContentLogic,
		IImageFileContentLogic cachedImageFileContentLogic)
	{
		_globalParameters = globalParameters;

		_imageResizer = imageResizer;
		_thumbnailCacheOptions = thumbnailCacheOptions;
		_fileSizeEngine = fileSizeEngine;

		_imageFileContentLogic = imageFileContentLogic;
		_cachedImageFileContentLogic = cachedImageFileContentLogic;

		if (_thumbnailCacheOptions.EnableThumbnailCaching)
		{
			_activeImageFileContentLogic = cachedImageFileContentLogic;
		}
		else
		{
			_activeImageFileContentLogic = imageFileContentLogic;
		}
	}

	public void EnableThumbnailCaching()
	{
		_activeImageFileContentLogic = _cachedImageFileContentLogic;
	}

	public void DisableThumbnailCaching()
	{
		_activeImageFileContentLogic = _imageFileContentLogic;
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
	private readonly IThumbnailCacheOptions _thumbnailCacheOptions;
	private readonly IFileSizeEngine _fileSizeEngine;

	private readonly IImageFileContentLogic _imageFileContentLogic;
	private readonly IImageFileContentLogic _cachedImageFileContentLogic;

	private IImageFileContentLogic _activeImageFileContentLogic;
}
