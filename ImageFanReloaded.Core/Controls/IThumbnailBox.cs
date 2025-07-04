using System;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IThumbnailBox
{
    event EventHandler<ThumbnailBoxSelectedEventArgs>? ThumbnailBoxSelected;
    event EventHandler<ThumbnailBoxClickedEventArgs>? ThumbnailBoxClicked;
    
    int Index { get; set; }
    IThumbnailInfo? ThumbnailInfo { get; set; }
    
    IImageFile? ImageFile { get; }
    bool IsSelected { get; }
    
    void SetControlProperties(int thumbnailSize, IGlobalParameters globalParameters);

    void SelectThumbnail();
    void UnselectThumbnail();
    void BringThumbnailIntoView();
    void RefreshThumbnail();
    void DisposeThumbnail();
}
