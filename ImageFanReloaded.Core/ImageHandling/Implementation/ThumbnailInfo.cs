using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ThumbnailInfo : IThumbnailInfo
{
	public ThumbnailInfo(
		IGlobalParameters globalParameters,
		ITabOptions tabOptions,
		IImageFile imageFile)
	{
		_globalParameters = globalParameters;

		_thumbnailSize = tabOptions.ThumbnailSize.ToInt();
		_applyImageOrientation = tabOptions.ApplyImageOrientation;

		ImageFile = imageFile;
		ThumbnailImage = _globalParameters.GetLoadingImageThumbnail(_thumbnailSize);
	}

	public IThumbnailBox? ThumbnailBox { get; set; }
	public IImage? ThumbnailImage { get; private set; }

	public IImageFile ImageFile { get; }
	public string ThumbnailText => ImageFile.ImageFileData.ImageFileName;

	public void ReadThumbnailInputFromDisc() => ImageFile.ReadImageFile();

	public void GetThumbnail()
	{
		ThumbnailImage = ImageFile.GetThumbnail(_thumbnailSize, _applyImageOrientation);
	}

	public void RefreshThumbnail() => ThumbnailBox!.RefreshThumbnail();

	public void DisposeThumbnail()
	{
		if (ThumbnailImage is not null && _globalParameters.CanDisposeImage(ThumbnailImage))
		{
			ThumbnailImage.Dispose();
			ThumbnailImage = null;
		}
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;

	private readonly int _thumbnailSize;
	private readonly bool _applyImageOrientation;

	#endregion
}
