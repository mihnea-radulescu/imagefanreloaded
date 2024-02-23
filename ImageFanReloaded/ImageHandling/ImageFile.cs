using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;

namespace ImageFanReloaded.ImageHandling;

public class ImageFile : IImageFile
{
    public ImageFile(
	    IGlobalParameters globalParameters,
	    IImageResizer imageResizer,
	    string imageFileName,
	    string imageFilePath,
	    int sizeOnDiscInKilobytes)
    {
	    _globalParameters = globalParameters;
	    _imageResizer = imageResizer;
        
        FileName = imageFileName;
        _imageFilePath = imageFilePath;
        
        SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
        ImageSize = _globalParameters.InvalidImage.Size;

		_thumbnailGenerationLockObject = new object();

		_hasReadImageError = false;
    }

    public string FileName { get; }
    
    public int SizeOnDiscInKilobytes { get; }
	public ImageSize ImageSize { get; private set; }

	public IImage GetImage()
    {
        IImage image;

        try
        {
	        var bitmap = new Bitmap(_imageFilePath);
            ImageSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);
            image = new Image(bitmap, ImageSize);
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
			var bitmap = new Bitmap(_imageFilePath);
			ImageSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);
			image = new Image(bitmap, ImageSize);

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
			var bitmap = new Bitmap(_imageFilePath);
			ImageSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);
			imageData = new Image(bitmap, ImageSize);
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
		var imageSizeText = _hasReadImageError
			? " - invalid image - "
			: $" - {ImageSize!.Width}x{ImageSize!.Height} - ";

		var imageInfo = $"{FileName}{imageSizeText}{SizeOnDiscInKilobytes} KB";
		return imageInfo;
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageResizer _imageResizer;
    private readonly string _imageFilePath;
    private readonly object _thumbnailGenerationLockObject;

    private IImage? _imageData;
    private bool _hasReadImageError;

    #endregion
}
