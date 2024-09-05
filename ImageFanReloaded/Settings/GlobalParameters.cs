using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.Settings.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Properties;

namespace ImageFanReloaded.Settings;

public class GlobalParameters : GlobalParametersBase
{
	public GlobalParameters(IOperatingSystemSettings operatingSystemSettings, IImageResizer imageResizer)
		: base(operatingSystemSettings)
	{
		_imageResizer = imageResizer;
		
		var invalidBitmap = GetBitmapFromResource(Resources.InvalidImage);
		var invalidBitmapSize = new ImageSize(invalidBitmap.Size.Width, invalidBitmap.Size.Height);
		var invalidImage = new Image(invalidBitmap, invalidBitmapSize);
		InvalidImage = invalidImage;
		
		_validThumbnailSizes = BuildValidThumbnailSizes();
		_invalidImageThumbnails = BuildInvalidImageThumbnails();
		_loadingImageThumbnails = BuildLoadingImageThumbnails();

        PersistentImages = [
	        invalidImage, .._invalidImageThumbnails.Values.ToList(), .._loadingImageThumbnails.Values.ToList()];
        
        var iconSize = new ImageSize(IconSizeSquareLength);

        DesktopFolderIcon = GetResizedIcon(Resources.DesktopFolderIcon, iconSize);
        DocumentsFolderIcon = GetResizedIcon(Resources.DocumentsFolderIcon, iconSize);
        DownloadsFolderIcon = GetResizedIcon(Resources.DownloadsFolderIcon, iconSize);
        DriveIcon = GetResizedIcon(Resources.DriveIcon, iconSize);
        FolderIcon = GetResizedIcon(Resources.FolderIcon, iconSize);
        HomeFolderIcon = GetResizedIcon(Resources.HomeFolderIcon, iconSize);
        PicturesFolderIcon = GetResizedIcon(Resources.PicturesFolderIcon, iconSize);
	}

	public override bool IsValidThumbnailSize(int thumbnailSize) => _validThumbnailSizes.Contains(thumbnailSize);
	
	public override IImage InvalidImage { get; }
	public override HashSet<IImage> PersistentImages { get; }
	
	public override IImage DesktopFolderIcon { get; }
	public override IImage DocumentsFolderIcon { get; }
	public override IImage DownloadsFolderIcon { get; }
	public override IImage DriveIcon { get; }
	public override IImage FolderIcon { get; }
	public override IImage HomeFolderIcon { get; }
	public override IImage PicturesFolderIcon { get; }

	public override IImage GetInvalidImageThumbnail(int thumbnailSize) => _invalidImageThumbnails[thumbnailSize];
	public override IImage GetLoadingImageThumbnail(int thumbnailSize) => _loadingImageThumbnails[thumbnailSize];
	
	#region Private

	private const int ThumbnailSizeLowerThreshold = 50;
	private const int ThumbnailSizeUpperThreshold = 400;
	
	private readonly IImageResizer _imageResizer;

	private readonly HashSet<int> _validThumbnailSizes;
	private readonly IReadOnlyDictionary<int, IImage> _invalidImageThumbnails;
	private readonly IReadOnlyDictionary<int, IImage> _loadingImageThumbnails;

	private static Bitmap GetBitmapFromResource(byte[] resourceData)
	{
		using Stream stream = new MemoryStream(resourceData);
		stream.Position = 0;

		var image = new Bitmap(stream);
		return image;
	}

	private IImage GetResizedIcon(byte[] resourceData, ImageSize newIconSize)
	{
		using var icon = GetBitmapFromResource(resourceData);
		var iconSize = new ImageSize(icon.Size.Width, icon.Size.Height);
		var iconImage = new Image(icon, iconSize);
            
		var resizedIcon = _imageResizer.CreateResizedImage(iconImage, newIconSize);
		return resizedIcon;
	}
	
	private HashSet<int> BuildValidThumbnailSizes()
	{
		var validThumbnailSizes = new HashSet<int>();
		
		for (var thumbnailSize = ThumbnailSizeLowerThreshold;
		     thumbnailSize <= ThumbnailSizeUpperThreshold;
		     thumbnailSize += ThumbnailSizeIncrement)
		{
			validThumbnailSizes.Add(thumbnailSize);
		}

		return validThumbnailSizes;
	}

	private IReadOnlyDictionary<int, IImage> BuildInvalidImageThumbnails()
	{
		var invalidImageThumbnails = new Dictionary<int, IImage>();

		foreach (var thumbnailSize in _validThumbnailSizes)
		{
			var thumbnailImageSize = new ImageSize(thumbnailSize);
			
			var invalidImageThumbnail = _imageResizer.CreateResizedImage(InvalidImage, thumbnailImageSize);
			invalidImageThumbnails.Add(thumbnailSize, invalidImageThumbnail);
		}

		return invalidImageThumbnails;
	}
	
	private IReadOnlyDictionary<int, IImage> BuildLoadingImageThumbnails()
	{
		var loadingImageThumbnails = new Dictionary<int, IImage>();

		using (var loadingBitmap = GetBitmapFromResource(Resources.LoadingImage))
		{
			var loadingBitmapSize = new ImageSize(loadingBitmap.Size.Width, loadingBitmap.Size.Height);
			var loadingImage = new Image(loadingBitmap, loadingBitmapSize);
			
			foreach (var thumbnailSize in _validThumbnailSizes)
			{
				var thumbnailImageSize = new ImageSize(thumbnailSize);
			
				var loadingImageThumbnail = _imageResizer.CreateResizedImage(loadingImage, thumbnailImageSize);
				loadingImageThumbnails.Add(thumbnailSize, loadingImageThumbnail);
			}
		}

		return loadingImageThumbnails;
	}

	#endregion
}
