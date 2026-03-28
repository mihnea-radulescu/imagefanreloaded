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
		IImageFileContentLogic cachingEnabledImageFileContentLogic)
	{
		_globalParameters = globalParameters;

		_imageResizer = imageResizer;
		_fileSizeEngine = fileSizeEngine;

		_imageFileContentLogic = imageFileContentLogic;
		_cachingEnabledImageFileContentLogic =
			cachingEnabledImageFileContentLogic;

		_activeImageFileContentLogic =
			thumbnailCacheOptions.EnableThumbnailCaching
				? cachingEnabledImageFileContentLogic
				: imageFileContentLogic;
	}

	public void EnableThumbnailCaching()
	{
		_activeImageFileContentLogic = _cachingEnabledImageFileContentLogic;
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
	private readonly IFileSizeEngine _fileSizeEngine;

	private readonly IImageFileContentLogic _imageFileContentLogic;
	private readonly IImageFileContentLogic _cachingEnabledImageFileContentLogic;

	private IImageFileContentLogic _activeImageFileContentLogic;
}
