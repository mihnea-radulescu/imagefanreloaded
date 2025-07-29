using System;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageFileBase : IImageFile
{
	protected ImageFileBase(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		ImageFileData imageFileData)
	{
		_globalParameters = globalParameters;
		_imageResizer = imageResizer;

		ImageFileData = imageFileData;

		ImageSize = _globalParameters.InvalidImage.Size;

		_thumbnailGenerationLockObject = new object();
	}

	public ImageFileData ImageFileData { get; }

	public ImageSize ImageSize { get; private set; }
	public bool IsAnimatedImage { get; private set; }
	public TimeSpan AnimatedImageSlideshowDelay { get; private set; }

	public bool HasImageReadError { get; private set; }

	public IImage GetImage(bool applyImageOrientation)
	{
		IImage image;

		try
		{
			image = GetImageFromDisc(applyImageOrientation);

			SetImageProperties(image);
		}
		catch
		{
			SetImageProperties(_globalParameters.InvalidImage);

			image = _globalParameters.InvalidImage;

			HasImageReadError = true;
		}

		return image;
	}

	public IImage GetResizedImage(ImageSize viewPortSize, bool applyImageOrientation)
	{
		IImage? image = default;
		IImage resizedImage;

		try
		{
			image = GetImageFromDisc(applyImageOrientation);

			SetImageProperties(image);

			resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize, ImageQuality.High);
		}
		catch
		{
			SetImageProperties(_globalParameters.InvalidImage);

			resizedImage = _globalParameters.InvalidImage;

			HasImageReadError = true;
		}
		finally
		{
			image?.Dispose();
		}

		return resizedImage;
	}

	public void ReadImageDataFromDisc(bool applyImageOrientation)
	{
		IImage imageData;

		try
		{
			imageData = GetImageFromDisc(applyImageOrientation);

			SetImageProperties(imageData);
		}
		catch
		{
			SetImageProperties(_globalParameters.InvalidImage);

			imageData = _globalParameters.InvalidImage;

			HasImageReadError = true;
		}

		lock (_thumbnailGenerationLockObject)
		{
			_imageInstance = imageData;
		}
	}

	public IImage GetThumbnail(int thumbnailSize)
	{
		lock (_thumbnailGenerationLockObject)
		{
			IImage thumbnail;

			try
			{
				var thumbnailImageSize = new ImageSize(thumbnailSize);

				thumbnail = _imageResizer.CreateResizedImage(
					_imageInstance!, thumbnailImageSize, ImageQuality.Medium);
			}
			catch
			{
				thumbnail = _globalParameters.GetInvalidImageThumbnail(thumbnailSize);
			}
			finally
			{
				DisposeImageData();
			}

			return thumbnail;
		}
	}

	public void DisposeImageData()
	{
		lock (_thumbnailGenerationLockObject)
		{
			if (_imageInstance is not null &&
				_globalParameters.CanDisposeImage(_imageInstance))
			{
				_imageInstance.Dispose();
				_imageInstance = default;
			}
		}
	}

	public string GetBasicImageInfo(bool longFormat)
	{
		var imageFileInfo = longFormat ? ImageFileData.ImageFilePath : ImageFileData.ImageFileName;

		var imageInfo = HasImageReadError
			? $"{imageFileInfo} - image read error - {ImageFileData.SizeOnDiscInKilobytes} KB"
			: $"{imageFileInfo} - {ImageSize} - {ImageFileData.SizeOnDiscInKilobytes} KB";

		return imageInfo;
	}

	#region Protected

	protected readonly IGlobalParameters _globalParameters;

	protected abstract IImage GetImageFromDisc(bool applyImageOrientation);

	protected bool IsDirectlySupportedImageFileExtension
		=> _globalParameters.DirectlySupportedImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	protected bool IsAnimationEnabledImageFileExtension
		=> _globalParameters.AnimationEnabledImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	#endregion

	#region Private

	private readonly IImageResizer _imageResizer;
	private readonly object _thumbnailGenerationLockObject;

	private IImage? _imageInstance;
	
	private void SetImageProperties(IImage image)
	{
		ImageSize = image.Size;
		IsAnimatedImage = image.IsAnimated;
		AnimatedImageSlideshowDelay = image.TotalImageFramesDelay;
	}

	#endregion
}
