using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Global;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ThumbnailInfo : IThumbnailInfo
{
    public ThumbnailInfo(
        IGlobalParameters globalParameters,
        IImageFile imageFile)
    {
        _globalParameters = globalParameters;
        ImageFile = imageFile;

        ThumbnailImage = _globalParameters.LoadingImageThumbnail;
    }

    public IThumbnailBox? ThumbnailBox { get; set; }
    public IImage? ThumbnailImage { get; private set; }
    
    public IImageFile ImageFile { get; }
    public string ThumbnailText => ImageFile.ImageFileName;

    public void ReadThumbnailInputFromDisc() => ImageFile.ReadImageDataFromDisc();

    public void GetThumbnail()
    {
        ThumbnailImage = ImageFile.GetThumbnail();
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

    #endregion
}
