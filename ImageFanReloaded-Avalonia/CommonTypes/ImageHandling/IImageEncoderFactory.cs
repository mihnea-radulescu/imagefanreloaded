using SixLabors.ImageSharp.Formats;

namespace ImageFanReloaded.CommonTypes.ImageHandling;

public interface IImageEncoderFactory
{
	IImageEncoder GetImageEncoder(ImageFormat imageFormat);
}
