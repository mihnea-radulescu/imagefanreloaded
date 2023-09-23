using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views.Implementation;

public partial class ImageWindow
	: Window, IImageView
{
	static ImageWindow()
	{
		HandCursor = new Cursor(StandardCursorType.Hand);
		NoneCursor = new Cursor(StandardCursorType.None);
		SizeAllCursor = new Cursor(StandardCursorType.SizeAll);
	}
	
	public event EventHandler<ThumbnailChangedEventArgs> ThumbnailChanged;

	public ImageWindow()
	{
		InitializeComponent();

		AddHandler(PointerWheelChangedEvent, OnMouseWheel, RoutingStrategies.Tunnel);
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

	private static readonly Cursor HandCursor;
	private static readonly Cursor NoneCursor;
	private static readonly Cursor SizeAllCursor;

	private ImageSize _screenSize;
	private IImageFile _imageFile;

	private double _negligibleImageDragX, _negligibleImageDragY;

	private bool _canZoomToImageSize;
	private Cursor _screenSizeCursor;

	private Point _mouseDownCoordinates, _mouseUpCoordinates;

	private ImageViewState _imageViewState;

	private void OnMouseDown(object sender, PointerPressedEventArgs e)
	{
		if (!_canZoomToImageSize ||
			_imageViewState == ImageViewState.ResizedToScreenSize)
		{
			return;
		}

		_mouseDownCoordinates = e.GetPosition(_image);
	}

	private void OnMouseUp(object sender, PointerReleasedEventArgs e)
	{
		if (_imageViewState == ImageViewState.ResizedToScreenSize)
		{
			if (e.InitialPressMouseButton == MouseButton.Left && _canZoomToImageSize)
			{
				var mousePositionToImage = e.GetPosition(_image);
				var imageSize = new ImageSize(
					_image.Source.Size.Width, _image.Source.Size.Height);

				var coordinatesToImageSizeRatio =
					GetCoordinatesToImageSizeRatio(mousePositionToImage, imageSize);

				ZoomToImageSize(coordinatesToImageSizeRatio);
			}
			else if (e.InitialPressMouseButton == MouseButton.Right)
			{
				Close();
			}
		}
		else if (_imageViewState == ImageViewState.ZoomedToImageSize)
		{
			if (e.InitialPressMouseButton == MouseButton.Left)
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
			else if (e.InitialPressMouseButton == MouseButton.Right)
			{
				Close();
			}
		}
	}

	private void OnMouseWheel(object sender, PointerWheelEventArgs e)
	{
		var delta = e.Delta;

		if (delta.Y > 0)
		{
			RaiseThumbnailChanged(-1);
		}
		else if (delta.Y < 0)
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

		var newHorizontalScrollOffset = _imageScrollViewer.Offset.X +
			normalizedDragX * ImageScrollFactor * _image.Source.Size.Width;
		var newVerticalScrollOffset = _imageScrollViewer.Offset.Y +
			normalizedDragY * ImageScrollFactor * _image.Source.Size.Height;

		if (newHorizontalScrollOffset < 0)
		{
			newHorizontalScrollOffset = 0;
		}
		else if (newHorizontalScrollOffset > _imageScrollViewer.ScrollBarMaximum.X)
		{
			newHorizontalScrollOffset = _imageScrollViewer.ScrollBarMaximum.X;
		}
		if (newVerticalScrollOffset < 0)
		{
			newVerticalScrollOffset = 0;
		}
		else if (newVerticalScrollOffset > _imageScrollViewer.ScrollBarMaximum.Y)
		{
			newVerticalScrollOffset = _imageScrollViewer.ScrollBarMaximum.Y;
		}

		_imageScrollViewer.Offset = new Vector(
			newHorizontalScrollOffset, newVerticalScrollOffset);
	}

	private CoordinatesToImageSizeRatio GetCoordinatesToImageSizeRatio(
			Point mousePositionToImage, ImageSize imageSize)
	{
		CoordinatesToImageSizeRatio coordinatesToImageSizeRatio;

		if (mousePositionToImage.X >= 0 &&
			mousePositionToImage.X <= _image.Source.Size.Width &&
			mousePositionToImage.Y >= 0 &&
			mousePositionToImage.Y <= _image.Source.Size.Height)
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
			screenSizeCursor = HandCursor;
		}
		else
		{
			screenSizeCursor = NoneCursor;
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

	private void ZoomToImageSize(CoordinatesToImageSizeRatio coordinatesToImageSizeRatio)
	{
		var image = _imageFile.GetImage();

		var zoomToX = image.Size.Width * coordinatesToImageSizeRatio.RatioX;
		var zoomToY = image.Size.Height * coordinatesToImageSizeRatio.RatioY;

		var zoomRectangle = new Rect(
			zoomToX - (ImageZoomScalingFactor * image.Size.Width),
			zoomToY - (ImageZoomScalingFactor * image.Size.Height),
			2 * (ImageZoomScalingFactor * image.Size.Width),
			2 * (ImageZoomScalingFactor * image.Size.Height));

		_imageViewState = ImageViewState.ZoomedToImageSize;

		BeginInit();

		_image.Source = image;

		_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
		_imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

		_image.BringIntoView(zoomRectangle);

		Cursor = SizeAllCursor;

		EndInit();
	}

	#endregion
}
