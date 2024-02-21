using Avalonia.Media.Imaging;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

public class ImageFile : IImageFile
{
    public ImageFile(
	    IImageResizer imageResizer,
	    string imageFileName,
	    string imageFilePath,
	    int sizeOnDiscInKilobytes)
    {
        _imageResizer = imageResizer;
        
        FileName = imageFileName;
        _imageFilePath = imageFilePath;
        
        SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
        ImageSize = GlobalData.InvalidImageSize;

		_thumbnailGenerationLockObject = new object();

		_hasReadImageError = false;
    }

    public string FileName { get; }
    
    public int SizeOnDiscInKilobytes { get; }
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
			
			_hasReadImageError = true;
		}

        return image;
    }

    public Bitmap GetResizedImage(ImageSize viewPortSize)
    {
        Bitmap? image = null;
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
			
			_hasReadImageError = true;
		}
        finally
        {
            image?.Dispose();
        }

        return resizedImage;
    }

	public void ReadImageDataFromDisc()
	{
		Bitmap imageData;

		try
		{
			imageData = new Bitmap(_imageFilePath);
			ImageSize = new ImageSize(imageData.Size.Width, imageData.Size.Height);
		}
		catch
		{
			imageData = GlobalData.InvalidImage;
			ImageSize = GlobalData.InvalidImageSize;

			_hasReadImageError = true;
		}
		
		lock (_thumbnailGenerationLockObject)
		{
			_imageData = imageData;
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
                    .CreateResizedImage(_imageData!, GlobalData.ThumbnailSize);
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
		lock (_thumbnailGenerationLockObject)
		{
			if (_imageData is not null &&
			    _imageData != GlobalData.InvalidImage)
			{
				_imageData.Dispose();
			}

			_imageData = null;
		}
	}

	public string GetImageInfo()
	{
		var imageSizeText = _hasReadImageError
			? " - invalid image - "
			: $" - {ImageSize!.Width}x{ImageSize!.Height} - ";

		var imageInfo = $"{FileName}{imageSizeText}{SizeOnDiscInKilobytes} KB";
		return imageInfo;
	}

	#region Private

	private readonly IImageResizer _imageResizer;
    private readonly string _imageFilePath;
    private readonly object _thumbnailGenerationLockObject;

    private Bitmap? _imageData;
    private bool _hasReadImageError;

    #endregion
}
