using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IThumbnailInfoFactory
{
	public IThumbnailInfo GetThumbnailInfo(ITabOptions tabOptions, IImageFile imageFile);
}
