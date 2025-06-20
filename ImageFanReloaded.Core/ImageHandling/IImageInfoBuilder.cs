using System.Threading.Tasks;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageInfoBuilder
{
	Task<string> BuildImageInfo(IImageFile imageFile);
}
