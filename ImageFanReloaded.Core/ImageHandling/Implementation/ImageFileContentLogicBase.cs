using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageFileContentLogicBase : IImageFileContentLogic
{
	public abstract ImageData GetImageData(ImageFileData imageFileData);

	public abstract ImageData GetImageData(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation);

	public abstract void UpdateThumbnail(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail);

	protected ImageFileContentLogicBase(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	protected bool GetNormalizedApplyImageOrientation(
		ImageFileData imageFileData, bool applyImageOrientation)
			=> applyImageOrientation &&
			   _globalParameters.ExifEnabledImageFileExtensions
				   .Contains(imageFileData.FileExtension);

	private readonly IGlobalParameters _globalParameters;
}
