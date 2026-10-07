using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.Controls.Extensions;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls;

public partial class ContentTabItemHeader : UserControl, IContentTabItemHeader
{
	public ContentTabItemHeader()
	{
		InitializeComponent();
	}

	public IGlobalParameters? GlobalParameters { get; set; }
	public IAccentColorChangePublisher? AccentColorChangePublisher { get; set; }
	public IContentTabItem? ContentTabItem { get; set; }

	public event EventHandler<ContentTabItemAddedEventArgs>? TabCloned;
	public event EventHandler<ContentTabItemEventArgs>? TabClosed;

	public void SetTabHeader(string tabTitle, string tabTooltip)
	{
		_tabTitleTextBlock.Text = tabTitle;
		_tabToolTipTextBlock.Text = tabTooltip;
	}

	public void ShowTabCloseButton(bool showTabCloseButton)
		=> _tabCloseBorder.IsVisible = showTabCloseButton;

	private void OnControlLoaded(object? sender, RoutedEventArgs e)
	{
		var accentColor = AccentColorChangePublisher!.GetAccentColor();
		SetTabCloseBorderBackground(accentColor);

		_tabToolTipTextBlock.FontSize = _tabTitleTextBlock.FontSize;

		AccentColorChangePublisher!.AccentColorChanged += OnAccentColorChanged;
	}

	private void OnControlDetachedFromVisualTree(
		object? sender, VisualTreeAttachmentEventArgs e)
	{
		AccentColorChangePublisher!.AccentColorChanged -= OnAccentColorChanged;
	}

	private void OnAccentColorChanged(
		object? sender, AccentColorChangedEventArgs e)
	{
		var accentColor = e.AccentColor;

		SetTabCloseBorderBackground(accentColor);
	}

	private void OnTabClone(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Right)
		{
			var activeFileSystemEntryInfo =
				ContentTabItem!.GetActiveFileSystemEntryInfo();
			var isExpandedFolderTreeViewSelectedItem =
				ContentTabItem!.GetIsExpandedFolderTreeViewSelectedItem();

			TabCloned?.Invoke(
				this,
				new ContentTabItemAddedEventArgs(
					ContentTabItem!,
					activeFileSystemEntryInfo,
					isExpandedFolderTreeViewSelectedItem));
		}
	}

	private void OnTabClose(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			TabClosed?.Invoke(
				this, new ContentTabItemEventArgs(ContentTabItem!));
		}
	}

	private void SetTabCloseBorderBackground(
		System.Drawing.Color tabCloseBorderBackgroundColor)
	{
		var avaloniaTabCloseBorderBackgroundColor =
			tabCloseBorderBackgroundColor.ToAvaloniaColor();

		_tabCloseBorder.Background =
			avaloniaTabCloseBorderBackgroundColor.GetColorBrush();
	}
}
