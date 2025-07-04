using System;
using System.Collections.Generic;
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
		if (IsAnimationEnabledImageFileExtension)
		{
			return BuildAnimatedImageFromFile(ImageFileData.ImageFilePath);
		}
		else if (applyImageOrientation)
		{
			return BuildIndirectlySupportedImageFromFile(ImageFileData.ImageFilePath);
		}
		else if (IsDirectlySupportedImageFileExtension)
		{
			return BuildImageFromFile(ImageFileData.ImageFilePath);
		}
		else
		{
			return BuildIndirectlySupportedImageFromFile(ImageFileData.ImageFilePath);
		}
	}

	#endregion

	#region Private

	private const uint ImageQualityLevel = 80;

	private static IImage BuildAnimatedImageFromFile(string inputFilePath)
	{
		var imageCollection = new MagickImageCollection(inputFilePath);

		if (imageCollection.Count == 1)
		{
			return BuildIndirectlySupportedImage(imageCollection[0]);
		}
		else
		{
			return BuildAnimatedImage(imageCollection);
		}
	}

	private static IImage BuildAnimatedImage(MagickImageCollection imageCollection)
	{
		var animatedImageFrames = new List<IImageFrame>();

		imageCollection.Coalesce();

		foreach (var image in imageCollection)
		{
			var animationDelayInMilliseconds = image.AnimationDelay * 10;
			var delayUntilNextFrame = TimeSpan.FromMilliseconds(animationDelayInMilliseconds);

			using var imageStream = new MemoryStream();
			WriteImageToStream(image, imageStream, false);

			var imageFrame = BuildImageFrameFromStream(imageStream, delayUntilNextFrame);
			animatedImageFrames.Add(imageFrame);
		}

		return new Image(animatedImageFrames);
	}

	private static IImage BuildIndirectlySupportedImageFromFile(string inputFilePath)
	{
		IMagickImage image = new MagickImage(inputFilePath);

		return BuildIndirectlySupportedImage(image);
	}

	private static IImage BuildIndirectlySupportedImage(IMagickImage image)
	{
		using var imageStream = new MemoryStream();
		WriteImageToStream(image, imageStream, true);

		return BuildImageFromStream(imageStream);
	}

	private static IImage BuildImageFromFile(string inputFilePath)
	{
		var bitmap = new Bitmap(inputFilePath);

		return BuildImage(bitmap);
	}

	private static IImageFrame BuildImageFrameFromStream(
		Stream inputStream, TimeSpan delayUntilNextFrame)
	{
		inputStream.Reset();
		var bitmap = new Bitmap(inputStream);

		return BuildImageFrame(bitmap, delayUntilNextFrame);
	}

	private static IImageFrame BuildImageFrame(Bitmap bitmap, TimeSpan delayUntilNextFrame)
	{
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		return new ImageFrame(bitmap, bitmapSize, delayUntilNextFrame);
	}

	private static IImage BuildImageFromStream(Stream inputStream)
	{
		inputStream.Reset();
		var bitmap = new Bitmap(inputStream);

		return BuildImage(bitmap);
	}

	private static IImage BuildImage(Bitmap bitmap)
	{
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		return new Image(bitmap, bitmapSize);
	}

	private static void WriteImageToStream(
		IMagickImage image, Stream imageStream, bool autoOrientImage)
	{
		image.Format = MagickFormat.Jpg;
		image.Quality = ImageQualityLevel;

		if (autoOrientImage)
		{
			image.AutoOrient();
		}

		image.Write(imageStream);
	}

	#endregion
}
