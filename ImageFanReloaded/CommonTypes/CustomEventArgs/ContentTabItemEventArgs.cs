using System;
using ImageFanReloaded.Controls;

namespace ImageFanReloaded.CommonTypes.CustomEventArgs;

public class ContentTabItemEventArgs : EventArgs
{
	public ContentTabItemEventArgs(IContentTabItem contentTabItem)
	{
		ContentTabItem = contentTabItem;
	}

	public IContentTabItem ContentTabItem { get; }
}
