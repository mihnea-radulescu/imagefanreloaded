using System.IO;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFileContentLogic
	: ImageFileContentLogicBase, IImageFileContentLogic
{
	public ImageFileContentLogic(IGlobalParameters globalParameters)
		: base(globalParameters)
	{
	}

	public override ImageData GetImageData(ImageFileData imageFileData)
	{
		var imageFilePath = imageFileData.FilePath;

		if (!File.Exists(imageFilePath))
		{
			return new ImageData(null);
		}

		try
		{
			var imageFileContent = File.ReadAllBytes(imageFilePath);

			var imageFileContentStream = new MemoryStream(imageFileContent);
			imageFileContentStream.Reset();

			return new ImageData(imageFileContentStream);
		}
		catch
		{
			return new ImageData(null);
		}
	}

	public override ImageData GetImageData(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation)
			=> GetImageData(imageFileData);

	public override void UpdateThumbnail(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
	}
}
