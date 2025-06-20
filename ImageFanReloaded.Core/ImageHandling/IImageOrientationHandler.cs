namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageOrientationHandler
{
	ushort GetImageOrientation(object imageObject);

	void ApplyImageOrientation(object imageObject, ushort imageOrientation);
}
