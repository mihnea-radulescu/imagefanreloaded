using System;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls;

public interface IThumbnailBox
{
    event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxSelected;
    event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxClicked;
    
    int Index { get; set; }
    IThumbnailInfo? ThumbnailInfo { get; set; }
    
    IImageFile? ImageFile { get; }
    bool IsSelected { get; }
    
    void SetControlProperties(IGlobalParameters globalParameters);

    void SelectThumbnail();
    void UnselectThumbnail();
    void BringThumbnailIntoView();
    void RefreshThumbnail();
    void DisposeThumbnail();
}
