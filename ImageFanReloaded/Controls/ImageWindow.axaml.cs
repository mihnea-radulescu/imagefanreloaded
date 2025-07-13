using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling.Extensions;
using ImageFanReloaded.Keyboard;

namespace ImageFanReloaded.Controls;

public partial class ImageWindow : Window, IImageView
{
	static ImageWindow()
	{
		ArrowCursor = new Cursor(StandardCursorType.Arrow);
		HandCursor = new Cursor(StandardCursorType.Hand);
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
			_invalidImage = _globalParameters!.InvalidImage;
		}
	}

	public IScreenInformation? ScreenInformation { get; set; }

	public ITabOptions? TabOptions { get; set; }

	public event EventHandler<ImageViewClosingEventArgs>? ViewClosing;
	public event EventHandler<ImageChangedEventArgs>? ImageChanged;

	public bool CanAdvanceToDesignatedImage { get; set; }

	public async Task<bool> CanStartSlideshowFromContentTabItem()
		=> await Dispatcher.UIThread.InvokeAsync(() => IsVisible);

	public async Task StartSlideshowFromContentTabItem()
		=> await Dispatcher.UIThread.InvokeAsync(StartSlideshow);

	public async Task SetImage(IImageFile imageFile)
	{
		_imageFile = imageFile;

		Title = _imageFile.ImageFileData.ImageFileName;

		_negligibleImageDragX = imageFile.ImageSize.Width * NegligibleImageDragFactor;
		_negligibleImageDragY = imageFile.ImageSize.Height * NegligibleImageDragFactor;

		_screenSize = ScreenInformation!.GetScreenSize();

		_canZoomToImageSize = CanZoomToImageSize();
		_screenSizeCursor = GetScreenSizeCursor();

		_imageInfoTextBox.Text = _imageFile.GetBasicImageInfo(TabOptions!.RecursiveFolderBrowsing);
		_imageInfoTextBox.IsVisible = TabOptions!.ShowImageViewImageInfo;

		_showMainViewAfterImageViewClosing = false;

		await ResizeToScreenSize();
	}

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private const double ImageZoomScalingFactor = 0.1;
	private const double ImageScrollFactor = 0.1;
	private const double NegligibleImageDragFactor = 0.025;

	private const int OneImageForward = 1;
	private const int OneImageBackward = -1;

	private static readonly Cursor ArrowCursor;
	private static readonly Cursor HandCursor;
	private static readonly Cursor SizeAllCursor;

	private CancellationTokenSource? _ctsAnimation;
	private CancellationTokenSource? _ctsSlideshow;

	private Task? _animationTask;

	private IImage? _previousImage;

	private IGlobalParameters? _globalParameters;
	private IImage? _invalidImage;

	private IImageFile? _imageFile;

	private double _negligibleImageDragX, _negligibleImageDragY;

	private ImageSize? _screenSize;

	private bool _canZoomToImageSize;
	private Cursor? _screenSizeCursor;

	private Point _mouseDownCoordinates, _mouseUpCoordinates;

	private ImageViewState _imageViewState;

	private bool _showMainViewAfterImageViewClosing;

	private void NotifyStopAnimation() => _ctsAnimation?.Cancel();
	private void NotifyStopSlideshow() => _ctsSlideshow?.Cancel();

	private async void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		NotifyStopSlideshow();

		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldStartSlideshow(keyModifiers, keyPressing))
		{
			await StartSlideshow();
		}
		else if (ShouldHandleImageDrag(keyModifiers))
		{
			if (keyPressing == _globalParameters!.UpKey)
			{
				DragImage(0, -1);
			}
			else if (keyPressing == _globalParameters!.DownKey)
			{
				DragImage(0, 1);
			}
			else if (keyPressing == _globalParameters!.LeftKey)
			{
				DragImage(-1, 0);
			}
			else if (keyPressing == _globalParameters!.RightKey)
			{
				DragImage(1, 0);
			}
		}
		else if (ShouldHandleBackwardNavigation(keyModifiers, keyPressing))
		{
			await RaiseImageChanged(OneImageBackward, false);
		}
		else if (ShouldHandleForwardNavigation(keyModifiers, keyPressing))
		{
			await RaiseImageChanged(OneImageForward, false);
		}
		else if (ShouldHandleImageZoom(keyModifiers, keyPressing))
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
				await ResizeToScreenSize();
			}
		}
		else if (ShouldToggleImageInfo(keyModifiers, keyPressing))
		{
			ToggleImageInfo();
		}
		else if (ShouldHandleEscapeAction(keyModifiers, keyPressing))
		{
			await HandleEscapeAction();
		}
		else if (ShouldHandleWindowClose(keyModifiers, keyPressing))
		{
			_showMainViewAfterImageViewClosing = true;

			await CloseWindow();
		}

		e.Handled = true;
	}

	private void OnMouseDown(object? sender, PointerPressedEventArgs e)
	{
		NotifyStopSlideshow();

		if (!_canZoomToImageSize || _imageViewState == ImageViewState.ResizedToScreenSize)
		{
			return;
		}

		_mouseDownCoordinates = e.GetPosition(_imageControl);
	}

	private async void OnMouseUp(object? sender, PointerReleasedEventArgs e)
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
				await HandleEscapeAction();
			}
		}
		else if (_imageViewState == ImageViewState.ZoomedToImageSize)
		{
			if (e.InitialPressMouseButton == MouseButton.Left)
			{
				_mouseUpCoordinates = e.GetPosition(_imageControl);

				if (_mouseDownCoordinates == _mouseUpCoordinates)
				{
					await ResizeToScreenSize();
				}
				else
				{
					var (normalizedDragX, normalizedDragY) = GetNormalizedDrag();
					DragImage(normalizedDragX, normalizedDragY);
				}
			}
			else if (e.InitialPressMouseButton == MouseButton.Right)
			{
				await HandleEscapeAction();
			}
		}
	}

	private async void OnMouseWheel(object? sender, PointerWheelEventArgs e)
	{
		NotifyStopSlideshow();

		var delta = e.Delta;

		if (delta.Y > 0)
		{
			await RaiseImageChanged(OneImageBackward, false);
		}
		else if (delta.Y < 0)
		{
			await RaiseImageChanged(OneImageForward, false);
		}

		e.Handled = true;
	}

	private void OnClosing(object? sender, WindowClosingEventArgs e)
	{
		ViewClosing?.Invoke(this, new ImageViewClosingEventArgs(_showMainViewAfterImageViewClosing));
	}

	private bool ShouldStartSlideshow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier && keyPressing == _globalParameters!.SKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleImageDrag(ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers)
	{
		if (keyModifiers == _globalParameters!.CtrlKeyModifier && _imageViewState == ImageViewState.ZoomedToImageSize)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleBackwardNavigation(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			_globalParameters!.IsBackwardNavigationKey(keyPressing))
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleForwardNavigation(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			_globalParameters!.IsForwardNavigationKey(keyPressing))
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleImageZoom(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.EnterKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldToggleImageInfo(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.IKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleEscapeAction(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.EscapeKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleWindowClose(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.TKey)
		{
			return true;
		}

		return false;
	}

	private async Task HandleEscapeAction()
	{
		if (_imageInfoTextBox.IsFocused)
		{
			Focus();
		}
		else
		{
			await CloseWindow();
		}
	}

	private async Task RaiseImageChanged(int increment, bool isSlideshow)
	{
		NotifyStopAnimation();

		ImageChanged?.Invoke(this, new ImageChangedEventArgs(this, increment));

		if (!isSlideshow && !CanAdvanceToDesignatedImage)
		{
			await CloseWindow();
		}
	}

	private async Task StartSlideshow()
	{
		var slideshowInterval = TimeSpan.FromSeconds(TabOptions!.SlideshowInterval.ToInt());

		try
		{
			_ctsSlideshow = new CancellationTokenSource();

			await PauseBetweenImages(slideshowInterval);

			if (_ctsSlideshow.IsCancellationRequested)
			{
				return;
			}

			do
			{
				if (_ctsSlideshow.IsCancellationRequested)
				{
					return;
				}

				await RaiseImageChanged(OneImageForward, true);

				if (_ctsSlideshow.IsCancellationRequested)
				{
					return;
				}

				await PauseBetweenImages(slideshowInterval);
			} while (CanAdvanceToDesignatedImage);

			await CloseWindow();
		}
		catch
		{
		}
	}

	private async Task AnimateImage(IImage image, CancellationTokenSource ctsAnimation)
	{
		try
		{
			while (!ctsAnimation.IsCancellationRequested)
			{
				var thumbnailImageFrames = image.ImageFrames;
				foreach (var aThumbnailImageFrame in thumbnailImageFrames)
				{
					if (ctsAnimation.IsCancellationRequested)
					{
						break;
					}

					var anImageFrameBitmap = aThumbnailImageFrame.GetBitmap();

					if (ctsAnimation.IsCancellationRequested)
					{
						break;
					}

					await Dispatcher.UIThread.InvokeAsync(()
						=> _imageControl.Source = anImageFrameBitmap);

					if (ctsAnimation.IsCancellationRequested)
					{
						break;
					}

					await Task.Delay(aThumbnailImageFrame.DelayUntilNextFrame, ctsAnimation.Token);
				}
			}
		}
		catch
		{
		}
	}

	private async Task ResizeToScreenSize()
	{
		var image = _imageFile!.GetResizedImage(_screenSize!, TabOptions!.ApplyImageOrientation);

		_imageViewState = ImageViewState.ResizedToScreenSize;
		Cursor = _screenSizeCursor;

		await WaitForAnimationTask();

		if (image.IsAnimated)
		{
			_ctsAnimation = new CancellationTokenSource();
			_animationTask = Task.Run(() => AnimateImage(image, _ctsAnimation));
		}
		else
		{
			SetImageSource(image);
		}
	}

	private (double, double) GetNormalizedDrag()
	{
		var dragX = _mouseUpCoordinates.X - _mouseDownCoordinates.X;

		double normalizedDragX;
		if (Math.Abs(dragX) < _negligibleImageDragX)
		{
			normalizedDragX = 0;
		}
		else
		{
			normalizedDragX = dragX >= 0 ? -1 : 1;
		}

		var dragY = _mouseUpCoordinates.Y - _mouseDownCoordinates.Y;

		double normalizedDragY;
		if (Math.Abs(dragY) < _negligibleImageDragY)
		{
			normalizedDragY = 0;
		}
		else
		{
			normalizedDragY = dragY >= 0 ? -1 : 1;
		}

		return (normalizedDragX, normalizedDragY);
	}

	private void DragImage(double normalizedDragX, double normalizedDragY)
	{
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

		_imageScrollViewer.Offset = new Vector(newHorizontalScrollOffset, newVerticalScrollOffset);
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
			!_imageFile!.IsAnimatedImage &&
			(_imageFile!.ImageSize.Width > _screenSize!.Width ||
			 _imageFile!.ImageSize.Height > _screenSize!.Height);

		return canZoomToImageSize;
	}

	private Cursor GetScreenSizeCursor()
		=> _canZoomToImageSize ? HandCursor : ArrowCursor;

	private void ZoomToImageSize(CoordinatesToImageSizeRatio coordinatesToImageSizeRatio)
	{
		var image = _imageFile!.GetImage(TabOptions!.ApplyImageOrientation);
		SetImageSource(image);

		_imageViewState = ImageViewState.ZoomedToImageSize;
		Cursor = SizeAllCursor;

		var zoomRectangle = GetZoomRectangle(coordinatesToImageSizeRatio, image);
		_imageControl.BringIntoView(zoomRectangle);
	}

	private static Rect GetZoomRectangle(
		CoordinatesToImageSizeRatio coordinatesToImageSizeRatio, IImage image)
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

	private void ToggleImageInfo()
	{
		TabOptions!.ShowImageViewImageInfo = !TabOptions!.ShowImageViewImageInfo;

		_imageInfoTextBox.IsVisible = TabOptions!.ShowImageViewImageInfo;
	}

	private async Task CloseWindow()
	{
		NotifyStopAnimation();
		NotifyStopSlideshow();

		await WaitForAnimationTask();

		Close();

		SetImageSource(null);
	}

	private void SetImageSource(IImage? image)
	{
		_imageControl.Source = image?.GetBitmap();

		if (_previousImage is not null &&
			_previousImage != _invalidImage)
		{
			_previousImage.Dispose();
		}

		_previousImage = image;
	}

	private async Task PauseBetweenImages(TimeSpan slideshowInterval)
	{
		var slideshowDelay = _imageFile!.IsAnimatedImage
			? _imageFile!.AnimatedImageSlideshowDelay
			: slideshowInterval;

		await Task.Delay(slideshowDelay, _ctsSlideshow!.Token);
	}

	private async Task WaitForAnimationTask()
	{
		if (_animationTask is not null)
		{
			await _animationTask;
			_animationTask = default;

			_ctsAnimation?.Dispose();
			_ctsAnimation = default;
		}
	}

	#endregion
}
