using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;
using ImageFanReloaded.Properties;

namespace ImageFanReloaded
{
    public static class GlobalData
    {
		public static readonly ImageSize ThumbnailSize;

		public static readonly Bitmap InvalidImageAsBitmap;

        public static readonly ImageSource InvalidImage;
		public static readonly ImageSize InvalidImageSize;

		public static readonly ImageSource InvalidImageThumbnail;

		public static readonly ImageSource LoadingImageThumbnail;

        public static readonly ImageSource DriveIcon;
        public static readonly ImageSource FolderIcon;

        public static readonly int ProcessorCount;

		public static readonly Key TabSwitchKey;

		public static readonly Key EscapeKey;
		public static readonly Key EnterKey;

		public static readonly HashSet<Key> BackwardNavigationKeys;
        public static readonly HashSet<Key> ForwardNavigationKeys;

        static GlobalData()
        {
			ThumbnailSize = new ImageSize(ThumbnailSizeSquareLength);
			IconSize = new ImageSize(IconSizeSquareLength);

			IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
            IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

            InvalidImageAsBitmap = Resources.InvalidImage;

            InvalidImage = Resources.InvalidImage.ConvertToImageSource();
			InvalidImageSize = new ImageSize(InvalidImage.Width, InvalidImage.Height);

			using (var resizedInvalidImage = imageResizer
					.CreateResizedImage(Resources.InvalidImage, ThumbnailSize))
			{
				InvalidImageThumbnail = resizedInvalidImage.ConvertToImageSource();
			}

			using (var resizedLoadingImage = imageResizer
					.CreateResizedImage(Resources.LoadingImage, ThumbnailSize))
			{
				LoadingImageThumbnail = resizedLoadingImage.ConvertToImageSource();
			}

			using (var residedDriveIcon = imageResizer
					.CreateResizedImage(Resources.DriveIcon, IconSize))
			{
				DriveIcon = residedDriveIcon.ConvertToImageSource();
			}

			using (var resizedFolderIcon = imageResizer
				.CreateResizedImage(Resources.FolderIcon, IconSize))
			{
				FolderIcon = resizedFolderIcon.ConvertToImageSource();
			}

			ProcessorCount = Environment.ProcessorCount;

            TabSwitchKey = Key.Tab;

            EscapeKey = Key.Escape;
            EnterKey = Key.Enter;

            BackwardNavigationKeys = new HashSet<Key>
            { 
                Key.W, Key.A, Key.Up, Key.Left, Key.Back, Key.PageUp
            };

            ForwardNavigationKeys = new HashSet<Key>
            { 
                Key.S, Key.D, Key.Down, Key.Right, Key.Space, Key.PageDown
            };
        }

		#region Private

		private const int ThumbnailSizeSquareLength = 250;
		private const int IconSizeSquareLength = 24;

		private static readonly ImageSize IconSize;

		#endregion
	}
}
