using ImageFanReloaded.Core.DiscAccess.EntryInfo;

namespace ImageFanReloaded.Core.Controls.Factories;

public interface IFolderVisualStateFactory
{
	IFolderVisualState GetFolderVisualState(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo fileSystemEntryInfo);
}
