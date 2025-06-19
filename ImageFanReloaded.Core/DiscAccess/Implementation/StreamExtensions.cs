using System.IO;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public static class StreamExtensions
{
	public static void Reset(this Stream stream) => stream.Position = 0;
}
