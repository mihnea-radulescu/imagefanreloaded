using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.Controls.Implementation
{
    public partial class ThumbnailBox
        : UserControl, IRefreshableControl
    {
        public ThumbnailBox()
        {
            InitializeComponent();

            SetControlProperties();
        }

        public event EventHandler<EventArgs> ThumbnailBoxClicked;

        public IImageFile ImageFile { get; private set; }
        public bool IsSelected { get; private set; }

		public ThumbnailInfo ThumbnailInfo
		{
            get => _thumbnailInfo;

            set
            {
                _thumbnailInfo = value;

                ImageFile = _thumbnailInfo.ImageFile;

                _thumbnailImage.Source = _thumbnailInfo.ThumbnailImage;
                _thumbnailTextBlock.Text = _thumbnailInfo.ThumbnailText;
            }
		}

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
            _thumbnailImage.MaxWidth = GlobalData.ThumbnailSize.Width;
            _thumbnailImage.MaxHeight = GlobalData.ThumbnailSize.Height;
        }

        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            ThumbnailBoxClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
