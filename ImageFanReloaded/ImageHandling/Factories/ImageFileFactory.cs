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
		IFileSizeEngine fileSizeEngine)
	{
		_globalParameters = globalParameters;
		_imageResizer = imageResizer;
		_fileSizeEngine = fileSizeEngine;
	}

	public IImageFile GetImageFile(
		StaticImageFileData staticImageFileData, TransientImageFileData transientImageFileData)
		=> new ImageFile(
			_globalParameters,
			_imageResizer,
			_fileSizeEngine,
			staticImageFileData,
			transientImageFileData);

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;
	private readonly IFileSizeEngine _fileSizeEngine;

	#endregion
}
