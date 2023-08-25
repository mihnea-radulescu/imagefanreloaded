using System;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

public class ImageFile
    : IImageFile
{
    public ImageFile(IImageResizer imageResizer, string imageFilePath)
    {
        _imageResizer = imageResizer;
        _imageFilePath = imageFilePath;

        FileName = Path.GetFileName(imageFilePath);

		ImageSize = GlobalData.InvalidImageSize;

		_thumbnailGenerationLockObject = new object();
    }

    public string FileName { get; }

	public ImageSize ImageSize { get; private set; }

	public IImage GetImage()
    {
        IImage image;

        try
        {
            image = new Bitmap(_imageFilePath);
			ImageSize = new ImageSize(image.Size.Width, image.Size.Height);
		}
        catch
        {
            image = GlobalData.InvalidImage;
			ImageSize = GlobalData.InvalidImageSize;
		}

        return image;
    }

    public IImage GetResizedImage(ImageSize viewPortSize)
    {
        IImage resizedImage;

        try
		{
			var image = new Bitmap(_imageFilePath);
			ImageSize = new ImageSize(image.Size.Width, image.Size.Height);

			resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);
		}
		catch
        {
            resizedImage = GlobalData.InvalidImage;
			ImageSize = GlobalData.InvalidImageSize;
		}

        return resizedImage;
    }

	public void ReadImageDataFromDisc()
    {
        lock (_thumbnailGenerationLockObject)
        {
            try
            {
                _imageData = new Bitmap(_imageFilePath);
				ImageSize = new ImageSize(_imageData.Size.Width, _imageData.Size.Height);
			}
            catch
            {
                _imageData = GlobalData.InvalidImage;
            }
        }
    }

    public IImage GetThumbnail()
    {
        lock (_thumbnailGenerationLockObject)
        {
            if (_imageData == null)
            {
                throw new InvalidOperationException(
                    $"The method {nameof(ReadImageDataFromDisc)} must be executed prior to calling the method {nameof(GetThumbnail)}.");
            }

            IImage thumbnail;

            try
            {
                thumbnail = _imageResizer
                    .CreateResizedImage(_imageData, GlobalData.ThumbnailSize);
            }
            catch
            {
                thumbnail = GlobalData.InvalidImageThumbnail;
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
        _imageData = null;
	}

	#region Private

	private readonly IImageResizer _imageResizer;
    private readonly string _imageFilePath;
    private readonly object _thumbnailGenerationLockObject;

    private IImage _imageData;

	#endregion
}
