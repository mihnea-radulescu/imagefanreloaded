using ImageFanReloaded.Core.Global;

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
			
			resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);
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

    public IImage GetThumbnail()
    {
        lock (_thumbnailGenerationLockObject)
        {
            IImage thumbnail;

            try
            {
                thumbnail = _imageResizer.CreateResizedImage(_imageData!, _globalParameters.ThumbnailSize);
            }
            catch
            {
	            thumbnail = _globalParameters.InvalidImageThumbnail;
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

	public string GetImageInfo()
	{
		var imageInfo = _hasReadImageError
			? $"{ImageFilePath} - invalid image"
			: $"{ImageFilePath} - {ImageSize.Width}x{ImageSize.Height} - {SizeOnDiscInKilobytes} KB";
				
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
