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
			UpdateViewState();
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
		UpdateViewState();
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
	}

	private void RaiseThumbnailChanged(int increment)
	{
		ThumbnailChanged?.Invoke(this, new ThumbnailChangedEventArgs(increment));
	}

	private void UpdateViewState()
	{
		if (_imageViewState == ImageViewState.ResizedToScreenSize &&
			_canZoomToImageSize)
		{
			ZoomToImageSize();
		}
		else if (_imageViewState == ImageViewState.ZoomedToImageSize)
		{
			ResizeToScreenSize();
		}
	}

	private bool CanZoomToImageSize()
	{
		var shouldZoomToImageSize =
			_imageFile.ImageSize.Width > _screenSize.Width ||
			_imageFile.ImageSize.Height > _screenSize.Height;

		return shouldZoomToImageSize;
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

	private void ZoomToImageSize()
	{
		var image = _imageFile.GetImage();

		BeginInit();

		_image.Source = image;

		_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
		_imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
		_imageScrollViewer.Offset = new Vector(0, 0);

		Cursor = SizeAllCursor;

		_imageViewState = ImageViewState.ZoomedToImageSize;

		EndInit();
	}

	#endregion
}
