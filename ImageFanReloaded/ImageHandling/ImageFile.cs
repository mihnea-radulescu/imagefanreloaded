using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using ImageMagick;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageCore.Implementation;
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
		IImageFileContentLogic imageFileContentLogic,
		ImageFileData imageFileData)
		: base(
			globalParameters,
			imageResizer,
			fileSizeEngine,
			imageFileContentLogic,
			imageFileData)
	{
	}

	protected override IImage GetImageFromStream(
		ImageFileData imageFileData,
		Stream imageFileContentStream,
		bool isKnownImage,
		bool applyImageOrientation)
	{
		if (isKnownImage)
		{
			return BuildDirectlySupportedImageFromStream(
				imageFileContentStream);
		}

		if (IsAnimationEnabledImageFileExtension)
		{
			return BuildAnimatedImageFromStream(imageFileContentStream);
		}

		if (applyImageOrientation && IsExifEnabledImageFileExtension)
		{
			return BuildIndirectlySupportedImageFromStream(
				imageFileData, imageFileContentStream);
		}

		if (IsDirectlySupportedImageFileExtension)
		{
			return BuildDirectlySupportedImageFromStream(
				imageFileContentStream);
		}

		return BuildIndirectlySupportedImageFromStream(
			imageFileData, imageFileContentStream);
	}

	private IImage BuildAnimatedImageFromStream(Stream imageFileContentStream)
	{
		using var imageCollection = new MagickImageCollection(
			imageFileContentStream);

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
			var animationDelayScalingFactor =
				(double)image.AnimationTicksPerSecond / 100;
			var animationDelayInMillisecondsAfterScaling =
				animationDelayInMilliseconds / animationDelayScalingFactor;

			var delayUntilNextFrame = TimeSpan.FromMilliseconds(
				animationDelayInMillisecondsAfterScaling);

			using var imageStream = new MemoryStream();
			WriteImageToStream(image, imageStream, false);

			var imageFrame = BuildImageFrameFromStream(
				imageStream, delayUntilNextFrame);
			animatedImageFrames.Add(imageFrame);
		}

		return new Image(animatedImageFrames);
	}

	private IImage BuildIndirectlySupportedImageFromStream(
		ImageFileData imageFileData,
		Stream imageFileContentStream)
	{
		var magickFormat = GetMagickFormat(imageFileData);
		using IMagickImage image = new MagickImage(
			imageFileContentStream, magickFormat);

		return BuildIndirectlySupportedImage(image);
	}

	private IImage BuildIndirectlySupportedImage(IMagickImage image)
	{
		using var imageStream = new MemoryStream();
		WriteImageToStream(image, imageStream, true);

		return BuildImageFromStream(imageStream);
	}

	private void WriteImageToStream(
		IMagickImage image, Stream imageStream, bool autoOrientImage)
	{
		image.Quality = (uint)GlobalParameters.ImageQualityLevel;

		if (autoOrientImage)
		{
			image.AutoOrient();
		}

		image.Write(imageStream, MagickFormat.Jpg);

		imageStream.Reset();
	}

	private static IImage BuildDirectlySupportedImageFromStream(
		Stream imageFileContentStream)
	{
		var bitmap = new Bitmap(imageFileContentStream);

		return BuildImage(bitmap);
	}

	private static IImageFrame BuildImageFrameFromStream(
		Stream inputStream, TimeSpan delayUntilNextFrame)
	{
		var bitmap = new Bitmap(inputStream);

		return BuildImageFrame(bitmap, delayUntilNextFrame);
	}

	private static IImageFrame BuildImageFrame(
		Bitmap bitmap, TimeSpan delayUntilNextFrame)
	{
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		return new ImageFrame(bitmap, bitmapSize, delayUntilNextFrame);
	}

	private static IImage BuildImageFromStream(Stream inputStream)
	{
		var bitmap = new Bitmap(inputStream);

		return BuildImage(bitmap);
	}

	private static IImage BuildImage(Bitmap bitmap)
	{
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		return new Image(bitmap, bitmapSize);
	}

	private static MagickFormat GetMagickFormat(ImageFileData imageFileData)
	{
		var normalizedFileExtension =
			imageFileData.FileExtension.ToLowerInvariant();

		return normalizedFileExtension switch
		{
			".cur" => MagickFormat.Cur,
			".dng" => MagickFormat.Dng,
			".ico" => MagickFormat.Ico,
			".nrw" => MagickFormat.Nrw,
			".pef" => MagickFormat.Pef,
			".pict" => MagickFormat.Pict,
			".tga" => MagickFormat.Tga,
			".wbmp" => MagickFormat.Wbmp,
			_ => MagickFormat.Unknown
		};
	}
}
