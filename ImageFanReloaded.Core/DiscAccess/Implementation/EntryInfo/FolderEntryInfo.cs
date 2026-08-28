using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageCore;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public class FolderEntryInfo : DriveOrFolderEntryInfoBase
{
	public FolderEntryInfo(
		IFileSystemEntryInfo? parent,
		string folderName,
		string folderPath,
		bool zipArchivesEnabled,
		IImage folderIcon)
			: base(
				parent,
				folderName,
				folderPath,
				zipArchivesEnabled,
				folderIcon)
	{
	}
}
