using ImageFanReloaded.Core.DiscAccess.EntryInfo;

namespace ImageFanReloaded.Core.Controls;

public interface IFileSystemTreeViewItem
{
	IFileSystemEntryInfo? FileSystemEntryInfo { get; set; }
}
