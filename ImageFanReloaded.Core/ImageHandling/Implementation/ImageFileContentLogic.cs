using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFileContentLogic
	: ImageFileContentLogicBase, IImageFileContentLogic
{
	public ImageFileContentLogic(IGlobalParameters globalParameters)
		: base(globalParameters)
	{
	}

	public override ImageData GetImageData(IImageFileData imageFileData)
		=> imageFileData.GetImageData();

	public override ImageData GetImageData(
		IImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation)
			=> GetImageData(imageFileData);

	public override void UpdateThumbnail(
		IImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
	}
}
