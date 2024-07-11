using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling;

namespace ImageFanReloaded.Controls;

public partial class ThumbnailBox : UserControl, IThumbnailBox
{
	static ThumbnailBox()
	{
		HandCursor = new Cursor(StandardCursorType.Hand);
		ArrowCursor = new Cursor(StandardCursorType.Arrow);
	}
	
	public ThumbnailBox()
	{
		InitializeComponent();
	}

	public event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxSelected;
	public event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxClicked;
	
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
		}
	}
	
	public IImageFile? ImageFile { get; private set; }
	public bool IsSelected { get; private set; }
	
	public void SetControlProperties(IGlobalParameters globalParameters)
	{
		_thumbnailImage.MaxWidth = globalParameters.ThumbnailSize.Width;
		_thumbnailImage.MaxHeight = globalParameters.ThumbnailSize.Height;
	}

	public void SelectThumbnail()
	{
		_thumbnailBoxBorder.BorderBrush = Brushes.Gray;
		Cursor = HandCursor;
		IsSelected = true;

		BringThumbnailIntoView();
		
		ThumbnailBoxSelected?.Invoke(this, new ThumbnailBoxEventArgs(this));
	}

	public void UnselectThumbnail()
	{
		_thumbnailBoxBorder.BorderBrush = Brushes.LightGray;
		Cursor = ArrowCursor;
		IsSelected = false;
	}

	public void BringThumbnailIntoView() => this.BringIntoView();

	public void RefreshThumbnail()
	{
		_thumbnailImage.Source = _thumbnailInfo!.ThumbnailImage!.GetBitmap();
	}

	public void DisposeThumbnail()
	{
		_thumbnailInfo!.DisposeThumbnail();
		ImageFile!.DisposeImageData();
	}

	#region Private
	
	private static readonly Cursor HandCursor;
	private static readonly Cursor ArrowCursor;

	private IThumbnailInfo? _thumbnailInfo;

	private void OnMouseClick(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			ThumbnailBoxClicked?.Invoke(this, new ThumbnailBoxEventArgs(this));
		}
	}

	#endregion
}
