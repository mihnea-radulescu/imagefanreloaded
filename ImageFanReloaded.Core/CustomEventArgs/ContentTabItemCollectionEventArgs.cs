using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ContentTabItemCollectionEventArgs : EventArgs
{
	public ContentTabItemCollectionEventArgs(
		IReadOnlyList<IContentTabItem> contentTabItemCollection)
	{
		ContentTabItemCollection = contentTabItemCollection;
	}

	public IReadOnlyList<IContentTabItem> ContentTabItemCollection { get; }
}
