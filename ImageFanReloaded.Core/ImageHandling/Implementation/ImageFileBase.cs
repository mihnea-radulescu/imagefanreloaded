using System;
using System.IO;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageFileBase : IImageFile
{
	protected ImageFileBase(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		IFileSizeEngine fileSizeEngine,
		ImageFileData imageFileData)
	{
		_globalParameters = globalParameters;
		_imageResizer = imageResizer;
		_fileSizeEngine = fileSizeEngine;

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

		var imageFileContentStream = GetImageFileContentStream();

		if (HasImageReadError)
		{
			image = _globalParameters.InvalidImage;
		}
		else
		{
			try
			{
				image = GetImageFromStream(imageFileContentStream!, applyImageOrientation);

				SetImageProperties(image);
			}
			catch
			{
				image = _globalParameters.InvalidImage;

				HasImageReadError = true;
			}
		}

		imageFileContentStream?.Dispose();

		return image;
	}

	public (IImage, IImage) GetImageAndResizedImage(
		ImageSize viewPortSize, bool applyImageOrientation)
	{
		IImage image = GetImage(applyImageOrientation);
		IImage resizedImage;

		if (HasImageReadError)
		{
			resizedImage = _globalParameters.InvalidImage;
		}
		else
		{
			try
			{
				resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);
			}
			catch
			{
				resizedImage = _globalParameters.InvalidImage;

				HasImageReadError = true;
			}
		}

		return (image, resizedImage);
	}

	public void ReadImageFile()
	{
		var imageFileContentStream = GetImageFileContentStream();

		if (!HasImageReadError)
		{
			lock (_thumbnailGenerationLockObject)
			{
				_imageFileContentStream = imageFileContentStream;
			}
		}
	}

	public IImage GetThumbnail(int thumbnailSize, bool applyImageOrientation)
	{
		IImage? image = default;
		IImage thumbnail;

		if (HasImageReadError)
		{
			thumbnail = _globalParameters.GetInvalidImageThumbnail(thumbnailSize);
		}
		else
		{
			try
			{
				lock (_thumbnailGenerationLockObject)
				{
					image = GetImageFromStream(_imageFileContentStream!, applyImageOrientation);
				}

				SetImageProperties(image);

				var thumbnailImageSize = new ImageSize(thumbnailSize);

				thumbnail = _imageResizer.CreateResizedImage(image, thumbnailImageSize);
			}
			catch
			{
				thumbnail = _globalParameters.GetInvalidImageThumbnail(thumbnailSize);

				HasImageReadError = true;
			}
			finally
			{
				DisposeImage(image);
				DisposeImageFileContentStream();
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
				var sizeOnDiscInKilobytes = _fileSizeEngine.ConvertToKilobytes(imageFileInfo.Length);
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
			sizeOnDiscInKilobytes, _globalParameters.DecimalDigitCountForDisplay);

		var imageInfo = HasImageReadError
			? $"{imageFileInfo} - image read error - {sizeOnDiscInKilobytesForDisplay} KB"
			: $"{imageFileInfo} - {ImageSize} - {sizeOnDiscInKilobytesForDisplay} KB";

		return imageInfo;
	}

	public void DisposeImageFileContentStream()
	{
		lock (_thumbnailGenerationLockObject)
		{
			if (_imageFileContentStream is not null)
			{
				_imageFileContentStream.Dispose();
				_imageFileContentStream = null;
			}
		}
	}

	#region Protected

	protected readonly IGlobalParameters _globalParameters;

	protected abstract IImage GetImageFromStream(
		Stream imageFileContentStream, bool applyImageOrientation);

	protected bool IsDirectlySupportedImageFileExtension
		=> _globalParameters.DirectlySupportedImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	protected bool IsAnimationEnabledImageFileExtension
		=> _globalParameters.AnimationEnabledImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	protected Stream? GetImageFileContentStream()
	{
		var imageFilePath = ImageFileData.ImageFilePath;

		if (!File.Exists(imageFilePath))
		{
			HasImageReadError = true;

			return default;
		}

		try
		{
			var imageFileContent = File.ReadAllBytes(imageFilePath);

			var imageFileContentStream = new MemoryStream(imageFileContent);
			imageFileContentStream.Reset();

			return imageFileContentStream;
		}
		catch
		{
			HasImageReadError = true;

			return default;
		}
	}

	#endregion

	#region Private

	private readonly IImageResizer _imageResizer;
	private readonly IFileSizeEngine _fileSizeEngine;

	private readonly object _thumbnailGenerationLockObject;

	private Stream? _imageFileContentStream;

	private void SetImageProperties(IImage image)
	{
		ImageSize = image.Size;
		IsAnimatedImage = image.IsAnimated;
		AnimatedImageSlideshowDelay = image.TotalImageFramesDelay;
	}

	private void DisposeImage(IImage? image)
	{
		if (image is not null && _globalParameters.CanDisposeImage(image))
		{
			image.Dispose();
		}
	}

	#endregion
}
