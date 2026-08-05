using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using ImageFanReloaded.Controls.Extensions;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;

namespace ImageFanReloaded.Controls;

public partial class ContentTabItemHeader : UserControl, IContentTabItemHeader
{
	public ContentTabItemHeader()
	{
		InitializeComponent();
	}

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

	private static IBrush? _accentColorBrush;

	private void OnControlLoaded(object? sender, RoutedEventArgs e)
	{
		_tabToolTipTextBlock.FontSize = _tabTitleTextBlock.FontSize;

		if (_accentColorBrush is not null)
		{
			_tabCloseBorder.Background = _accentColorBrush;
		}
		else
		{
			var accentColor = this.GetAccentColor();
			if (accentColor is not null)
			{
				_accentColorBrush = accentColor.Value.GetColorBrush();
				_tabCloseBorder.Background = _accentColorBrush;
			}
		}
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
					activeFileSystemEntryInfo!.Path,
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
}
