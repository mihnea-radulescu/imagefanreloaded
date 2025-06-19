using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public abstract class ImageFileBase : IImageFile
{
	protected ImageFileBase(
	    IGlobalParameters globalParameters,
	    IImageResizer imageResizer,
	    string imageFileName,
	    string imageFilePath,
	    decimal sizeOnDiscInKilobytes)
    {
	    _globalParameters = globalParameters;
	    _imageResizer = imageResizer;
        
        ImageFileName = imageFileName;
        ImageFilePath = imageFilePath;
        
        SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
        ImageSize = _globalParameters.InvalidImage.Size;

		_thumbnailGenerationLockObject = new object();

		_hasReadImageError = false;
    }

    public string ImageFileName { get; }
    public string ImageFilePath { get; }
    
    public decimal SizeOnDiscInKilobytes { get; }
	public ImageSize ImageSize { get; private set; }

	public abstract ImageInfo? ImageInfo { get; protected set; }

	public IImage GetImage()
	{
		IImage image;

		try
		{
			image = GetImageFromDisc(ImageFilePath);
			ImageSize = image.Size;
		}
		catch
		{
			ImageSize = _globalParameters.InvalidImage.Size;
			image = _globalParameters.InvalidImage;

			_hasReadImageError = true;
		}

		return image;
	}

    public IImage GetResizedImage(ImageSize viewPortSize)
    {
        IImage? image = null;
        IImage resizedImage;

        try
		{
			image = GetImageFromDisc(ImageFilePath);
			ImageSize = image.Size;
			
			resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize, ImageQuality.High);
		}
		catch
        {
	        ImageSize = _globalParameters.InvalidImage.Size;
	        resizedImage = _globalParameters.InvalidImage;
			
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
		IImage imageData;

		try
		{
			imageData = GetImageFromDisc(ImageFilePath);
			ImageSize = imageData.Size;
		}
		catch
		{
			ImageSize = _globalParameters.InvalidImage.Size;
			imageData = _globalParameters.InvalidImage;

			_hasReadImageError = true;
		}
		
		lock (_thumbnailGenerationLockObject)
		{
			_imageData = imageData;
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
					_imageData!, thumbnailImageSize, ImageQuality.Medium);
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
			if (_imageData is not null &&
			    _globalParameters.CanDisposeImage(_imageData))
			{
				_imageData.Dispose();
				_imageData = null;
			}
		}
	}

	public string GetImageInfo(bool longFormat)
	{
		var imageFileInfo = longFormat ? ImageFilePath : ImageFileName;
		
		var imageInfo = _hasReadImageError
			? $"{imageFileInfo} - invalid image"
			: $"{imageFileInfo} - {ImageSize} - {SizeOnDiscInKilobytes} KB";
				
		return imageInfo;
	}
	
	#region Protected

	protected abstract IImage GetImageFromDisc(string imageFilePath);
	
	#endregion

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;
    private readonly object _thumbnailGenerationLockObject;

    private IImage? _imageData;
    private bool _hasReadImageError;

    #endregion
}
