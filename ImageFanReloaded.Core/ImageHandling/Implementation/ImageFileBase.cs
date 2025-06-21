using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageFileBase : IImageFile
{
	static ImageFileBase()
	{
		ExifEnabledImageFormats = new HashSet<string>(
			StringComparer.InvariantCultureIgnoreCase)
		{
			".jpe", ".jpeg", ".jpg",
			".png",
			".tif", ".tiff",
			".webp"
		};
	}

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

	public bool HasReadImageError { get; private set; }

	public IImage GetImage(bool applyImageOrientation)
	{
		IImage image;

		try
		{
			image = GetImageFromDisc(applyImageOrientation);
			ImageSize = image.Size;
		}
		catch
		{
			ImageSize = _globalParameters.InvalidImage.Size;
			image = _globalParameters.InvalidImage;

			HasReadImageError = true;
		}

		return image;
	}

	public IImage GetResizedImage(ImageSize viewPortSize, bool applyImageOrientation)
	{
		IImage? image = null;
		IImage resizedImage;

		try
		{
			image = GetImageFromDisc(applyImageOrientation);
			ImageSize = image.Size;
			
			resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize, ImageQuality.High);
		}
		catch
		{
			ImageSize = _globalParameters.InvalidImage.Size;
			resizedImage = _globalParameters.InvalidImage;
			
			HasReadImageError = true;
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
			ImageSize = imageData.Size;
		}
		catch
		{
			ImageSize = _globalParameters.InvalidImage.Size;
			imageData = _globalParameters.InvalidImage;

			HasReadImageError = true;
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
				_imageInstance = null;
			}
		}
	}

	public string GetImageInfo(bool longFormat)
	{
		var imageFileInfo = longFormat ? ImageFileData.ImageFilePath : ImageFileData.ImageFileName;
		
		var imageInfo = HasReadImageError
			? $"{imageFileInfo} - invalid image"
			: $"{imageFileInfo} - {ImageSize} - {ImageFileData.SizeOnDiscInKilobytes} KB";
				
		return imageInfo;
	}

	#region Protected

	protected abstract IImage GetImageFromDisc(bool applyImageOrientation);

	protected bool IsExifEnabledImageFormat
		=> ExifEnabledImageFormats.Contains(ImageFileData.ImageFileExtension);

	protected bool IsAvaloniaSupportedImageFileExtension
		=> _globalParameters.DirectlySupportedImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	#endregion

	#region Private

	private static readonly HashSet<string> ExifEnabledImageFormats;

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;
	private readonly object _thumbnailGenerationLockObject;

	private IImage? _imageInstance;

	#endregion
}
