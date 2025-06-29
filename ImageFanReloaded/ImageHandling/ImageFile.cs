using System.IO;
using Avalonia.Media.Imaging;
using ImageMagick;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageFile : ImageFileBase
{
	public ImageFile(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		ImageFileData imageFileData)
		: base(globalParameters, imageResizer, imageFileData)
	{
	}

	#region Protected

	protected override IImage GetImageFromDisc(bool applyImageOrientation)
	{
		if (applyImageOrientation)
		{
			return BuildIndirectlySupportedImage(ImageFileData.ImageFilePath);
		}
		else if (IsDirectlySupportedImageFileExtension)
		{
			return BuildImageFromFile(ImageFileData.ImageFilePath);
		}
		else
		{
			return BuildIndirectlySupportedImage(ImageFileData.ImageFilePath);
		}
	}

	#endregion

	#region Private

	private const uint ImageQualityLevel = 80; 

	private static IImage BuildIndirectlySupportedImage(string inputFilePath)
	{
		var image = new MagickImage(inputFilePath);

		image.Format = MagickFormat.Jpg;
		image.Quality = ImageQualityLevel;

		image.AutoOrient();

		using var imageStream = new MemoryStream();
		image.Write(imageStream);

		return BuildImageFromStream(imageStream);
	}

	private static Image BuildImageFromFile(string inputFilePath)
	{
		var bitmap = new Bitmap(inputFilePath);

		return BuildImage(bitmap);
	}

	private static Image BuildImageFromStream(Stream inputStream)
	{
		inputStream.Reset();
		var bitmap = new Bitmap(inputStream);

		return BuildImage(bitmap);
	}

	private static Image BuildImage(Bitmap bitmap)
	{
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		return new Image(bitmap, bitmapSize);
	}

	#endregion
}
