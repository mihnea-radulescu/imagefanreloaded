using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Processing;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

public class ImageResizer
    : IImageResizer
{
	public ImageResizer(
        IImageResizeCalculator imageResizeCalculator,
        IImageEncoderFactory imageEncoderFactory)
    {
	    _imageResizeCalculator = imageResizeCalculator;
		_imageEncoderFactory = imageEncoderFactory;
	}

    public IImage CreateResizedImage(
        IImage image, ImageSize viewPortSize, ImageFormat imageFormat = ImageFormat.Jpeg)
	{
		var imageSize = new ImageSize(image.Size.Width, image.Size.Height);

		var resizedImageSize = _imageResizeCalculator
		    .GetResizedImageSize(imageSize, viewPortSize);

        var resizedImage = BuildResizedImage(image, resizedImageSize, imageFormat);
        return resizedImage;
    }

    #region Private

    private readonly IImageResizeCalculator _imageResizeCalculator;
	private readonly IImageEncoderFactory _imageEncoderFactory;

	private IImage BuildResizedImage(
        IImage image, ImageSize resizedImageSize, ImageFormat imageFormat)
    {
        var bitmap = (Bitmap)image;

        using (Stream stream = new MemoryStream())
		{
            bitmap.Save(stream);
            stream.Position = 0;

			using (SixLabors.ImageSharp.Image loadedImage =
                   SixLabors.ImageSharp.Image.Load(stream))
			{
				loadedImage.Mutate(context =>
                context.Resize(
                    resizedImageSize.Width,
                    resizedImageSize.Height,
                    KnownResamplers.Lanczos3));

                using (Stream saveStream = new MemoryStream())
                {
                    var imageEncoder = _imageEncoderFactory.GetImageEncoder(imageFormat);
                    
                    loadedImage.Save(saveStream, imageEncoder);
                    saveStream.Position = 0;

                    var resizedImage = new Bitmap(saveStream);
                    return resizedImage;
				}
			}
		}
	}

    #endregion
}
