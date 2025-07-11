namespace ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class BmpSaveFileImageFormat : ISaveFileImageFormat
{
	public string Extension => ".bmp";

	public bool IsAnimationEnabled => false;
}
