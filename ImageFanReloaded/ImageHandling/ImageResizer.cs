using Avalonia;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;

namespace ImageFanReloaded.ImageHandling;

public class ImageResizer : IImageResizer
{
	public ImageResizer(IImageResizeCalculator imageResizeCalculator)
    {
        _imageResizeCalculator = imageResizeCalculator;
	}

    public IImage CreateResizedImage(IImage image, ImageSize viewPortSize)
	{
		var imageSize = new ImageSize(image.Size.Width, image.Size.Height);

		var resizedImageSize = _imageResizeCalculator
			.GetResizedImageSize(imageSize, viewPortSize);

        var resizedImage = BuildResizedImage(image, resizedImageSize);
        return resizedImage;
    }

    #region Private

    private readonly IImageResizeCalculator _imageResizeCalculator;

	private static IImage BuildResizedImage(IImage image, ImageSize resizedImageSize)
    {
        var destinationSize = new PixelSize(resizedImageSize.Width, resizedImageSize.Height);

        var bitmap = image.GetBitmap();
        var resizedBitmap = bitmap.CreateScaledBitmap(destinationSize);
        
        var resizedImage = new Image(resizedBitmap, resizedImageSize);
        return resizedImage;
	}

    #endregion
}
