using System;
using ImageFanReloaded.CommonTypes.CustomEventArgs;

namespace ImageFanReloaded.Controls;

public interface IContentTabItemHeader
{
	public IContentTabItem? ContentTabItem { get; set; }

	public event EventHandler<ContentTabItemEventArgs>? TabClosed;
	
	void SetTabTitle(string tabTitle);

	void ShouldShowTabCloseButton(bool showTabCloseButton);
}
