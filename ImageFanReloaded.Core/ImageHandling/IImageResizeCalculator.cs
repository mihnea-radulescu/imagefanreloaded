namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageResizeCalculator
{
	ImageSize GetResizedImageSize(ImageSize imageSize, ImageSize viewPortSize);
}
