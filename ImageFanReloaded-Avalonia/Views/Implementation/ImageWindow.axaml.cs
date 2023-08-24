using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
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
	}

	public void SetImage(ImageSize screenSize, IImageFile imageFile)
	{
		_screenSize = screenSize;
		_imageFile = imageFile;

		Title = _imageFile.FileName;

		_canZoomToImageSize = CanZoomToImageSize();
		_screenSizeCursor = GetScreenSizeCursor();

		ResizeToScreenSize();
	}

	#region Private

	private static readonly Cursor HandCursor;
	private static readonly Cursor NoneCursor;
	private static readonly Cursor SizeAllCursor;

	private ImageSize _screenSize;
	private IImageFile _imageFile;

	private bool _canZoomToImageSize;
	private Cursor _screenSizeCursor;

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

	private void OnMouseClick(object sender, PointerReleasedEventArgs e)
	{
		if (!_canZoomToImageSize)
		{
			return;
		}

		if (_imageViewState == ImageViewState.ResizedToScreenSize)
		{
			var mousePositionToImage = e.GetPosition(_image);
			var imageSize = new ImageSize(
				_image.Source.Size.Width, _image.Source.Size.Height);

			var coordinatesToImageSizeRatio =
				GetCoordinatesToImageSizeRatio(mousePositionToImage, imageSize);

			ZoomToImageSize(coordinatesToImageSizeRatio);
		}
		else if (_imageViewState == ImageViewState.ZoomedToImageSize)
		{
			ResizeToScreenSize();
		}
	}

	private void OnMouseWheel(object sender, PointerWheelEventArgs e)
	{
		if (_imageViewState == ImageViewState.ResizedToScreenSize)
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
		}
		else if (_imageViewState == ImageViewState.ZoomedToImageSize)
		{
			e.Handled = true;
		}
	}

	private void RaiseThumbnailChanged(int increment)
	{
		ThumbnailChanged?.Invoke(this, new ThumbnailChangedEventArgs(increment));
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

		BeginInit();

		_image.Source = image;

		_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
		_imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

		Cursor = _screenSizeCursor;

		_imageViewState = ImageViewState.ResizedToScreenSize;

		EndInit();
	}

	private void ZoomToImageSize(CoordinatesToImageSizeRatio coordinatesToImageSizeRatio)
	{
		var image = _imageFile.GetImage();

		BeginInit();

		_image.Source = image;

		_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
		_imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

		var horizontalOffset = image.Size.Width * coordinatesToImageSizeRatio.RatioX;
		var verticalOffset = image.Size.Height * coordinatesToImageSizeRatio.RatioY;

		_imageScrollViewer.Offset = new Vector(horizontalOffset, verticalOffset);

		Cursor = SizeAllCursor;

		_imageViewState = ImageViewState.ZoomedToImageSize;

		EndInit();
	}

	#endregion
}
