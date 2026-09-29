using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;
using ImageFanReloaded.Core.ImageCore;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.EntryInfo;

public abstract class ZipArchiveEntryInfoBase : FileSystemEntryInfoBase
{
	protected ZipArchiveEntryInfoBase(
		IFileSystemEntryInfo? parent,
		string name,
		string path,
		bool hasSubFolders,
		IImage icon)
			: base(parent, name, path, hasSubFolders, icon)
	{
	}

	protected static bool HasArchiveSubFoldersUnderPath(
		IReadOnlyList<ZipArchiveEntry> entries, string path)
	{
		try
		{
			var hasAnySubFolders = entries
				.Any(anEntry => anEntry.IsDirectSubFolderOf(path));

			return hasAnySubFolders;
		}
		catch
		{
			return false;
		}
	}
}
