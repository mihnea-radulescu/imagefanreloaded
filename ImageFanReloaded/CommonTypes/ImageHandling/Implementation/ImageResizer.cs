using Avalonia;
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

    public Bitmap CreateResizedImage(
        Bitmap image, ImageSize viewPortSize)
	{
		var imageSize = new ImageSize(image.Size.Width, image.Size.Height);

		var resizedImageSize = _imageResizeCalculator
		    .GetResizedImageSize(imageSize, viewPortSize);

        var resizedImage = BuildResizedImage(image, resizedImageSize);
        return resizedImage;
    }

    #region Private

    private readonly IImageResizeCalculator _imageResizeCalculator;

	private static Bitmap BuildResizedImage(Bitmap image, ImageSize resizedImageSize)
    {
        var destinationSize = new PixelSize(
            resizedImageSize.Width, resizedImageSize.Height);

        var resizedImage = image.CreateScaledBitmap(destinationSize);
        return resizedImage;
	}

    #endregion
}
