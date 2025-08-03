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
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling.Extensions;
using ImageFanReloaded.Keyboard;
using ImageFanReloaded.Mouse;

namespace ImageFanReloaded.Controls;

public partial class FullScreenImageWindow : Window, IImageView
{
	public FullScreenImageWindow()
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

	public IMouseCursorFactory? MouseCursorFactory
	{
		get => _mouseCursorFactory;
		set
		{
			_mouseCursorFactory = value;

			_standardCursor = _mouseCursorFactory!.StandardCursor.GetCursor();
			_zoomCursor = _mouseCursorFactory!.ZoomCursor.GetCursor();
			_dragCursor = _mouseCursorFactory!.DragCursor.GetCursor();
		}
	}

	public IScreenInfo? ScreenInfo { get; set; }
	public ITabOptions? TabOptions { get; set; }

	public bool IsStandaloneView { get; set; }

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

		Title = _imageFile.StaticImageFileData.ImageFileName;

		_negligibleImageDragX = imageFile.ImageSize.Width * NegligibleImageDragFactor;
		_negligibleImageDragY = imageFile.ImageSize.Height * NegligibleImageDragFactor;

		_scaledScreenSize ??= ScreenInfo!.GetScaledScreenSize(this);

		_canZoomToImageSize = CanZoomToImageSize();
		_screenSizeCursor = GetScreenSizeCursor();

		await ResizeToScreenSize();

		_imageInfoTextBox.Text = _imageFile.GetBasicImageInfo(TabOptions!.RecursiveFolderBrowsing);
		_imageInfoTextBox.IsVisible = TabOptions!.ShowImageViewImageInfo;
	}

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private const double ImageZoomScalingFactor = 0.1;
	private const double ImageScrollFactor = 0.1;
	private const double NegligibleImageDragFactor = 0.025;

	private const int OneImageForward = 1;
	private const int OneImageBackward = -1;

	private CancellationTokenSource? _ctsAnimation;
	private CancellationTokenSource? _ctsSlideshow;

	private Task? _animationTask;

	private IImage? _previousImage;

	private IGlobalParameters? _globalParameters;
	private IMouseCursorFactory? _mouseCursorFactory;

	private IImage? _invalidImage;

	private IImageFile? _imageFile;

	private Cursor? _standardCursor;
	private Cursor? _zoomCursor;
	private Cursor? _dragCursor;

	private double _negligibleImageDragX;
	private double _negligibleImageDragY;

	private ImageSize? _scaledScreenSize;

	private bool _canZoomToImageSize;
	private Cursor? _screenSizeCursor;

	private Point _mouseDownCoordinates;
	private Point _mouseUpCoordinates;

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
		else if (ShouldShowMainViewAfterImageViewClosing(keyModifiers, keyPressing))
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

		_mouseDownCoordinates = e.GetPosition(_displayImage);
	}

	private async void OnMouseUp(object? sender, PointerReleasedEventArgs e)
	{
		if (_imageViewState == ImageViewState.ResizedToScreenSize)
		{
			if (e.InitialPressMouseButton == MouseButton.Left && _canZoomToImageSize)
			{
				var mousePositionToImage = e.GetPosition(_displayImage);
				var imageSize = new ImageSize(
					_displayImage.Source!.Size.Width, _displayImage.Source!.Size.Height);

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
				_mouseUpCoordinates = e.GetPosition(_displayImage);

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

	private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
	{
		ViewClosing?.Invoke(this, new ImageViewClosingEventArgs(_showMainViewAfterImageViewClosing));
	}

	private bool ShouldStartSlideshow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.SKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleImageDrag(ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers)
	{
		if (keyModifiers == _globalParameters!.CtrlKeyModifier &&
			_imageViewState == ImageViewState.ZoomedToImageSize)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleBackwardNavigation(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			_globalParameters!.IsBackwardNavigationKey(keyPressing))
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleForwardNavigation(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			_globalParameters!.IsForwardNavigationKey(keyPressing))
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleImageZoom(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.EnterKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldToggleImageInfo(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.IKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleEscapeAction(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.EscapeKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldShowMainViewAfterImageViewClosing(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (IsStandaloneView &&
			keyModifiers == _globalParameters!.NoneKeyModifier &&
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
						=> _displayImage.Source = anImageFrameBitmap);

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
		var image = _imageFile!.GetResizedImage(
			_scaledScreenSize!, TabOptions!.ApplyImageOrientation);

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

	private (double normalizedDragX, double normalizedDragY) GetNormalizedDrag()
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
			normalizedDragX * ImageScrollFactor * _displayImage.Source!.Size.Width;
		var newVerticalScrollOffset = _imageScrollViewer.Offset.Y +
			normalizedDragY * ImageScrollFactor * _displayImage.Source!.Size.Height;

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
			mousePoint.X <= _displayImage.Source!.Size.Width &&
			mousePoint.Y >= 0 &&
			mousePoint.Y <= _displayImage.Source!.Size.Height)
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
			(_imageFile!.ImageSize.Width > _scaledScreenSize!.Width ||
			 _imageFile!.ImageSize.Height > _scaledScreenSize!.Height);

		return canZoomToImageSize;
	}

	private Cursor GetScreenSizeCursor() => _canZoomToImageSize ? _zoomCursor! : _standardCursor!;

	private void ZoomToImageSize(CoordinatesToImageSizeRatio coordinatesToImageSizeRatio)
	{
		var image = _imageFile!.GetImage(TabOptions!.ApplyImageOrientation);
		SetImageSource(image);

		_imageViewState = ImageViewState.ZoomedToImageSize;
		Cursor = _dragCursor!;

		var zoomRectangle = GetZoomRectangle(coordinatesToImageSizeRatio, image);
		_displayImage.BringIntoView(zoomRectangle);
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
		_displayImage.Source = image?.GetBitmap();

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
