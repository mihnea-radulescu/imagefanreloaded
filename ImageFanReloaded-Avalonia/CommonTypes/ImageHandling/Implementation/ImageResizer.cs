using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

public class ImageResizer
    : IImageResizer
{
	public ImageResizer(
        IImageResizeCalculator imageResizeCalculator)
    {
        _imageResizeCalculator = imageResizeCalculator;
	}

    public IImage CreateResizedImage(
        IImage image, ImageSize viewPortSize)
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
        var bitmap = (Bitmap)image;
        var destinationSize = new PixelSize(
            resizedImageSize.Width, resizedImageSize.Height);

        var resizedImage = bitmap.CreateScaledBitmap(destinationSize);
        return resizedImage;
	}

    #endregion
}
