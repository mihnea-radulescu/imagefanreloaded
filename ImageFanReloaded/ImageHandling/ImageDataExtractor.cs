using System.IO;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageDataExtractor : IImageDataExtractor
{
	public ImageDataExtractor(IGlobalParameters globalParameters)
	{
		_imageQualityLevel = (int)globalParameters.ImageQualityLevel;
	}

	public byte[] GetImageData(IImage image)
	{
		var bitmap = ((Image)image).GetInstance<Bitmap>();

		using (var imageDataStream = new MemoryStream())
		{
			bitmap.Save(imageDataStream, _imageQualityLevel);

			var imageData = imageDataStream.ToArray();
			return imageData;
		}
	}

	private readonly int _imageQualityLevel;
}
