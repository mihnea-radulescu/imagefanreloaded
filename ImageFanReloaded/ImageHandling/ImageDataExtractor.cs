using System.IO;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageDataExtractor : IImageDataExtractor
{
	public ImageDataExtractor(IGlobalParameters globalParameters)
	{
		_bitmapEncoderOptions = new JpegBitmapEncoderOptions
		{
			Quality = globalParameters.ImageQualityLevel
		};
	}

	public byte[] GetImageData(IImage image)
	{
		var bitmap = image.GetInstance<Bitmap>();

		using (var imageDataStream = new MemoryStream())
		{
			bitmap.Save(imageDataStream, _bitmapEncoderOptions);

			var imageData = imageDataStream.ToArray();
			return imageData;
		}
	}

	private readonly BitmapEncoderOptions _bitmapEncoderOptions;
}
