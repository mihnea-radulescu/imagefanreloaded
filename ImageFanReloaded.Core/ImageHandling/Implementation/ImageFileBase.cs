using System;
using System.IO;
using ImageFanReloaded.Core.DiscAccess;
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

		_thumbnailGenerationLockObject = new object();
	}

	public ImageFileData ImageFileData { get; }

	public ImageSize ImageSize { get; private set; }
	public bool IsAnimatedImage { get; private set; }
	public TimeSpan AnimatedImageSlideshowDelay { get; private set; }

	public bool HasImageReadError { get; private set; }

	public IImage GetImage(bool applyImageOrientation)
	{
		using var imageData = _imageFileContentLogic.GetImageData(
			ImageFileData.ImageFilePath, applyImageOrientation);

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
					imageData.ImageDataStream!, applyImageOrientation);

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
		UpsizeFullScreenImagesUpToScreenSize
			upsizeFullScreenImagesUpToScreenSize,
		bool applyImageOrientation)
	{
		var image = GetImage(applyImageOrientation);

		IImage resizedImage;

		if (HasImageReadError)
		{
			resizedImage = GlobalParameters.InvalidImage;
		}
		else
		{
			var doesFitWithinViewPort = image.DoesFitWithinViewPort(
				viewPortSize);
			if (doesFitWithinViewPort)
			{
				if (upsizeFullScreenImagesUpToScreenSize ==
					UpsizeFullScreenImagesUpToScreenSize.Disabled)
				{
					resizedImage = image;
				}
				else
				{
					var maxUpscalingFactorToViewPort = image
						.GetMaxUpscalingFactorToViewPort(viewPortSize);
					var upscalingFactor = Math.Min(
						maxUpscalingFactorToViewPort,
						upsizeFullScreenImagesUpToScreenSize.Value);

					try
					{
						resizedImage = _imageResizer.CreateUpsizedImage(
							image, upscalingFactor);

						image.Dispose();
						image = resizedImage;
					}
					catch
					{
						resizedImage = GlobalParameters.InvalidImage;

						HasImageReadError = true;
					}
				}
			}
			else
			{
				try
				{
					resizedImage = _imageResizer.CreateDownsizedImage(
						image, viewPortSize);
				}
				catch
				{
					resizedImage = GlobalParameters.InvalidImage;

					HasImageReadError = true;
				}
			}
		}

		return (image, resizedImage);
	}

	public void ReadImageFile(int thumbnailSize, bool applyImageOrientation)
	{
		var imageData = _imageFileContentLogic.GetImageData(
			ImageFileData.ImageFilePath, thumbnailSize, applyImageOrientation);

		if (imageData.ImageDataStream is null)
		{
			HasImageReadError = true;
		}

		if (!HasImageReadError)
		{
			lock (_thumbnailGenerationLockObject)
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
				lock (_thumbnailGenerationLockObject)
				{
					image = GetImageFromStream(
						_imageData!.ImageDataStream!, applyImageOrientation);
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
						ImageFileData.ImageFilePath,
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
			var imageFileInfo = new FileInfo(ImageFileData.ImageFilePath);

			if (!imageFileInfo.Exists)
			{
				HasImageReadError = true;
			}
			else
			{
				var sizeOnDiscInKilobytes = _fileSizeEngine.ConvertToKilobytes(
					imageFileInfo.Length);
				var lastModificationTime = imageFileInfo.LastWriteTime;

				ImageFileData.SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
				ImageFileData.LastModificationTime = lastModificationTime;
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
			? ImageFileData.ImageFilePath
			: ImageFileData.ImageFileName;

		var sizeOnDiscInKilobytes = ImageFileData.SizeOnDiscInKilobytes;
		var sizeOnDiscInKilobytesForDisplay = decimal.Round(
			sizeOnDiscInKilobytes,
			GlobalParameters.DecimalDigitCountForDisplay);

		var imageInfo = HasImageReadError
			? $"{imageFileInfo} - image read error - {sizeOnDiscInKilobytesForDisplay} KB"
			: $"{imageFileInfo} - {ImageSize} - {sizeOnDiscInKilobytesForDisplay} KB";

		return imageInfo;
	}

	public void DisposeImageData()
	{
		lock (_thumbnailGenerationLockObject)
		{
			_imageData?.Dispose();
			_imageData = null;
		}
	}

	protected readonly IGlobalParameters GlobalParameters;

	protected abstract IImage GetImageFromStream(
		Stream imageFileContentStream, bool applyImageOrientation);

	protected bool IsDirectlySupportedImageFileExtension
		=> GlobalParameters.DirectlySupportedImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	protected bool IsAnimationEnabledImageFileExtension
		=> GlobalParameters.AnimationEnabledImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	private readonly IImageResizer _imageResizer;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IImageFileContentLogic _imageFileContentLogic;

	private readonly object _thumbnailGenerationLockObject;

	private ImageData? _imageData;

	private void SetImageProperties(IImage image)
	{
		ImageSize = image.Size;
		IsAnimatedImage = image.IsAnimated;
		AnimatedImageSlideshowDelay = image.TotalImageFramesDelay;
	}

	private bool ShouldUpdateThumbnail(IImage? thumbnail)
		=> _imageData?.ShouldUpdateThumbnail == true && !thumbnail!.IsAnimated;

	private void DisposeImage(IImage? image)
	{
		if (image is not null && GlobalParameters.CanDisposeImage(image))
		{
			image.Dispose();
		}
	}
}
