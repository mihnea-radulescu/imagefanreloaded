namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageResizeCalculator
{
	ImageSize GetDownsizedImageSize(
		ImageSize imageSize, ImageSize viewPortSize);

	ImageSize GetUpsizedImageSize(ImageSize imageSize, double scalingFactor);
}
