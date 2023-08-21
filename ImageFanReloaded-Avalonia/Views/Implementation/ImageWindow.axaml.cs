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

	private void OnMouseClick(object sender, PointerReleasedEventArgs e)
	{
		ChangeImageMode();
	}

	private void OnMouseWheel(object sender, PointerWheelEventArgs e)
	{
		if (_imageViewState == ImageViewState.FullScreen)
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
		var screenBounds = Screens.ScreenFromWindow(this).Bounds;
		var screenSize = new ImageSize(screenBounds.Width, screenBounds.Height);
		var image = _imageFile.GetResizedImage(screenSize);

		BeginInit();

		_image.Source = image;

		_imageViewState = ImageViewState.FullScreen;
		WindowState = WindowState.FullScreen;

		_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
		_imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

		EndInit();
	}

	private void GoDetailed()
	{
		var image = _imageFile.GetImage();

		BeginInit();

		_image.Source = image;

		_imageViewState = ImageViewState.Detailed;
		WindowState = WindowState.Maximized;

		_imageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
		_imageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
		_imageScrollViewer.Offset = new Vector(0, 0);

		EndInit();
	}

	#endregion
}
