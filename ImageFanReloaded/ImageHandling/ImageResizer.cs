using Avalonia;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;

namespace ImageFanReloaded.ImageHandling;

public class ImageResizer : ImageResizerBase
{
	public ImageResizer(IImageResizeCalculator imageResizeCalculator)
		: base(imageResizeCalculator)
    {
	}
	
    #region Protected

	protected override IImage BuildResizedImage(IImage image, ImageSize resizedImageSize)
    {
        var destinationSize = new PixelSize(resizedImageSize.Width, resizedImageSize.Height);

        var bitmap = image.GetBitmap();
        var resizedBitmap = bitmap.CreateScaledBitmap(destinationSize);
        
        var resizedImage = new Image(resizedBitmap, resizedImageSize);
        return resizedImage;
	}

    #endregion
}
