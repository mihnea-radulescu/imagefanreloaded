using System;
using Avalonia.Controls;
using Avalonia.Input;
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

	public void SetTabTitle(string tabTitle)
		=> _textBlockTabTitle.Text = tabTitle;

	public void ShowTabCloseButton(bool showTabCloseButton)
		=> _borderTabClose.IsVisible = showTabCloseButton;

	#region Private

	private void OnTabClose(object? sender, PointerPressedEventArgs e)
		=> TabClosed?.Invoke(this, new ContentTabItemEventArgs(ContentTabItem!));

	#endregion
}
