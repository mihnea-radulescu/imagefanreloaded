using System;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderChangedEventArgs : EventArgs
{
	public FolderChangedEventArgs(
		IContentTabItem contentTabItem, string name, string path)
	{
		ContentTabItem = contentTabItem;

		Name = name;
		Path = path;
	}

	public IContentTabItem ContentTabItem { get; }

	public string Name { get; }
	public string Path { get; }
}
