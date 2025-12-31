using System.IO;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public static class StreamExtensions
{
	extension(Stream stream)
	{
		public void Reset() => stream.Position = 0;
	}
}
