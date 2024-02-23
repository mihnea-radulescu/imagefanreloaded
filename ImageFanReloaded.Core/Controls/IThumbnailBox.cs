using System;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls;

public interface IThumbnailBox
{
    void SetControlProperties(IGlobalParameters globalParameters);
    
    event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxSelected;
    event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxClicked;
    
    IImageFile? ImageFile { get; }
    bool IsSelected { get; }
    
    IThumbnailInfo? ThumbnailInfo { get; set; }

    void SelectThumbnail();
    void UnselectThumbnail();
    void BringThumbnailIntoView();
    void RefreshThumbnail();
    void DisposeThumbnail();
}
