using System;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IContentTabItemHeader
{
	IGlobalParameters? GlobalParameters { get; set; }
	IContentTabItem? ContentTabItem { get; set; }

	event EventHandler<ContentTabItemAddedEventArgs>? TabCloned;
	event EventHandler<ContentTabItemEventArgs>? TabClosed;

	void SetTabHeader(string tabTitle, string tabTooltip);

	void ShowTabCloseButton(bool showTabCloseButton);
}
