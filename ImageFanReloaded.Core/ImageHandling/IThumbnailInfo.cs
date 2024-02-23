using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IThumbnailInfo
{
	IThumbnailBox? ThumbnailBox { get; set; }
	IImage? ThumbnailImage { get; }
	
	IImageFile ImageFile { get; }
	string ThumbnailText { get; }
	
	void ReadThumbnailInputFromDisc();
	
	void GetThumbnail();
	void RefreshThumbnail();
	void DisposeThumbnail();
}
