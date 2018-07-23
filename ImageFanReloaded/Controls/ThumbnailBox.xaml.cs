using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Infrastructure.Interface;

namespace ImageFanReloaded.Controls
{
    public partial class ThumbnailBox
        : UserControl
    {
        public ThumbnailBox(IVisualActionDispatcher dispatcher, ThumbnailInfo thumbnailInfo)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

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

        public void DisposeThumbnail()
        {
            ImageFile.Dispose();
        }

        #region Private

        private readonly IVisualActionDispatcher _dispatcher;

        private ThumbnailInfo _thumbnailInfo;

        private void SetControlProperties()
        {
            _thumbnailImage.MaxWidth = GlobalData.ThumbnailSize;
            _thumbnailImage.MaxHeight = GlobalData.ThumbnailSize;
        }

        private void SetThumbnailInfo(ThumbnailInfo thumbnailInfo)
        {
            if (_thumbnailInfo != null)
            {
                _thumbnailInfo.ThumbnailImageChanged -= OnThumbnailImageChanged;
            }

            _thumbnailInfo = thumbnailInfo;
            ImageFile = _thumbnailInfo.ImageFile;

            _thumbnailImage.Source = _thumbnailInfo.ThumbnailImage;
            _thumbnailTextBlock.Text = _thumbnailInfo.ThumbnailText;

            _thumbnailInfo.ThumbnailImageChanged += OnThumbnailImageChanged;
        }

        private void OnThumbnailImageChanged(object sender, EventArgs e)
        {
            _dispatcher.Invoke(() => _thumbnailImage.Source = _thumbnailInfo.ThumbnailImage);
        }

        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            ThumbnailBoxClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
