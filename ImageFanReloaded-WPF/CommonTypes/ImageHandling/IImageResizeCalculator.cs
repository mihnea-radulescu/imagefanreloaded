namespace ImageFanReloaded.CommonTypes.ImageHandling
{
	public interface IImageResizeCalculator
	{
		ImageSize GetResizedImageSize(
			ImageSize imageSize, ImageSize viewPortSize);
	}
}
