using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.Global.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Properties;

namespace ImageFanReloaded.Global;

public class GlobalParameters : GlobalParametersBase
{
	public GlobalParameters(
		IImageResizeCalculator imageResizeCalculator,
		IImageResizer imageResizer)
		: base(imageResizeCalculator, imageResizer)
	{
		var invalidBitmap = GetBitmapFromResource(Resources.InvalidImage);
		var invalidBitmapSize = new ImageSize(
			invalidBitmap.Size.Width, invalidBitmap.Size.Height);
		
		var invalidImage = new Image(invalidBitmap, invalidBitmapSize);
		var invalidImageThumbnail = imageResizer.CreateResizedImage(invalidImage, ThumbnailSize);
		InvalidImage = invalidImage;
		InvalidImageThumbnail = invalidImageThumbnail;

		IImage loadingImageThumbnail;
        using (var loadingBitmap = GetBitmapFromResource(Resources.LoadingImage))
        {
            var loadingBitmapSize = new ImageSize(loadingBitmap.Size.Width, loadingBitmap.Size.Height);
            var loadingImage = new Image(loadingBitmap, loadingBitmapSize);
            loadingImageThumbnail = imageResizer.CreateResizedImage(loadingImage, ThumbnailSize);
            LoadingImageThumbnail = loadingImageThumbnail;
        }

        PersistentImages = [invalidImage, invalidImageThumbnail, loadingImageThumbnail];
        
        var iconSize = new ImageSize(IconSizeSquareLength);

        using (var driveBitmap = GetBitmapFromResource(Resources.DriveIcon))
        {
            var driveBitmapSize = new ImageSize(
	            driveBitmap.Size.Width, driveBitmap.Size.Height);
            var driveImage = new Image(driveBitmap, driveBitmapSize);
            
            DriveIcon = imageResizer.CreateResizedImage(driveImage, iconSize);
        }

		using (var folderBitmap = GetBitmapFromResource(Resources.FolderIcon))
		{
			var folderBitmapSize = new ImageSize(
				folderBitmap.Size.Width, folderBitmap.Size.Height);
			var folderImage = new Image(folderBitmap, folderBitmapSize);
            
			FolderIcon = imageResizer.CreateResizedImage(folderImage, iconSize);
		}
	}
	
	public override IImage InvalidImage { get; }
	public override IImage InvalidImageThumbnail { get; }
	public override IImage LoadingImageThumbnail { get; }
	
	public override HashSet<IImage> PersistentImages { get; }
	
	public override IImage DriveIcon { get; }
	public override IImage FolderIcon { get; }
	
	#region Private

	private static Bitmap GetBitmapFromResource(byte[] resourceData)
	{
		using (Stream stream = new MemoryStream(resourceData))
		{
			stream.Position = 0;

			var image = new Bitmap(stream);
			return image;
		}
	}

	#endregion
}
