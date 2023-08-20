namespace ImageFanReloaded.CommonTypes.ImageHandling
{
	public interface IImageResizeCalculator
	{
		ImageDimensions GetResizedImageDimensions(
			ImageDimensions imageDimensions, ImageDimensions imageDimensionsToResizeTo);
	}
}
