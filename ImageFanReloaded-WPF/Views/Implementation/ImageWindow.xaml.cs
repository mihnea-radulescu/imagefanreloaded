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
        
        public void SetImage(ImageSize screenSize, IImageFile imageFile)
        {
			_screenSize = screenSize; 
            _imageFile = imageFile;

            Title = _imageFile.FileName;

            _negligibleImageDragX = imageFile.ImageSize.Width * NegligibleImageDragFactor;
			_negligibleImageDragY = imageFile.ImageSize.Height * NegligibleImageDragFactor;

			_canZoomToImageSize = CanZoomToImageSize();
            _screenSizeCursor = GetScreenSizeCursor();

			ResizeToScreenSize();
        }

		#region Private

		private const double NegligibleImageDragFactor = 0.05;
		private const double DragRatioToImage = 0.10;

        private ImageSize _screenSize;
        private IImageFile _imageFile;

		private double _negligibleImageDragX, _negligibleImageDragY;

		private bool _canZoomToImageSize;
        private Cursor _screenSizeCursor;

        private Point _mouseDownCoordinates, _mouseUpCoordinates;
        private double _horizontalScrollOffset, _verticalScrollOffset;

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
			else if (keyPressed == Key.Escape)
            {
                if (_imageViewState == ImageViewState.ResizedToScreenSize)
                {
                    Close();
                }
                else if (_imageViewState == ImageViewState.ZoomedToImageSize)
                {
                    ResizeToScreenSize();
                }
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

			var newHorizontalScrollOffset = _horizontalScrollOffset +
				normalizedDragX * DragRatioToImage * _image.Source.Width;
			var newVerticalScrollOffset = _verticalScrollOffset +
				normalizedDragY * DragRatioToImage * _image.Source.Height;

            if (newHorizontalScrollOffset < 0)
            {
                newHorizontalScrollOffset = 0;
            }
            else if (newHorizontalScrollOffset > _image.Source.Width)
            {
                newHorizontalScrollOffset = _image.Source.Width;
            }
			if (newVerticalScrollOffset < 0)
			{
				newVerticalScrollOffset = 0;
			}
			else if (newVerticalScrollOffset > _image.Source.Height)
			{
				newVerticalScrollOffset = _image.Source.Height;
			}

            _horizontalScrollOffset = newHorizontalScrollOffset;
            _verticalScrollOffset = newVerticalScrollOffset;

			_imageScrollViewer.ScrollToHorizontalOffset(_horizontalScrollOffset);
			_imageScrollViewer.ScrollToVerticalOffset(_verticalScrollOffset);
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

			BeginInit();

			_image.Source = image;

            _imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            Cursor = _screenSizeCursor;

			_imageViewState = ImageViewState.ResizedToScreenSize;

			EndInit();
        }

        private void ZoomToImageSize(
            CoordinatesToImageSizeRatio coordinatesToImageSizeRatio)
        {
            var image = _imageFile.GetImage();

			BeginInit();

			_image.Source = image;

			_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            _horizontalScrollOffset = image.Width * coordinatesToImageSizeRatio.RatioX;
			_verticalScrollOffset = image.Height * coordinatesToImageSizeRatio.RatioY;

			_imageScrollViewer.ScrollToHorizontalOffset(_horizontalScrollOffset);
			_imageScrollViewer.ScrollToVerticalOffset(_verticalScrollOffset);

			Cursor = Cursors.SizeAll;

			_imageViewState = ImageViewState.ZoomedToImageSize;

			EndInit();
        }

        #endregion
    }
}
