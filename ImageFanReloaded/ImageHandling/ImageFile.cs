using System.IO;
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
		ImageFileData imageFileData,
		IImageOrientationHandler imageOrientationHandler)
		: base(globalParameters, imageResizer, imageFileData)
	{
		_imageOrientationHandler = imageOrientationHandler;
	}

	#region Protected

	protected override IImage GetImageFromDisc(bool applyImageOrientation)
	{
		if (applyImageOrientation && IsExifEnabledImageFormat)
		{
			try
			{
				return BuildAndTransformImage(ImageFileData.ImageFilePath, true);
			}
			catch
			{
				return BuildImageFromFile(ImageFileData.ImageFilePath);
			}
		}
		else if (IsAvaloniaSupportedImageFileExtension)
		{
			return BuildImageFromFile(ImageFileData.ImageFilePath);
		}
		else
		{
			return BuildAndTransformImage(ImageFileData.ImageFilePath, false);
		}
	}

	#endregion

	#region Private

	private readonly IImageOrientationHandler _imageOrientationHandler;

	private IImage BuildAndTransformImage(string inputFilePath, bool applyImageOrientation)
	{
		var image = SixLabors.ImageSharp.Image.Load(inputFilePath);

		if (applyImageOrientation)
		{
			_imageOrientationHandler.ApplyImageOrientation(image);
		}

		using var imageStream = new MemoryStream();
		SixLabors.ImageSharp.ImageExtensions.SaveAsJpeg(image, imageStream);

		return BuildImageFromStream(imageStream);
	}

	private static Image BuildImageFromFile(string inputFilePath)
	{
		var bitmap = new Avalonia.Media.Imaging.Bitmap(inputFilePath);

		return BuildImage(bitmap);
	}

	private static Image BuildImageFromStream(Stream inputStream)
	{
		inputStream.Reset();
		var bitmap = new Avalonia.Media.Imaging.Bitmap(inputStream);

		return BuildImage(bitmap);
	}

	private static Image BuildImage(Avalonia.Media.Imaging.Bitmap bitmap)
	{
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		return new Image(bitmap, bitmapSize);
	}

	#endregion
}
