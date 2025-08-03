using System;
using System.Threading;
using System.Threading.Tasks;
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

namespace ImageFanReloaded.Controls;

public partial class WindowedImageWindow : Window, IImageView
{
	public WindowedImageWindow()
	{
		InitializeComponent();

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
		AddHandler(PointerReleasedEvent, OnMouseUp, RoutingStrategies.Tunnel);
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

	public IMouseCursorFactory? MouseCursorFactory { get; set; }

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

		if (_halfScaledScreenSize is null)
		{
			_halfScaledScreenSize = ScreenInfo!.GetHalfScaledScreenSize(this);

			Width = _halfScaledScreenSize.Width;
			Height = _halfScaledScreenSize.Height;
		}

		if (TabOptions!.ImageViewDisplayMode == ImageViewDisplayMode.WindowedMaximized)
		{
			WindowState = WindowState.Maximized;
		}

		_displayImage.MaxWidth = _imageFile.ImageSize.Width;
		_displayImage.MaxHeight = _imageFile.ImageSize.Height;

		await DisplayImage();
	}

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private const int OneImageForward = 1;
	private const int OneImageBackward = -1;

	private CancellationTokenSource? _ctsAnimation;
	private CancellationTokenSource? _ctsSlideshow;

	private Task? _animationTask;

	private IImage? _previousImage;

	private IGlobalParameters? _globalParameters;

	private IImage? _invalidImage;

	private IImageFile? _imageFile;

	private ImageSize? _halfScaledScreenSize;

	private bool _showMainViewAfterImageViewClosing;

	private void NotifyStopAnimation() => _ctsAnimation?.Cancel();
	private void NotifyStopSlideshow() => _ctsSlideshow?.Cancel();

	private async void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		NotifyStopSlideshow();

		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldMaximizeWindow(keyModifiers, keyPressing))
		{
			MaximizeWindow();
		}
		else if (ShouldRestoreNormalWindowSize(keyModifiers, keyPressing))
		{
			RestoreNormalWindowSize();
		}
		else if (ShouldStartSlideshow(keyModifiers, keyPressing))
		{
			await StartSlideshow();
		}
		else if (ShouldHandleBackwardNavigation(keyModifiers, keyPressing))
		{
			await RaiseImageChanged(OneImageBackward, false);
		}
		else if (ShouldHandleForwardNavigation(keyModifiers, keyPressing))
		{
			await RaiseImageChanged(OneImageForward, false);
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

	private async void OnMouseUp(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Right)
		{
			await HandleEscapeAction();
		}

		e.Handled = true;
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

	private bool ShouldMaximizeWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.MKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldRestoreNormalWindowSize(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == _globalParameters!.NoneKeyModifier &&
			keyPressing == _globalParameters!.NKey)
		{
			return true;
		}

		return false;
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

	private async Task HandleEscapeAction() => await CloseWindow();

	private async Task RaiseImageChanged(int increment, bool isSlideshow)
	{
		NotifyStopAnimation();

		ImageChanged?.Invoke(this, new ImageChangedEventArgs(this, increment));

		if (!isSlideshow && !CanAdvanceToDesignatedImage)
		{
			await CloseWindow();
		}
	}

	private void MaximizeWindow()
	{
		if (WindowState == WindowState.Normal)
		{
			WindowState = WindowState.Maximized;

			TabOptions!.ImageViewDisplayMode = ImageViewDisplayMode.WindowedMaximized;
		}
	}

	private void RestoreNormalWindowSize()
	{
		if (WindowState == WindowState.Maximized)
		{
			WindowState = WindowState.Normal;

			TabOptions!.ImageViewDisplayMode = ImageViewDisplayMode.Windowed;
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

	private async Task DisplayImage()
	{
		var image = _imageFile!.GetImage(TabOptions!.ApplyImageOrientation);

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
