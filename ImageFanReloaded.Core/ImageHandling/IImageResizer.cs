namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageResizer
{
	IImage CreateResizedImage(IImage image, ImageSize viewPortSize);
}
