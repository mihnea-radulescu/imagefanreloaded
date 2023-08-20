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
        public const int ThumbnailSize = 250;

        public static readonly Image InvalidImageAsBitmap;

        public static readonly ImageSource InvalidImage;
        public static readonly ImageSource InvalidImageThumbnail;

        public static readonly ImageSource LoadingImageThumbnail;

        public static readonly ImageSource DriveIcon;
        public static readonly ImageSource FolderIcon;

        public static readonly int ProcessorCount;

        public static readonly HashSet<Key> BackwardNavigationKeys;
        public static readonly HashSet<Key> ForwardNavigationKeys;

        static GlobalData()
        {
            IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
            IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

            InvalidImageAsBitmap = Resources.InvalidImage;

            InvalidImage = Resources.InvalidImage.ConvertToImageSource();
            InvalidImageThumbnail = imageResizer
                                        .CreateThumbnail(Resources.InvalidImage, ThumbnailSize)
                                        .ConvertToImageSource();

            LoadingImageThumbnail = imageResizer
                                        .CreateThumbnail(Resources.LoadingImage, ThumbnailSize)
                                        .ConvertToImageSource();

            using (var driveIconBitmap = Resources.DriveIcon)
            {
                DriveIcon = imageResizer
                                        .CreateThumbnail(driveIconBitmap, IconSize)
                                        .ConvertToImageSource();
            }

            using (var folderIconBitmap = Resources.FolderIcon)
            {
                FolderIcon = imageResizer
                                        .CreateThumbnail(folderIconBitmap, IconSize)
                                        .ConvertToImageSource();
            }

            ProcessorCount = Environment.ProcessorCount;

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

		private const int IconSize = 24;

		#endregion
	}
}
