using System;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderOrderingChangedEventArgs : EventArgs
{
	public FolderOrderingChangedEventArgs(
		IContentTabItem contentTabItem, string path)
	{
		ContentTabItem = contentTabItem;

		Path = path;
	}

	public IContentTabItem ContentTabItem { get; }

	public string Path { get; }
}
