using System;
using ImageFanReloaded.Core.CustomEventArgs;

namespace ImageFanReloaded.Core.Controls;

public interface IContentTabItemHeader
{
	public IContentTabItem? ContentTabItem { get; set; }

	public event EventHandler<ContentTabItemEventArgs>? TabClosed;

	void SetTabHeader(string tabTitle, string tabTooltip);

	void ShowTabCloseButton(bool showTabCloseButton);
}
