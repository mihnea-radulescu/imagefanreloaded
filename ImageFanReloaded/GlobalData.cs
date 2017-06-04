using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Factories.Interface;
using ImageFanReloaded.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageFanReloaded
{
    public static class GlobalData
    {
        public const int ThumbnailSize = 200;

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
            TypesFactory = new ProductionTypesFactory();
            
            InvalidImageAsBitmap = Resources.InvalidImage;

            InvalidImage = Resources.InvalidImage.ConvertToImageSource();
            InvalidImageThumbnail = TypesFactory
                                        .ImageResizerInstance
                                        .CreateThumbnail(Resources.InvalidImage, ThumbnailSize)
                                        .ConvertToImageSource();

            LoadingImageThumbnail = TypesFactory
                                        .ImageResizerInstance
                                        .CreateThumbnail(Resources.LoadingImage, ThumbnailSize)
                                        .ConvertToImageSource();

            using (var driveIconBitmap = Resources.DriveIcon)
                DriveIcon = TypesFactory
                                        .ImageResizerInstance
                                        .CreateThumbnail(driveIconBitmap, 24)
                                        .ConvertToImageSource();

            using (var folderIconBitmap = Resources.FolderIcon)
                FolderIcon = TypesFactory
                                        .ImageResizerInstance
                                        .CreateThumbnail(folderIconBitmap, 24)
                                        .ConvertToImageSource();

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

        public static readonly ITypesFactory TypesFactory;

        #endregion
    }
}
