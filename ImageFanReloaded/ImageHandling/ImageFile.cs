using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using ImageMagick;
using ImageFanReloaded.Core.DiscAccess;
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
		IFileSizeEngine fileSizeEngine,
		StaticImageFileData staticImageFileData,
		TransientImageFileData transientImageFileData)
		: base(
			globalParameters,
			imageResizer,
			fileSizeEngine,
			staticImageFileData,
			transientImageFileData)
	{
	}

	#region Protected

	protected override IImage GetImageFromDisc(bool applyImageOrientation)
	{
		var imageFilePath = StaticImageFileData.ImageFilePath;
		if (!File.Exists(imageFilePath))
		{
			InitializeNonExistingImageData();

			throw new FileNotFoundException($"Image '{imageFilePath}' not found.", imageFilePath);
		}

		if (IsAnimationEnabledImageFileExtension)
		{
			return BuildAnimatedImageFromFile(imageFilePath);
		}
		else if (applyImageOrientation)
		{
			return BuildIndirectlySupportedImageFromFile(imageFilePath);
		}
		else if (IsDirectlySupportedImageFileExtension)
		{
			return BuildImageFromFile(imageFilePath);
		}
		else
		{
			return BuildIndirectlySupportedImageFromFile(imageFilePath);
		}
	}

	#endregion

	#region Private

	private IImage BuildAnimatedImageFromFile(string inputFilePath)
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

	private IImage BuildAnimatedImage(MagickImageCollection imageCollection)
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

	private IImage BuildIndirectlySupportedImageFromFile(string inputFilePath)
	{
		IMagickImage image = new MagickImage(inputFilePath);

		return BuildIndirectlySupportedImage(image);
	}

	private IImage BuildIndirectlySupportedImage(IMagickImage image)
	{
		using var imageStream = new MemoryStream();
		WriteImageToStream(image, imageStream, true);

		return BuildImageFromStream(imageStream);
	}

	private void WriteImageToStream(IMagickImage image, Stream imageStream, bool autoOrientImage)
	{
		image.Quality = _globalParameters.ImageQualityLevel;

		if (autoOrientImage)
		{
			image.AutoOrient();
		}

		image.Write(imageStream, MagickFormat.Jpg);
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

	#endregion
}
