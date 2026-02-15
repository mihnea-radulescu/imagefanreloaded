using System.IO;
using ImageFanReloaded.Core.DiscAccess.Implementation;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFileContentLogic : IImageFileContentLogic
{
	public ImageData GetImageData(
		ImageFileData imageFileData, bool applyImageOrientation)
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

	public ImageData GetImageData(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation)
			=> GetImageData(imageFileData, applyImageOrientation);

	public void UpdateThumbnail(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
	}
}
