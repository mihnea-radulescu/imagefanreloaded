using System;
using ImageFanReloaded.Controls;

namespace ImageFanReloaded.CommonTypes.CustomEventArgs;

public class TabItemEventArgs
	: EventArgs
{
	public TabItemEventArgs(IContentTabItem contentTabItem)
	{
		ContentTabItem = contentTabItem;
	}

	public IContentTabItem ContentTabItem { get; }
}
