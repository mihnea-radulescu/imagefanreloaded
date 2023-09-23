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

        public IScreenInformation ScreenInformation { private get; set; }

        public void SetImage(IImageFile imageFile)
        {
            _imageFile = imageFile;

            Title = _imageFile.FileName;

            _negligibleImageDragX = imageFile.ImageSize.Width * NegligibleImageDragFactor;
			_negligibleImageDragY = imageFile.ImageSize.Height * NegligibleImageDragFactor;

			_screenSize = ScreenInformation.GetScreenSize();

			_canZoomToImageSize = CanZoomToImageSize();
            _screenSizeCursor = GetScreenSizeCursor();

			ResizeToScreenSize();
        }

		#region Private

		private const double ImageZoomScalingFactor = 0.1;
		private const double ImageScrollFactor = 0.1;
		private const double NegligibleImageDragFactor = 0.05;

        private IImageFile _imageFile;

		private double _negligibleImageDragX, _negligibleImageDragY;

		private ImageSize _screenSize;
		private bool _canZoomToImageSize;
        private Cursor _screenSizeCursor;

        private Point _mouseDownCoordinates, _mouseUpCoordinates;

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
            else if (keyPressed == GlobalData.EnterKey)
			{
				if (!_canZoomToImageSize)
                {
                    return;
                }

				if (_imageViewState == ImageViewState.ResizedToScreenSize)
				{
					ZoomToImageSize(CoordinatesToImageSizeRatio.ImageCenter);
				}
				else if (_imageViewState == ImageViewState.ZoomedToImageSize)
				{
					ResizeToScreenSize();
				}
			}
			else if (keyPressed == GlobalData.EscapeKey)
            {
				Close();
			}
        }

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
			if (!_canZoomToImageSize ||
                _imageViewState == ImageViewState.ResizedToScreenSize)
			{
				return;
			}

            _mouseDownCoordinates = e.GetPosition(_image);
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
			if (_imageViewState == ImageViewState.ResizedToScreenSize)
			{
				if (e.ChangedButton == MouseButton.Left && _canZoomToImageSize)
				{
					var mousePositionToImage = e.GetPosition(_image);
					var imageSize = new ImageSize(_image.Source.Width, _image.Source.Height);

					var coordinatesToImageSizeRatio =
						GetCoordinatesToImageSizeRatio(mousePositionToImage, imageSize);

					ZoomToImageSize(coordinatesToImageSizeRatio);
				}
				else if (e.ChangedButton == MouseButton.Right)
				{
					Close();
				}
			}
			else if (_imageViewState == ImageViewState.ZoomedToImageSize)
			{
				if (e.ChangedButton == MouseButton.Left)
				{
					_mouseUpCoordinates = e.GetPosition(_image);

					if (_mouseDownCoordinates == _mouseUpCoordinates)
					{
						ResizeToScreenSize();
					}
					else
					{
						DragImage();
					}
				}
				else if (e.ChangedButton == MouseButton.Right)
				{
					Close();
				}
			}
		}

		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
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

            e.Handled = true;
		}

        private void RaiseThumbnailChanged(int increment)
        {
            ThumbnailChanged?.Invoke(this, new ThumbnailChangedEventArgs(increment));
        }

        private void DragImage()
        {
            var dragX = _mouseUpCoordinates.X - _mouseDownCoordinates.X;
			var dragY = _mouseUpCoordinates.Y - _mouseDownCoordinates.Y;

            double normalizedDragX;
			if (Math.Abs(dragX) < _negligibleImageDragX)
            {
                normalizedDragX = 0;
            }
            else
            {
				normalizedDragX = dragX >= 0 ? -1 : 1;
			}

			double normalizedDragY;
			if (Math.Abs(dragY) < _negligibleImageDragY)
			{
				normalizedDragY = 0;
			}
			else
			{
				normalizedDragY = dragY >= 0 ? -1 : 1;
			}

			var newHorizontalScrollOffset = _imageScrollViewer.HorizontalOffset +
				normalizedDragX * ImageScrollFactor * _image.Source.Width;
			var newVerticalScrollOffset = _imageScrollViewer.VerticalOffset +
				normalizedDragY * ImageScrollFactor * _image.Source.Height;

            if (newHorizontalScrollOffset < 0)
            {
                newHorizontalScrollOffset = 0;
            }
            else if (newHorizontalScrollOffset > _imageScrollViewer.ScrollableWidth)
            {
                newHorizontalScrollOffset = _imageScrollViewer.ScrollableWidth;
            }
			if (newVerticalScrollOffset < 0)
			{
				newVerticalScrollOffset = 0;
			}
			else if (newVerticalScrollOffset > _imageScrollViewer.ScrollableHeight)
			{
				newVerticalScrollOffset = _imageScrollViewer.ScrollableHeight;
			}

			_imageScrollViewer.ScrollToHorizontalOffset(newHorizontalScrollOffset);
			_imageScrollViewer.ScrollToVerticalOffset(newVerticalScrollOffset);
		}

		private CoordinatesToImageSizeRatio GetCoordinatesToImageSizeRatio(
			Point mousePositionToImage, ImageSize imageSize)
		{
			CoordinatesToImageSizeRatio coordinatesToImageSizeRatio;

			if (mousePositionToImage.X >= 0 &&
				mousePositionToImage.X <= _image.Source.Width &&
				mousePositionToImage.Y >= 0 &&
				mousePositionToImage.Y <= _image.Source.Height)
			{
				coordinatesToImageSizeRatio =
					new CoordinatesToImageSizeRatio(mousePositionToImage, imageSize);
			}
			else
			{
				coordinatesToImageSizeRatio = CoordinatesToImageSizeRatio.ImageCenter;
			}

			return coordinatesToImageSizeRatio;
		}

		private bool CanZoomToImageSize()
        {
			var canZoomToImageSize =
                _imageFile.ImageSize.Width > _screenSize.Width ||
				_imageFile.ImageSize.Height > _screenSize.Height;

            return canZoomToImageSize;
        }

        private Cursor GetScreenSizeCursor()
        {
            Cursor screenSizeCursor;
            
            if (_canZoomToImageSize)
            {
                screenSizeCursor = Cursors.Hand;
            }
            else
            {
                screenSizeCursor = Cursors.None;
            }

            return screenSizeCursor;
        }

		private void ResizeToScreenSize()
        {
			var image = _imageFile.GetResizedImage(_screenSize);

			_imageViewState = ImageViewState.ResizedToScreenSize;

			BeginInit();

			_image.Source = image;

            _imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            Cursor = _screenSizeCursor;

			EndInit();
        }

        private void ZoomToImageSize(
            CoordinatesToImageSizeRatio coordinatesToImageSizeRatio)
        {
            var image = _imageFile.GetImage();

			var zoomToX = image.Width * coordinatesToImageSizeRatio.RatioX;
			var zoomToY = image.Height * coordinatesToImageSizeRatio.RatioY;

			var zoomRectangle = new Rect(
				zoomToX - (ImageZoomScalingFactor * image.Width),
				zoomToY - (ImageZoomScalingFactor * image.Height),
				2 * (ImageZoomScalingFactor * image.Width),
				2 * (ImageZoomScalingFactor * image.Height));

			_imageViewState = ImageViewState.ZoomedToImageSize;

			BeginInit();

			_image.Source = image;

			_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

			_image.BringIntoView(zoomRectangle);

			Cursor = Cursors.SizeAll;

			EndInit();
        }

        #endregion
    }
}
