using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IThumbnailBox
{
	event EventHandler<ThumbnailBoxSelectedEventArgs>? ThumbnailBoxSelected;
	event EventHandler<ThumbnailBoxClickedEventArgs>? ThumbnailBoxClicked;

	int Index { get; set; }
	IThumbnailInfo? ThumbnailInfo { get; set; }

	IImageFile? ImageFile { get; }
	bool HasImageError { get; }

	bool IsSelected { get; }

	IMouseCursorFactory? MouseCursorFactory { get; set; }

	void SetControlProperties(int thumbnailSize, IGlobalParameters globalParameters);

	void SelectThumbnail();
	void UnselectThumbnail();

	void BringThumbnailIntoView();
	void RefreshThumbnail();

	bool IsAnimated { get; }
	Task AnimationTask { get; }
	void NotifyStopAnimation();

	Task DisposeThumbnail();
}
