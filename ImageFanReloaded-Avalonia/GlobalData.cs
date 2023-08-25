using System;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;
using ImageFanReloaded.Properties;

namespace ImageFanReloaded
{
    public static class GlobalData
    {
        public static readonly ImageSize ThumbnailSize;

		public static readonly IImage InvalidImage;
		public static readonly ImageSize InvalidImageSize;

		public static readonly IImage InvalidImageThumbnail;

		public static readonly IImage LoadingImageThumbnail;

		public static readonly IImage DriveIcon;
        public static readonly IImage FolderIcon;

        public static readonly int ProcessorCount;

        static GlobalData()
        {
            ThumbnailSize = new ImageSize(ThumbnailSizeSquareLength);
            IconSize = new ImageSize(IconSizeSquareLength);

			IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
			IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

            InvalidImage = GetImageFromResource(Resources.InvalidImage);
			InvalidImageSize = new ImageSize(
				InvalidImage.Size.Width, InvalidImage.Size.Height);

			InvalidImageThumbnail = imageResizer
                .CreateResizedImage(InvalidImage, ThumbnailSize);

			var loadingImage = GetImageFromResource(Resources.LoadingImage);
            LoadingImageThumbnail = imageResizer
                .CreateResizedImage(loadingImage, ThumbnailSize);

            var driveImage = GetImageFromResource(Resources.DriveIcon);
            DriveIcon = imageResizer.CreateResizedImage(driveImage, IconSize);

			var folderImage = GetImageFromResource(Resources.FolderIcon);
			FolderIcon = imageResizer.CreateResizedImage(folderImage, IconSize);

			ProcessorCount = Environment.ProcessorCount;
        }

        #region Private

        private const int ThumbnailSizeSquareLength = 250;
		private const int IconSizeSquareLength = 24;

		private static readonly ImageSize IconSize;

		private static IImage GetImageFromResource(byte[] resourceData)
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
}
