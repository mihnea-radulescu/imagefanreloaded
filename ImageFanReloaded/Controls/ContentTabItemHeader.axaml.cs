using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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

	public event EventHandler<ContentTabItemEventArgs>? TabClosed;

	public void SetTabHeader(string tabTitle, string tabTooltip)
	{
		_tabTitleTextBlock.Text = tabTitle;
		_tabToolTipTextBlock.Text = tabTooltip;
	}

	public void ShowTabCloseButton(bool showTabCloseButton) => _tabCloseBorder.IsVisible = showTabCloseButton;

	private void OnControlLoaded(object? sender, RoutedEventArgs e)
		=> _tabToolTipTextBlock.FontSize = _tabTitleTextBlock.FontSize;

	private void OnTabClose(object? sender, PointerReleasedEventArgs e)
		=> TabClosed?.Invoke(this, new ContentTabItemEventArgs(ContentTabItem!));
}
