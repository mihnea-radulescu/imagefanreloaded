using System.IO;
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

	public Bitmap GetImage()
    {
        Bitmap image;

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

    public Bitmap GetResizedImage(ImageSize viewPortSize)
    {
        Bitmap image = null;
        Bitmap resizedImage;

        try
		{
			image = new Bitmap(_imageFilePath);
			ImageSize = new ImageSize(image.Size.Width, image.Size.Height);

			resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);
		}
		catch
        {
            resizedImage = GlobalData.InvalidImage;
			ImageSize = GlobalData.InvalidImageSize;
		}
        finally
        {
            image?.Dispose();
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

    public Bitmap GetThumbnail()
    {
        lock (_thumbnailGenerationLockObject)
        {
            Bitmap thumbnail;

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
		if (_imageData != null &&
			_imageData != GlobalData.InvalidImage)
		{
			_imageData.Dispose();
		}

		_imageData = null;
	}

	#region Private

	private readonly IImageResizer _imageResizer;
    private readonly string _imageFilePath;
    private readonly object _thumbnailGenerationLockObject;

    private Bitmap _imageData;

	#endregion
}
