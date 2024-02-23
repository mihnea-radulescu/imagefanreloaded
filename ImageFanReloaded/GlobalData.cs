using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;
using ImageFanReloaded.Properties;

namespace ImageFanReloaded
{
    public static class GlobalData
    {
        public static readonly ImageSize ThumbnailSize;

		public static readonly Bitmap InvalidImage;
		public static readonly ImageSize InvalidImageSize;

		public static readonly Bitmap InvalidImageThumbnail;

		public static readonly Bitmap LoadingImageThumbnail;

		public static readonly Bitmap DriveIcon;
        public static readonly Bitmap FolderIcon;

        public static readonly int ProcessorCount;

        public static readonly Key TabKey;

        public static readonly Key EscapeKey;
        public static readonly Key EnterKey;

        public static readonly HashSet<Key> BackwardNavigationKeys;
        public static readonly HashSet<Key> ForwardNavigationKeys;

        static GlobalData()
        {
	        ThumbnailSize = new ImageSize(ThumbnailSizeSquareLength);

			IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
			IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

            InvalidImage = GetImageFromResource(Resources.InvalidImage);
			InvalidImageSize = new ImageSize(
				InvalidImage.Size.Width, InvalidImage.Size.Height);

			InvalidImageThumbnail = imageResizer
                .CreateResizedImage(InvalidImage, ThumbnailSize);

            using (var loadingImage = GetImageFromResource(Resources.LoadingImage))
            {
				LoadingImageThumbnail = imageResizer
					.CreateResizedImage(loadingImage, ThumbnailSize);
			}
            
            var iconSize = new ImageSize(IconSizeSquareLength);

            using (var driveImage = GetImageFromResource(Resources.DriveIcon))
            {
				DriveIcon = imageResizer.CreateResizedImage(driveImage, iconSize);
			}

			using (var folderImage = GetImageFromResource(Resources.FolderIcon))
			{
				FolderIcon = imageResizer.CreateResizedImage(folderImage, iconSize);
			}

			ProcessorCount = Environment.ProcessorCount;

            TabKey = Key.Tab;

            EscapeKey = Key.Escape;
            EnterKey = Key.Enter;

            BackwardNavigationKeys = [Key.W, Key.A, Key.Up, Key.Left, Key.Back, Key.PageUp];

            ForwardNavigationKeys = [Key.S, Key.D, Key.Down, Key.Right, Key.Space, Key.PageDown];
        }

        #region Private

        private const int ThumbnailSizeSquareLength = 250;
		private const int IconSizeSquareLength = 24;

		private static Bitmap GetImageFromResource(byte[] resourceData)
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
