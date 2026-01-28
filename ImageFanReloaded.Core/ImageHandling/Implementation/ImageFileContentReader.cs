using System.IO;
using ImageFanReloaded.Core.DiscAccess.Implementation;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFileContentReader : IImageFileContentReader
{
	public Stream? GetImageFileContentStream(string imageFilePath)
	{
		if (!File.Exists(imageFilePath))
		{
			return null;
		}

		try
		{
			var imageFileContent = File.ReadAllBytes(imageFilePath);

			var imageFileContentStream = new MemoryStream(imageFileContent);
			imageFileContentStream.Reset();

			return imageFileContentStream;
		}
		catch
		{
			return null;
		}
	}
}
