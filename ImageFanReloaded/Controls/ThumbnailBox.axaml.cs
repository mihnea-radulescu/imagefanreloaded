using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling.Extensions;
using ImageFanReloaded.Mouse;

namespace ImageFanReloaded.Controls;

public partial class ThumbnailBox : UserControl, IThumbnailBox
{
	public ThumbnailBox()
	{
		InitializeComponent();
	}

	public event EventHandler<ThumbnailBoxSelectedEventArgs>? ThumbnailBoxSelected;
	public event EventHandler<ThumbnailBoxClickedEventArgs>? ThumbnailBoxClicked;

	public ITabOptions? TabOptions { get; set; }

	public IMouseCursorFactory? MouseCursorFactory
	{
		get => _mouseCursorFactory;
		set
		{
			_mouseCursorFactory = value;

			_standardCursor = _mouseCursorFactory!.StandardCursor.GetCursor();
			_zoomCursor = _mouseCursorFactory!.ZoomCursor.GetCursor();
		}
	}

	public int Index { get; set; }

	public IThumbnailInfo? ThumbnailInfo
	{
		get => _thumbnailInfo;
		set
		{
			_thumbnailInfo = value;

			ImageFile = _thumbnailInfo!.ImageFile;

			_thumbnailImage.Source = _thumbnailInfo.ThumbnailImage!.GetBitmap();

			_thumbnailTextBlock.Text = _thumbnailInfo.ThumbnailText;
			_thumbnailTextBlock.IsVisible = TabOptions!.ShowThumbnailImageFileName;
		}
	}

	public IImageFile? ImageFile { get; private set; }
	public bool HasImageReadError => ImageFile!.HasImageReadError;

	public bool IsSelected { get; private set; }

	public void SetControlProperties(int thumbnailSize, IGlobalParameters globalParameters)
	{
		_thumbnailImage.MaxWidth = thumbnailSize;
		_thumbnailImage.MaxHeight = thumbnailSize;
	}

	public void SelectThumbnail()
	{
		_thumbnailBoxBorder.BorderBrush = Brushes.DodgerBlue;
		Cursor = _zoomCursor!;
		IsSelected = true;

		BringThumbnailIntoView();

		ThumbnailBoxSelected?.Invoke(this, new ThumbnailBoxSelectedEventArgs(this));
	}

	public void UnselectThumbnail()
	{
		_thumbnailBoxBorder.BorderBrush = Brushes.LightGray;
		Cursor = _standardCursor!;
		IsSelected = false;
	}

	public void BringThumbnailIntoView() => this.BringIntoView();

	public void RefreshThumbnail()
	{
		var thumbnailImage = _thumbnailInfo!.ThumbnailImage!;

		if (thumbnailImage.IsAnimated)
		{
			_isAnimated = true;
			_ctsAnimation = new CancellationTokenSource();
			_animationTask = Task.Run(() => AnimateImage(_ctsAnimation), _ctsAnimation.Token);
		}
		else
		{
			_thumbnailImage.Source = thumbnailImage.GetBitmap();
		}
	}

	public async Task UpdateThumbnailAfterImageFileChange()
	{
		NotifyStopAnimation();

		await DisposeThumbnail();

		await Task.Run(() =>
		{
			_thumbnailInfo!.ReadThumbnailInputFromDisc();
			_thumbnailInfo!.GetThumbnail();
		});

		RefreshThumbnail();
	}

	public bool IsAnimated => _isAnimated;
	public Task AnimationTask => _animationTask ?? Task.CompletedTask;
	public void NotifyStopAnimation() => _ctsAnimation?.Cancel();

	public async Task DisposeThumbnail()
	{
		await WaitForAnimationTask();

		_thumbnailImage.Source = null;

		_thumbnailInfo!.DisposeThumbnail();
		ImageFile!.DisposeImageData();
	}

	#region Private

	private IThumbnailInfo? _thumbnailInfo;

	private IMouseCursorFactory? _mouseCursorFactory;

	private Cursor? _standardCursor;
	private Cursor? _zoomCursor;

	private bool _isAnimated;
	private CancellationTokenSource? _ctsAnimation;
	private Task? _animationTask;

	private void OnMouseClick(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			ThumbnailBoxClicked?.Invoke(
				this, new ThumbnailBoxClickedEventArgs(this, MouseClickType.Left));
		}
		else if (e.InitialPressMouseButton == MouseButton.Right)
		{
			ThumbnailBoxClicked?.Invoke(
				this, new ThumbnailBoxClickedEventArgs(this, MouseClickType.Right));
		}
	}

	private async Task AnimateImage(CancellationTokenSource ctsAnimation)
	{
		try
		{
			while (!ctsAnimation.IsCancellationRequested)
			{
				var thumbnailImageFrames = _thumbnailInfo!.ThumbnailImage!.ImageFrames;
				foreach (var aThumbnailImageFrame in thumbnailImageFrames)
				{
					if (ctsAnimation.IsCancellationRequested)
					{
						return;
					}

					var anImageFrameBitmap = aThumbnailImageFrame.GetBitmap();

					if (ctsAnimation.IsCancellationRequested)
					{
						return;
					}

					await Dispatcher.UIThread.InvokeAsync(()
						=> _thumbnailImage.Source = anImageFrameBitmap);

					if (ctsAnimation.IsCancellationRequested)
					{
						return;
					}

					await Task.Delay(aThumbnailImageFrame.DelayUntilNextFrame, ctsAnimation.Token);
				}
			}
		}
		catch
		{
		}
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
