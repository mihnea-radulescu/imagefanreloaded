using System;
using System.Diagnostics;
using System.Windows.Media;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Infrastructure;

namespace ImageFanReloaded.CommonTypes.Info
{
    [DebuggerDisplay("{ThumbnailText}")]
    public class ThumbnailInfo
    {
        public ThumbnailInfo(IVisualActionDispatcher dispatcher, IImageFile imageFile)
        {
            _dispatcher = dispatcher;
            ImageFile = imageFile ?? throw new ArgumentNullException(nameof(imageFile));

            ThumbnailImage = GlobalData.LoadingImageThumbnail;
        }

        public IRefreshableControl ThumbnailBox { get; set; }

        public IImageFile ImageFile { get; private set; }

        public ImageSource ThumbnailImage { get; set; }
        public string ThumbnailText => ImageFile.FileName;

        public void ReadThumbnailInputFromDisc()
            => ImageFile.ReadThumbnailInputFromDisc();

        public void SaveThumbnail()
        {
            try
            {
                SaveThumbnailHelper();
            }
            catch (InvalidOperationException)
            {
                _dispatcher.Invoke(() => SaveThumbnailHelper());
            }
        }

        public void RefreshThumbnail()
            => ThumbnailBox.Refresh();

        #region Private

        private readonly IVisualActionDispatcher _dispatcher;

        private void SaveThumbnailHelper()
        {
            ThumbnailImage = ImageFile.GetThumbnail();
            ThumbnailImage.Freeze();
        }

        #endregion
    }
}
