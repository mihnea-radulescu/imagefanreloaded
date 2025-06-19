namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageInfoBuilder
{
	ImageInfo BuildImageInfo(
		string imageFileName,
		string imageFilePath,
		decimal sizeOnDiscInKilobytes,
		object? imageObject);
}
