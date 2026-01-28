using System.IO;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFileContentReader
{
	Stream? GetImageFileContentStream(string imageFilePath);
}
