using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;

public static class ZipArchiveEntryExtensions
{
	extension(ZipArchiveEntry zipArchiveEntry)
	{
		public bool IsFirstLevelSubFolder
			=> zipArchiveEntry.IsFolder &&
			   zipArchiveEntry.FullName.GetDirectorySeparatorCharCountInPath(
				   Path.DirectorySeparatorChar) == 1;

		public bool IsDirectSubFolderOf(string path)
			=> zipArchiveEntry.IsFolder &&
			   zipArchiveEntry.FullName.StartsWith(path) &&
			   zipArchiveEntry.FullName.Length > path.Length &&
			   zipArchiveEntry.FullName.GetDirectorySeparatorCharCountInPath(
				   Path.DirectorySeparatorChar) ==
			   path.GetDirectorySeparatorCharCountInPath(
				   Path.DirectorySeparatorChar) + 1;

		public bool IsImageFileDirectlyUnderPath(
			string path, HashSet<string> enabledImageFileExtensions)
				=> zipArchiveEntry.IsFile &&
				   enabledImageFileExtensions.Contains(
					   Path.GetExtension(zipArchiveEntry.Name)) &&
				   zipArchiveEntry.FullName.StartsWith(path) &&
				   zipArchiveEntry.FullName.Length > path.Length &&
				   zipArchiveEntry.FullName
					   .GetDirectorySeparatorCharCountInPath(
						   Path.DirectorySeparatorChar) ==
				   path.GetDirectorySeparatorCharCountInPath(
					   Path.DirectorySeparatorChar);

		private bool IsFolder => zipArchiveEntry.Length == 0;
		private bool IsFile => zipArchiveEntry.Length > 0;
	}
}
