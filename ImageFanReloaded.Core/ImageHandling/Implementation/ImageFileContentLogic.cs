using System.IO;
using ImageFanReloaded.Core.DiscAccess.Implementation;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFileContentLogic : IImageFileContentLogic
{
	public ImageData GetImageData(
		string imageFilePath, bool applyImageOrientation)
	{
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
		string imageFilePath, int thumbnailSize, bool applyImageOrientation)
			=> GetImageData(imageFilePath, applyImageOrientation);

	public void UpdateThumbnail(
		string imageFilePath,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
	}
}
