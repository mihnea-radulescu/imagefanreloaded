using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.Keyboard;

namespace ImageFanReloaded.Controls;

public partial class ImageWindow : Window, IImageView
{
	static ImageWindow()
	{
		HandCursor = new Cursor(StandardCursorType.Hand);
		NoneCursor = new Cursor(StandardCursorType.None);
		SizeAllCursor = new Cursor(StandardCursorType.SizeAll);
	}
	
	public ImageWindow()
	{
		InitializeComponent();

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
		AddHandler(PointerWheelChangedEvent, OnMouseWheel, RoutingStrategies.Tunnel);
	}
	
	public IGlobalParameters? GlobalParameters
	{
		get => _globalParameters;
		set
		{
			_globalParameters = value;
			_invalidImage = _globalParameters!.InvalidImage.GetBitmap();
		}
	}
	
	public IScreenInformation? ScreenInformation { get; set; }
	
	public event EventHandler<ImageViewClosingEventArgs>? ViewClosing;
	
	public event EventHandler<ImageChangedEventArgs>? ImageChanged;

	public void SetImage(IImageFile imageFile, bool recursiveFolderAccess)
	{
		_imageFile = imageFile;

		Title = _imageFile.ImageFileName;

		_negligibleImageDragX = imageFile.ImageSize.Width * NegligibleImageDragFactor;
		_negligibleImageDragY = imageFile.ImageSize.Height * NegligibleImageDragFactor;

		_screenSize = ScreenInformation!.GetScreenSize();

		_canZoomToImageSize = CanZoomToImageSize();
		_screenSizeCursor = GetScreenSizeCursor();

		_textBlockImageInfo.Text = _imageFile.GetImageInfo(recursiveFolderAccess);

		_showMainViewAfterImageViewClosing = false;

		ResizeToScreenSize();
	}

	public Task ShowDialog(IMainView owner) => ShowDialog((Window)owner);

	#region Private

	private const double ImageZoomScalingFactor = 0.1;
	private const double ImageScrollFactor = 0.1;
	private const double NegligibleImageDragFactor = 0.025;

	private static readonly Cursor HandCursor;
	private static readonly Cursor NoneCursor;
	private static readonly Cursor SizeAllCursor;
	
	private IGlobalParameters? _globalParameters;
	private Bitmap? _invalidImage;

	private IImageFile? _imageFile;

	private double _negligibleImageDragX, _negligibleImageDragY;

	private ImageSize? _screenSize;

	private bool _canZoomToImageSize;
	private Cursor? _screenSizeCursor;

	private Point _mouseDownCoordinates, _mouseUpCoordinates;

	private ImageViewState _imageViewState;

	private bool _showMainViewAfterImageViewClosing;

    private void OnKeyPressing(object? sender, KeyEventArgs e)
    {
        var keyPressing = e.Key.ToCoreKey();

        if (_globalParameters!.BackwardNavigationKeys.Contains(keyPressing))
        {
            RaiseImageChanged(-1);
        }
        else if (_globalParameters!.ForwardNavigationKeys.Contains(keyPressing))
        {
            RaiseImageChanged(1);
        }
        else if (keyPressing == _globalParameters!.EnterKey)
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
        else if (keyPressing == _globalParameters!.IKey)
        {
	        ToggleImageInfoVisibility();
        }
        else if (keyPressing == _globalParameters!.EscapeKey)
        {
	        CloseWindow();
        }
        else if (keyPressing == _globalParameters!.TKey)
        {
	        _showMainViewAfterImageViewClosing = true;
	        
	        CloseWindow();
        }

        e.Handled = true;
    }

    private void OnMouseDown(object? sender, PointerPressedEventArgs e)
	{
		if (!_canZoomToImageSize || _imageViewState == ImageViewState.ResizedToScreenSize)
		{
			return;
		}

		_mouseDownCoordinates = e.GetPosition(_imageControl);
	}

