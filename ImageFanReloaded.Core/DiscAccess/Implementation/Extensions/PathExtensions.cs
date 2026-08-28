using System.IO;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;

public static class PathExtensions
{
	extension(Path)
	{
		public static char ZipArchiveDirectorySeparatorChar => '/';
	}
}
