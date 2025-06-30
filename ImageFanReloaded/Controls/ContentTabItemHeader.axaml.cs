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
		_textBlockTabTitle.Text = tabTitle;
		_textBlockTabTooltip.Text = tabTooltip;
	}

	public void ShowTabCloseButton(bool showTabCloseButton)
		=> _borderTabClose.IsVisible = showTabCloseButton;

	#region Private

	private void OnLoaded(object? sender, RoutedEventArgs e)
		=> _textBlockTabTooltip.FontSize = _textBlockTabTitle.FontSize;

	private void OnTabClose(object? sender, PointerPressedEventArgs e)
		=> TabClosed?.Invoke(this, new ContentTabItemEventArgs(ContentTabItem!));

	#endregion
}
