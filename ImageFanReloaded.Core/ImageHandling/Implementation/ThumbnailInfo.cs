using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ThumbnailInfo : IThumbnailInfo
{
    public ThumbnailInfo(
        IGlobalParameters globalParameters,
        int thumbnailSize,
        IImageFile imageFile)
    {
        _globalParameters = globalParameters;
        _thumbnailSize = thumbnailSize;
        
        ImageFile = imageFile;
        ThumbnailImage = _globalParameters.GetLoadingImageThumbnail(_thumbnailSize);
    }

    public IThumbnailBox? ThumbnailBox { get; set; }
    public IImage? ThumbnailImage { get; private set; }
    
    public IImageFile ImageFile { get; }
    public string ThumbnailText => ImageFile.ImageFileName;

    public void ReadThumbnailInputFromDisc() => ImageFile.ReadImageDataFromDisc();

    public void GetThumbnail()
    {
        ThumbnailImage = ImageFile.GetThumbnail(_thumbnailSize);
    }

    public void RefreshThumbnail() => ThumbnailBox!.RefreshThumbnail();

    public void DisposeThumbnail()
    {
        if (ThumbnailImage is not null &&
            _globalParameters.CanDisposeImage(ThumbnailImage))
        {
            ThumbnailImage.Dispose();
            ThumbnailImage = null;
		}
    }

    #region Private

    private readonly IGlobalParameters _globalParameters;
    private readonly int _thumbnailSize;

    #endregion
}
