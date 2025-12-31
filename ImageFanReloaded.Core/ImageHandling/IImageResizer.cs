namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageResizer
{
	IImage CreateDownsizedImage(IImage image, ImageSize viewPortSize);

	IImage CreateUpsizedImage(IImage image, double scalingFactor);
}
