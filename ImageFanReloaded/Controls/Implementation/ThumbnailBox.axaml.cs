using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.Controls.Implementation;

public partial class ThumbnailBox : UserControl, IRefreshableControl
{
	public ThumbnailBox()
	{
		InitializeComponent();

		SetControlProperties();
	}

	public event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxSelected;
	public event EventHandler<ThumbnailBoxEventArgs>? ThumbnailBoxClicked;

	public IImageFile? ImageFile { get; private set; }
	public bool IsSelected { get; private set; }

	public ThumbnailInfo ThumbnailInfo
	{
		get => _thumbnailInfo!;
		set
		{
			_thumbnailInfo = value;

			ImageFile = _thumbnailInfo.ImageFile;

			_thumbnailImage.Source = _thumbnailInfo.ThumbnailImage;
			_thumbnailTextBlock.Text = _thumbnailInfo.ThumbnailText;
		}
	}

	public void SelectThumbnail()
	{
		_thumbnailBoxBorder.BorderBrush = Brushes.Gray;
		Cursor = new Cursor(StandardCursorType.Hand);
		IsSelected = true;

		this.BringIntoView();
		
		ThumbnailBoxSelected?.Invoke(this, new ThumbnailBoxEventArgs(this));
	}

	public void UnselectThumbnail()
	{
		_thumbnailBoxBorder.BorderBrush = Brushes.LightGray;
		Cursor = new Cursor(StandardCursorType.Arrow);
		IsSelected = false;
	}

	public void Refresh()
	{
		_thumbnailImage.Source = _thumbnailInfo!.ThumbnailImage;
	}

	public void DisposeThumbnail()
	{
		_thumbnailInfo!.DisposeThumbnail();
		ImageFile!.DisposeImageData();
	}

	#region Private

	private ThumbnailInfo? _thumbnailInfo;

	private void SetControlProperties()
	{
		_thumbnailImage.MaxWidth = GlobalData.ThumbnailSize.Width;
		_thumbnailImage.MaxHeight = GlobalData.ThumbnailSize.Height;
	}

	private void OnMouseClick(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			ThumbnailBoxClicked?.Invoke(this, new ThumbnailBoxEventArgs(this));
		}
	}

	#endregion
}
