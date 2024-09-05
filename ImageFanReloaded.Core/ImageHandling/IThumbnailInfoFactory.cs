namespace ImageFanReloaded.Core.ImageHandling;

public interface IThumbnailInfoFactory
{
	public IThumbnailInfo GetThumbnailInfo(int thumbnailSize, IImageFile imageFile);
}
