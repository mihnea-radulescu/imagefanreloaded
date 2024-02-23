namespace ImageFanReloaded.Core.ImageHandling;

public interface IThumbnailInfoFactory
{
	public IThumbnailInfo GetThumbnailInfo(IImageFile imageFile);
}
