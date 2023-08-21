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

        _thumbnailGenerationLockObject = new object();
    }

    public string FileName { get; }

    public IImage GetImage()
    {
        IImage image;

        try
        {
            image = new Bitmap(_imageFilePath);
        }
        catch
        {
            image = GlobalData.InvalidImage;
		}

        return image;
    }

    public IImage GetResizedImage(ImageSize viewPortSize)
    {
        IImage resizedImage;

        try
        {
            var image = new Bitmap(_imageFilePath);

            resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);
        }
        catch
        {
            resizedImage = GlobalData.InvalidImage;
        }

        return resizedImage;
    }

    public void ReadThumbnailInputFromDisc()
    {
        lock (_thumbnailGenerationLockObject)
        {
            try
            {
                _thumbnailInput = new Bitmap(_imageFilePath);
            }
            catch
            {
                _thumbnailInput = GlobalData.InvalidImage;
            }
        }
    }

    public IImage GetThumbnail()
    {
        lock (_thumbnailGenerationLockObject)
        {
            if (_thumbnailInput == null)
            {
                throw new InvalidOperationException(
                    $"The method {nameof(ReadThumbnailInputFromDisc)} must be executed prior to calling the method {nameof(GetThumbnail)}.");
            }

            IImage thumbnail;

            try
            {
                thumbnail = _imageResizer
                    .CreateResizedImage(_thumbnailInput, GlobalData.ThumbnailSize);
            }
            catch
            {
                thumbnail = GlobalData.InvalidImageThumbnail;
            }
            finally
            {
                DisposeThumbnailInput();
            }

            return thumbnail;
        }
    }

	public void DisposeThumbnailInput()
	{
		_thumbnailInput = null;
	}

	#region Private

	private readonly IImageResizer _imageResizer;
    private readonly string _imageFilePath;
    private readonly object _thumbnailGenerationLockObject;

    private IImage _thumbnailInput;

    #endregion
}
