using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class ContentTabItemAddedEventArgs : ContentTabItemEventArgs
{
	public ContentTabItemAddedEventArgs(
		IContentTabItem contentTabItem,
		string? inputPathToClone,
		bool isExpandedFolderTreeViewSelectedItem)
			: base(contentTabItem)
	{
		InputPathToClone = inputPathToClone;
		IsExpandedFolderTreeViewSelectedItem =
			isExpandedFolderTreeViewSelectedItem;
	}

	public string? InputPathToClone { get; }
	public bool IsExpandedFolderTreeViewSelectedItem { get; }

	public bool ShouldCloneInputPath => InputPathToClone is not null;
}
