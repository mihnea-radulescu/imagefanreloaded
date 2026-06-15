using System;
using System.IO;
using System.Threading;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageFileBase : IImageFile
{
	protected ImageFileBase(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		IFileSizeEngine fileSizeEngine,
		IImageFileContentLogic imageFileContentLogic,
		ImageFileData imageFileData)
	{
		GlobalParameters = globalParameters;

		_imageResizer = imageResizer;
		_fileSizeEngine = fileSizeEngine;
		_imageFileContentLogic = imageFileContentLogic;

		ImageFileData = imageFileData;
		ImageSize = GlobalParameters.InvalidImage.Size;

		_thumbnailGenerationLock = new Lock();
	}

	public ImageFileData ImageFileData { get; }

	public ImageSize ImageSize { get; private set; }
	public bool IsAnimatedImage { get; private set; }
	public TimeSpan AnimatedImageSlideshowDelay { get; private set; }

	public bool HasImageReadError { get; private set; }

	public IImage GetImage(bool applyImageOrientation)
	{
		using var imageData = _imageFileContentLogic.GetImageData(
			ImageFileData);

		if (imageData.ImageDataStream is null)
		{
			HasImageReadError = true;
		}

		IImage image;

		if (HasImageReadError)
		{
			image = GlobalParameters.InvalidImage;
		}
		else
		{
			try
			{
				image = GetImageFromStream(
					ImageFileData,
					imageData.ImageDataStream!,
					imageData.IsKnownImage,
					applyImageOrientation);

				SetImageProperties(image);
			}
			catch
			{
				image = GlobalParameters.InvalidImage;

				HasImageReadError = true;
			}
		}

		return image;
	}

	public (IImage, IImage) GetImageAndResizedImage(
		ImageSize viewPortSize,
		UpsizeFullScreenImageScalingFactor upsizeFullScreenImageScalingFactor,
		bool applyImageOrientation)
	{
		var image = GetImage(applyImageOrientation);

		if (HasImageReadError)
		{
			return (GlobalParameters.InvalidImage,
					GlobalParameters.InvalidImage);
		}

		try
		{
			var shouldUpsizeImage = !IsAnimatedImage &&
				upsizeFullScreenImageScalingFactor !=
					UpsizeFullScreenImageScalingFactor.Disabled;

			if (shouldUpsizeImage)
			{
				var upsizedImage = _imageResizer.CreateUpsizedImage(
					image, upsizeFullScreenImageScalingFactor.Value);

				image.Dispose();
				image = upsizedImage;
			}

			var doesImageFitWithinViewPort = image.DoesFitWithinViewPort(
				viewPortSize);

			if (doesImageFitWithinViewPort)
			{
				return (image, image);
			}

			var resizedImage = _imageResizer.CreateDownsizedImage(
				image, viewPortSize);

			return (image, resizedImage);
		}
		catch
		{
			return (GlobalParameters.InvalidImage,
					GlobalParameters.InvalidImage);
		}
	}

	public void ReadImageFile(int thumbnailSize, bool applyImageOrientation)
	{
		var imageData = _imageFileContentLogic.GetImageData(
			ImageFileData, thumbnailSize, applyImageOrientation);

		if (imageData.ImageDataStream is null)
		{
			HasImageReadError = true;
		}

		if (!HasImageReadError)
		{
			lock (_thumbnailGenerationLock)
			{
				_imageData = imageData;
			}
		}
	}

	public IImage GetThumbnail(int thumbnailSize, bool applyImageOrientation)
	{
		IImage? image = null;
		IImage? thumbnail = null;

		if (HasImageReadError)
		{
			thumbnail = GlobalParameters.GetInvalidImageThumbnail(
				thumbnailSize);
		}
		else
		{
			try
			{
				lock (_thumbnailGenerationLock)
				{
					image = GetImageFromStream(
						ImageFileData,
						_imageData!.ImageDataStream!,
						_imageData.IsKnownImage,
						applyImageOrientation);
				}

				SetImageProperties(image);

				var thumbnailImageSize = new ImageSize(thumbnailSize);

				if (image.DoesFitWithinViewPort(thumbnailImageSize))
				{
					thumbnail = image;
				}
				else
				{
					thumbnail = _imageResizer.CreateDownsizedImage(
						image, thumbnailImageSize);
				}
			}
			catch
			{
				thumbnail = GlobalParameters.GetInvalidImageThumbnail(
					thumbnailSize);

				HasImageReadError = true;
			}
			finally
			{
				if (ShouldUpdateThumbnail(thumbnail))
				{
					_imageFileContentLogic.UpdateThumbnail(
						ImageFileData,
						thumbnailSize,
						applyImageOrientation,
						thumbnail!);
				}

				if (thumbnail != image)
				{
					DisposeImage(image);
				}

				DisposeImageData();
			}
		}

		return thumbnail;
	}

	public void RefreshImageFileData()
	{
		try
		{
			var imageFileInfo = new FileInfo(ImageFileData.FilePath);

			if (!imageFileInfo.Exists)
			{
				HasImageReadError = true;
			}
			else
			{
				ImageFileData.FileSizeInBytes = (int)imageFileInfo.Length;
				ImageFileData.FileLastModificationTime =
					imageFileInfo.LastWriteTimeUtc;
			}
		}
		catch
		{
			HasImageReadError = true;
		}
	}

	public string GetBasicImageInfo(bool longFormat)
	{
		var imageFileInfo = longFormat
			? ImageFileData.FilePath
			: ImageFileData.FileName;

		var fileSizeInKilobytes =
			_fileSizeEngine.ConvertToKilobytes(ImageFileData.FileSizeInBytes);
		var fileSizeInKilobytesForDisplay = decimal.Round(
			fileSizeInKilobytes, GlobalParameters.DecimalDigitCountForDisplay);

		var imageInfo = HasImageReadError
			? $"{imageFileInfo} - image read error - {fileSizeInKilobytesForDisplay} KB"
			: $"{imageFileInfo} - {ImageSize} - {fileSizeInKilobytesForDisplay} KB";

		return imageInfo;
	}

	public void DisposeImageData()
	{
		lock (_thumbnailGenerationLock)
		{
			_imageData?.Dispose();
			_imageData = null;
		}
	}

	protected readonly IGlobalParameters GlobalParameters;

	protected abstract IImage GetImageFromStream(
		ImageFileData imageFileData,
		Stream imageFileContentStream,
		bool isKnownImage,
		bool applyImageOrientation);

	protected bool IsDirectlySupportedImageFileExtension
		=> GlobalParameters.DirectlySupportedImageFileExtensions.Contains(
			ImageFileData.FileExtension);

	protected bool IsAnimationEnabledImageFileExtension
		=> GlobalParameters.AnimationEnabledImageFileExtensions.Contains(
			ImageFileData.FileExtension);

	protected bool IsExifEnabledImageFileExtension
		=> GlobalParameters.ExifEnabledImageFileExtensions.Contains(
			ImageFileData.FileExtension);

	private readonly IImageResizer _imageResizer;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IImageFileContentLogic _imageFileContentLogic;

	private readonly Lock _thumbnailGenerationLock;

	private ImageData? _imageData;

	private void SetImageProperties(IImage image)
	{
		ImageSize = image.Size;
		IsAnimatedImage = image.IsAnimated;
		AnimatedImageSlideshowDelay = image.TotalImageFramesDelay;
	}

	private bool ShouldUpdateThumbnail(IImage? thumbnail)
		=> _imageData?.IsKnownImage == false && !thumbnail!.IsAnimated;

	private void DisposeImage(IImage? image)
	{
		if (image is not null && GlobalParameters.CanDisposeImage(image))
		{
			image.Dispose();
		}
	}
}
