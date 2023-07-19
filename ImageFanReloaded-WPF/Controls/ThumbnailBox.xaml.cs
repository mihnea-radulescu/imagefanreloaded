using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ImageFanReloadedWPF.CommonTypes.ImageHandling.Interface;
using ImageFanReloadedWPF.CommonTypes.Info;
using ImageFanReloadedWPF.Controls.Interface;

namespace ImageFanReloadedWPF.Controls
{
    public partial class ThumbnailBox
        : UserControl, IRefreshableControl
    {
        public ThumbnailBox(ThumbnailInfo thumbnailInfo)
        {
            if (thumbnailInfo == null)
            {
                throw new ArgumentNullException(nameof(thumbnailInfo));
            }

            InitializeComponent();

            SetControlProperties();
            SetThumbnailInfo(thumbnailInfo);
        }

        public event EventHandler<EventArgs> ThumbnailBoxClicked;

        public IImageFile ImageFile { get; private set; }
        public bool IsSelected { get; private set; }

        public void SelectThumbnail()
        {
            _thumbnailBoxBorder.BorderBrush = Brushes.Gray;
            Cursor = Cursors.Hand;
            IsSelected = true;

            BringIntoView();
        }

        public void UnselectThumbnail()
        {
            _thumbnailBoxBorder.BorderBrush = Brushes.LightGray;
            Cursor = Cursors.Arrow;
            IsSelected = false;
        }

        public void Refresh()
        {
            _thumbnailImage.Source = _thumbnailInfo.ThumbnailImage;
        }

        public void DisposeThumbnail()
        {
            ImageFile.DisposeThumbnailInput();
        }
        
        #region Private

        private ThumbnailInfo _thumbnailInfo;

        private void SetControlProperties()
        {
            _thumbnailImage.MaxWidth = GlobalData.ThumbnailSize;
            _thumbnailImage.MaxHeight = GlobalData.ThumbnailSize;
        }

        private void SetThumbnailInfo(ThumbnailInfo thumbnailInfo)
        {
            _thumbnailInfo = thumbnailInfo;
            ImageFile = _thumbnailInfo.ImageFile;

            _thumbnailImage.Source = _thumbnailInfo.ThumbnailImage;
            _thumbnailTextBlock.Text = _thumbnailInfo.ThumbnailText;
        }

        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            ThumbnailBoxClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
