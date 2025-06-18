using System.Threading.Tasks;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageInfoExtractor
{
	Task<string> BuildImageInfo(IImageFile imageFile);
}
