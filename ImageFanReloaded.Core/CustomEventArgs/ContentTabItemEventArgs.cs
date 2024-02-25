using System;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ContentTabItemEventArgs : EventArgs
{
	public ContentTabItemEventArgs(IContentTabItem contentTabItem)
	{
		ContentTabItem = contentTabItem;
	}

	public IContentTabItem ContentTabItem { get; }
}
