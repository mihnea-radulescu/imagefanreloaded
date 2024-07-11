using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.Settings.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Properties;

namespace ImageFanReloaded.Settings;

public class GlobalParameters : GlobalParametersBase
{
	public GlobalParameters(
		IOperatingSystemSettings operatingSystemSettings,
		IAboutInformationProvider aboutInformationProvider,
		IImageResizer imageResizer,
		IAppSettings appSettings)
		: base(operatingSystemSettings, aboutInformationProvider)
	{
		_imageResizer = imageResizer;
		
		var invalidBitmap = GetBitmapFromResource(Resources.InvalidImage);
		var invalidBitmapSize = new ImageSize(invalidBitmap.Size.Width, invalidBitmap.Size.Height);
		
		var thumbnailSizeInPixels = appSettings.ThumbnailSizeInPixels;
		var thumbnailSize = new ImageSize(thumbnailSizeInPixels);
		ThumbnailSize = thumbnailSize;
		
		var invalidImage = new Image(invalidBitmap, invalidBitmapSize);
		var invalidImageThumbnail = imageResizer.CreateResizedImage(invalidImage, thumbnailSize);
		InvalidImage = invalidImage;
		InvalidImageThumbnail = invalidImageThumbnail;

		IImage loadingImageThumbnail;
        using (var loadingBitmap = GetBitmapFromResource(Resources.LoadingImage))
        {
            var loadingBitmapSize = new ImageSize(loadingBitmap.Size.Width, loadingBitmap.Size.Height);
            var loadingImage = new Image(loadingBitmap, loadingBitmapSize);
            loadingImageThumbnail = imageResizer.CreateResizedImage(loadingImage, thumbnailSize);
            LoadingImageThumbnail = loadingImageThumbnail;
        }

        PersistentImages = [invalidImage, invalidImageThumbnail, loadingImageThumbnail];
        
        var iconSize = new ImageSize(IconSizeSquareLength);

        DesktopFolderIcon = GetResizedIcon(Resources.DesktopFolderIcon, iconSize);
        DocumentsFolderIcon = GetResizedIcon(Resources.DocumentsFolderIcon, iconSize);
        DownloadsFolderIcon = GetResizedIcon(Resources.DownloadsFolderIcon, iconSize);
        DriveIcon = GetResizedIcon(Resources.DriveIcon, iconSize);
        FolderIcon = GetResizedIcon(Resources.FolderIcon, iconSize);
        HomeFolderIcon = GetResizedIcon(Resources.HomeFolderIcon, iconSize);
        PicturesFolderIcon = GetResizedIcon(Resources.PicturesFolderIcon, iconSize);
	}
	
	public override IImage InvalidImage { get; }
	
	public override ImageSize ThumbnailSize { get; }
	public override IImage InvalidImageThumbnail { get; }
	public override IImage LoadingImageThumbnail { get; }
	
	public override HashSet<IImage> PersistentImages { get; }
	
	public override IImage DesktopFolderIcon { get; }
	public override IImage DocumentsFolderIcon { get; }
	public override IImage DownloadsFolderIcon { get; }
	public override IImage DriveIcon { get; }
	public override IImage FolderIcon { get; }
	public override IImage HomeFolderIcon { get; }
	public override IImage PicturesFolderIcon { get; }
	
	#region Private
	
	private readonly IImageResizer _imageResizer;

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

	#endregion
}
