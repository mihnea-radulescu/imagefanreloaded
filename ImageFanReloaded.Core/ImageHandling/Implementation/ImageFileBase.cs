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
		StaticImageFileData staticImageFileData,
		TransientImageFileData transientImageFileData)
	{
		_globalParameters = globalParameters;
		_imageResizer = imageResizer;
		_fileSizeEngine = fileSizeEngine;

		StaticImageFileData = staticImageFileData;
		TransientImageFileData = transientImageFileData;

		ImageSize = _globalParameters.InvalidImage.Size;

		_thumbnailGenerationLockObject = new object();
	}

	public StaticImageFileData StaticImageFileData { get; }
	public TransientImageFileData TransientImageFileData { get; private set; }

	public ImageSize ImageSize { get; private set; }
	public bool IsAnimatedImage { get; private set; }
	public TimeSpan AnimatedImageSlideshowDelay { get; private set; }

	public bool HasImageReadError { get; private set; }

	public IImage GetImage(bool applyImageOrientation)
	{
		IImage image = default!;

		try
		{
			image = GetImageFromDisc(applyImageOrientation);
			HasImageReadError = false;
		}
		catch
		{
			image = _globalParameters.InvalidImage;
			HasImageReadError = true;
		}
		finally
		{
			SetImageProperties(image);
		}

		return image;
	}

	public (IImage, IImage) GetImageAndResizedImage(
		ImageSize viewPortSize, bool applyImageOrientation)
	{
		IImage image = GetImage(applyImageOrientation);
		IImage resizedImage;

		if (HasImageReadError)
		{
			resizedImage = image;
		}
		else
		{
			try
			{
				resizedImage = _imageResizer.CreateResizedImage(
					image, viewPortSize, ImageQuality.High);
			}
			catch
			{
				resizedImage = _globalParameters.InvalidImage;
				HasImageReadError = true;
			}
		}

		return (image, resizedImage);
	}

	public void ReadImageDataFromDisc(bool applyImageOrientation)
	{
		var imageData = GetImage(applyImageOrientation);

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

	public void RefreshTransientImageFileData()
	{
		try
		{
			var imageFileInfo = new FileInfo(StaticImageFileData.ImageFilePath);

			if (!imageFileInfo.Exists)
			{
				InitializeNonExistingImageData();
			}
			else
			{
				var sizeOnDiscInKilobytes = _fileSizeEngine.ConvertToKilobytes(imageFileInfo.Length);
				var lastModificationTime = imageFileInfo.LastWriteTime;

				TransientImageFileData = new TransientImageFileData(
					sizeOnDiscInKilobytes, lastModificationTime);
			}
		}
		catch
		{
			InitializeNonExistingImageData();
		}
	}

	public string GetBasicImageInfo(bool longFormat)
	{
		var imageFileInfo = longFormat
			? StaticImageFileData.ImageFilePath
			: StaticImageFileData.ImageFileName;

		var sizeOnDiscInKilobytes = TransientImageFileData.SizeOnDiscInKilobytes.GetValueOrDefault();
		var sizeOnDiscInKilobytesForDisplay = decimal.Round(
			sizeOnDiscInKilobytes, _globalParameters.DecimalDigitCountForDisplay);

		var imageInfo = HasImageReadError
			? $"{imageFileInfo} - image read error - {sizeOnDiscInKilobytesForDisplay} KB"
			: $"{imageFileInfo} - {ImageSize} - {sizeOnDiscInKilobytesForDisplay} KB";

		return imageInfo;
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

	#region Protected

	protected readonly IGlobalParameters _globalParameters;

	protected abstract IImage GetImageFromDisc(bool applyImageOrientation);

	protected bool IsDirectlySupportedImageFileExtension
		=> _globalParameters.DirectlySupportedImageFileExtensions.Contains(
			StaticImageFileData.ImageFileExtension);

	protected bool IsAnimationEnabledImageFileExtension
		=> _globalParameters.AnimationEnabledImageFileExtensions.Contains(
			StaticImageFileData.ImageFileExtension);

	protected void InitializeNonExistingImageData()
	{
		HasImageReadError = true;

		TransientImageFileData = new TransientImageFileData(default, default);
	}

	#endregion

	#region Private

	private readonly IImageResizer _imageResizer;
	private readonly IFileSizeEngine _fileSizeEngine;

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
