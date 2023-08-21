using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

public class ImageEncoderFactory
	: IImageEncoderFactory
{
	static ImageEncoderFactory()
	{
		JpegEncoder = new JpegEncoder();
		PngEncoder = new PngEncoder();
	}
	
	public IImageEncoder GetImageEncoder(ImageFormat imageFormat)
	{
		switch (imageFormat)
		{
			case ImageFormat.Jpeg:
				return JpegEncoder;

			case ImageFormat.Png:
				return PngEncoder;

			default:
				return JpegEncoder;
		}
	}

	#region Private

	private static readonly IImageEncoder JpegEncoder;
	private static readonly IImageEncoder PngEncoder;

	#endregion
}
