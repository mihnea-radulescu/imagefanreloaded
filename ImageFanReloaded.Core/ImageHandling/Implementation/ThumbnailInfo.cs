using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Global;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ThumbnailInfo : IThumbnailInfo
{
    public ThumbnailInfo(
        IGlobalParameters globalParameters,
        IDispatcher dispatcher,
        IImageFile imageFile)
    {
        _globalParameters = globalParameters;
        _dispatcher = dispatcher;
        ImageFile = imageFile;

        ThumbnailImage = _globalParameters.LoadingImageThumbnail;
    }

    public IThumbnailBox? ThumbnailBox { get; set; }
    public IImage? ThumbnailImage { get; private set; }
    
    public IImageFile ImageFile { get; }
    public string ThumbnailText => ImageFile.FileName;

    public void ReadThumbnailInputFromDisc() => ImageFile.ReadImageDataFromDisc();

    public void GetThumbnail()
    {
        try
        {
            GetThumbnailHelper();
        }
        catch (InvalidOperationException)
        {
            _dispatcher.Invoke(GetThumbnailHelper);
        }
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
    private readonly IDispatcher _dispatcher;

    private void GetThumbnailHelper()
    {
        ThumbnailImage = ImageFile.GetThumbnail();
	}

    #endregion
}
