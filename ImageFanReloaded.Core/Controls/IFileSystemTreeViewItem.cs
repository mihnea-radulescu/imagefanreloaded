using ImageFanReloaded.Core.DiscAccess;

namespace ImageFanReloaded.Core.Controls;

public interface IFileSystemTreeViewItem
{
	FileSystemEntryInfo? FileSystemEntryInfo { get; set; }
}
