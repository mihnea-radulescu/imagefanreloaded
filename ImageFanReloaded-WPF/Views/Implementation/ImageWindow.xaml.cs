using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views.Implementation
{
    public partial class ImageWindow
        : Window, IImageView
    {
        public event EventHandler<ThumbnailChangedEventArgs> ThumbnailChanged;
        
        public ImageWindow()
        {
            InitializeComponent();
        }
        
        public void SetImage(IImageFile imageFile)
        {
            _imageFile = imageFile;
            Title = _imageFile.FileName;

            if (_imageViewState == ImageViewState.FullScreen)
            {
                GoFullScreen();
            }
            else if (_imageViewState == ImageViewState.Detailed)
            {
                GoDetailed();
            }
        }

        #region Private

        private IImageFile _imageFile;
        private ImageViewState _imageViewState;

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var keyPressed = e.Key;

            if (GlobalData.BackwardNavigationKeys.Contains(keyPressed))
            {
                RaiseThumbnailChanged(-1);
            }
            else if (GlobalData.ForwardNavigationKeys.Contains(keyPressed))
            {
                RaiseThumbnailChanged(1);
            }
            else if (keyPressed == Key.Enter)
            {
                ChangeImageMode();
            }
            else if (keyPressed == Key.Escape)
            {
                if (_imageViewState == ImageViewState.FullScreen)
                {
                    Close();
                }
                else if (_imageViewState == ImageViewState.Detailed)
                {
                    GoFullScreen();
                }
            }
        }

        private void OnScrollKeyPressed(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            ChangeImageMode();
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_imageViewState == ImageViewState.FullScreen)
            {
                var delta = e.Delta;

                if (delta > 0)
                {
                    RaiseThumbnailChanged(-1);
                }
                else if (delta < 0)
                {
                    RaiseThumbnailChanged(1);
                }
            }
        }

        private void RaiseThumbnailChanged(int increment)
        {
            ThumbnailChanged?.Invoke(this, new ThumbnailChangedEventArgs(increment));
        }

        private void ChangeImageMode()
        {
            if (_imageViewState == ImageViewState.FullScreen)
            {
                GoDetailed();
            }
            else if (_imageViewState == ImageViewState.Detailed)
            {
                GoFullScreen();
            }
        }

        private void GoFullScreen()
        {
            BeginInit();

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            _imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            _imageViewState = ImageViewState.FullScreen;
            var fullScreenSize = this.GetScreenBounds();
            _image.Source = _imageFile.GetResizedImage(fullScreenSize);

            EndInit();
        }

        private void GoDetailed()
        {
            BeginInit();

            _imageScrollViewer.ScrollToHorizontalOffset(0);
            _imageScrollViewer.ScrollToVerticalOffset(0);

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.SingleBorderWindow;

            _imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            _imageViewState = ImageViewState.Detailed;
            _image.Source = _imageFile.GetImage();

            EndInit();
        }

        #endregion
    }
}
