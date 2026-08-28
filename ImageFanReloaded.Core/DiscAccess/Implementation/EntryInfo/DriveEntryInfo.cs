using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageCore;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public class DriveEntryInfo : DriveOrFolderEntryInfoBase
{
	public DriveEntryInfo(
		IFileSystemEntryInfo? parent,
		string driveName,
		string drivePath,
		bool zipArchivesEnabled,
		IImage driveIcon)
			: base(parent, driveName, drivePath, zipArchivesEnabled, driveIcon)
	{
	}
}
