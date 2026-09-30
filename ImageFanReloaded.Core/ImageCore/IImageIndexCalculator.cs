using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageCore;

public interface IImageIndexCalculator
{
	int GetIncrementedImageIndex(
		ITabOptions tabOptions, int imageCount, int imageIndex, int increment);
}
