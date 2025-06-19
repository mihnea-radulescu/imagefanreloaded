namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageInfoBuilder
{
	ImageInfo BuildBasicImageInfo(
		string imageFileName,
		string imageFilePath,
		decimal sizeOnDiscInKilobytes,
		ImageSize? imageSize);

	ImageInfo BuildExtendedImageInfo(
		string imageFileName,
		string imageFilePath,
		decimal sizeOnDiscInKilobytes,
		object? imageObject);
}