	private void OnMouseUp(object? sender, PointerReleasedEventArgs e)
	{
		if (_imageViewState == ImageViewState.ResizedToScreenSize)
		{
			if (e.InitialPressMouseButton == MouseButton.Left && _canZoomToImageSize)
			{
				var mousePositionToImage = e.GetPosition(_imageControl);
				var imageSize = new ImageSize(
					_imageControl.Source!.Size.Width, _imageControl.Source!.Size.Height);

				var coordinatesToImageSizeRatio =
					GetCoordinatesToImageSizeRatio(mousePositionToImage, imageSize);

				ZoomToImageSize(coordinatesToImageSizeRatio);
			}
			else if (e.InitialPressMouseButton == MouseButton.Right)
			{
				CloseWindow();
			}
		}
		else if (_imageViewState == ImageViewState.ZoomedToImageSize)
		{
			if (e.InitialPressMouseButton == MouseButton.Left)
			{
				_mouseUpCoordinates = e.GetPosition(_imageControl);

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
				CloseWindow();
			}
		}
	}

	private void OnMouseWheel(object? sender, PointerWheelEventArgs e)
	{
		var delta = e.Delta;

		if (delta.Y > 0)
		{
			RaiseImageChanged(-1);
		}
		else if (delta.Y < 0)
		{
			RaiseImageChanged(1);
		}

		e.Handled = true;
	}
	
	private void OnClosing(object? sender, WindowClosingEventArgs e)
	{
		ViewClosing?.Invoke(this, new ImageViewClosingEventArgs(_showMainViewAfterImageViewClosing));
	}

	private void RaiseImageChanged(int increment)
	{
		ImageChanged?.Invoke(this, new ImageChangedEventArgs(this, increment));
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
			normalizedDragX * ImageScrollFactor * _imageControl.Source!.Size.Width;
		var newVerticalScrollOffset = _imageScrollViewer.Offset.Y +
			normalizedDragY * ImageScrollFactor * _imageControl.Source!.Size.Height;

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

		var mousePoint = new ImagePoint(
			(int)mousePositionToImage.X, (int)mousePositionToImage.Y);

		if (mousePoint.X >= 0 &&
		    mousePoint.X <= _imageControl.Source!.Size.Width &&
		    mousePoint.Y >= 0 &&
		    mousePoint.Y <= _imageControl.Source!.Size.Height)
		{
			coordinatesToImageSizeRatio =
				new CoordinatesToImageSizeRatio(mousePoint, imageSize);
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
			_imageFile!.ImageSize.Width > _screenSize!.Width ||
			_imageFile!.ImageSize.Height > _screenSize!.Height;

		return canZoomToImageSize;
	}

	private Cursor GetScreenSizeCursor() 
		=> _canZoomToImageSize ? HandCursor : NoneCursor;

	private void ResizeToScreenSize()
	{
		var image = _imageFile!.GetResizedImage(_screenSize!);
		var bitmap = image.GetBitmap();
		SetImageSource(bitmap);

		_imageViewState = ImageViewState.ResizedToScreenSize;
		Cursor = _screenSizeCursor;
	}

	private void ZoomToImageSize(CoordinatesToImageSizeRatio coordinatesToImageSizeRatio)
	{
		var image = _imageFile!.GetImage();
		var bitmap = image.GetBitmap();
		SetImageSource(bitmap);

		_imageViewState = ImageViewState.ZoomedToImageSize;
		Cursor = SizeAllCursor;

		var zoomRectangle = GetZoomRectangle(coordinatesToImageSizeRatio, bitmap);
		_imageControl.BringIntoView(zoomRectangle);
	}

	private static Rect GetZoomRectangle(
		CoordinatesToImageSizeRatio coordinatesToImageSizeRatio, Bitmap image)
	{
		var zoomToX = image.Size.Width * coordinatesToImageSizeRatio.RatioX;
		var zoomToY = image.Size.Height * coordinatesToImageSizeRatio.RatioY;

		var zoomRectangle = new Rect(
			zoomToX - (ImageZoomScalingFactor * image.Size.Width),
			zoomToY - (ImageZoomScalingFactor * image.Size.Height),
			2 * (ImageZoomScalingFactor * image.Size.Width),
			2 * (ImageZoomScalingFactor * image.Size.Height));

		return zoomRectangle;
	}
	
	private void ToggleImageInfoVisibility()
	{
		_textBlockImageInfo.IsVisible = !_textBlockImageInfo.IsVisible;
	}

	private void CloseWindow()
	{
		Close();

		SetImageSource(null);
	}

	private void SetImageSource(Bitmap? image)
	{
		var previousImageSource = _imageControl.Source as Bitmap;

		_imageControl.Source = image;

		if (previousImageSource is not null &&
			previousImageSource != _invalidImage)
		{
			previousImageSource.Dispose();
		}
	}

	#endregion
}
